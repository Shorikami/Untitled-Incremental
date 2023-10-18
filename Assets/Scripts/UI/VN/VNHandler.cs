using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;
using TMPro;

public enum AudioAdd { BGM, SFX }
public enum AudioFadeAction { None, In, Out }
public class VNHandler : MonoBehaviour
{
    public enum FADE_EFFECT
    {
        FADE_In,
        FADE_Out
    }

    public static VNHandler m_Instance { get; private set; }
    public string m_CurrCutsceneName;

    private bool m_CutsceneIsActive;
    public bool CutsceneActive
    { 
        get { return m_CutsceneIsActive; }
        set { m_CutsceneIsActive = value; }
    }

    public List<string> m_LoadedCutscene;
    private int m_LoadedTxtIdx = 0;

    public TextVN m_TextBox;

    public List<GameObject> m_CharacterPortraits;
    public Dictionary<string, Sprite> m_Backgrounds;

    public GameObject m_Background;
    public GameObject m_LayoutBack;
    public GameObject m_BlackWhiteFade;

    public GameObject m_PortraitBase;

    public GameObject m_ChoiceLayoutGroup;
    public GameObject m_ChoiceButtonPrefab;
    private List<GameObject> m_ActiveChoiceButtons = new List<GameObject>();
    public bool m_ButtonChoiceActive;

    public AudioSource m_SourceBGM;
    public AudioSource m_SourceSFX;

    public Dictionary<string, AudioClip> m_BGMs;
    public Dictionary<string, AudioClip> m_SFXs;
    public AudioContainer m_TypingSFX;

    public string m_TargetLine;
    public bool m_FindTargetLine;

    InputAction i_LMB;
    InputAction i_RMB;

    // Waiting after being marked
    private bool m_Waiting = false;
    public bool Waiting { get { return m_Waiting; } set { m_Waiting = value; } }

    // Enable this flag once to progress cutscene after waiting
    private bool m_EndWaiting = false;
    public bool WaitFlag { get { return m_EndWaiting; } set { m_EndWaiting = value; } }

    // Flag to see if waiting was caused by an action (movement, shaking, fading, etc.) 
    private bool m_WaitStoppedByAction;

    public bool WaitByAction { get { return m_WaitStoppedByAction; } set { m_WaitStoppedByAction = value; } }

    private float m_ElapsedWaiting;
    private float m_WaitingTime;

    private float m_FadeTime;
    private float m_ElapsedFadeTime;
    private FADE_EFFECT m_FadeEffect;

    private AudioFadeAction m_AudioAction;
    private float m_FadeTimeBGM;
    private float m_ElapsedFadeBGM;
    private float m_OriginalVolumeBGM;

    // Singleton instancing
    private void Awake()
    {
        if (m_Instance && m_Instance != this)
        {
            Destroy(this);
            return;
        }
        m_Instance = this;
        DontDestroyOnLoad(this.gameObject);

        i_LMB = new InputAction(binding: "<Mouse>/leftButton");
        i_RMB = new InputAction(binding: "<Mouse>/rightButton");

        i_LMB.Enable();
        i_RMB.Enable();

        m_CharacterPortraits = new List<GameObject>();
        m_Backgrounds = new Dictionary<string, Sprite>();
        m_BGMs = new Dictionary<string, AudioClip>();
        m_SFXs = new Dictionary<string, AudioClip>();

        m_OriginalVolumeBGM = m_SourceBGM.volume;
        m_CutsceneIsActive = false;
    }

    void Start()
    {
        m_LayoutBack.SetActive(false);
        //LoadTextFile(m_CurrCutsceneName);
    }

    void Update()
    {
        if (m_Waiting && !m_WaitStoppedByAction)
        {
            m_ElapsedWaiting += Time.deltaTime;

            if (m_ElapsedWaiting >= m_WaitingTime)
            {
                m_ElapsedWaiting = 0.0f;
                m_Waiting = false;
                m_EndWaiting = true;
            }
        }

        if (!m_Waiting)
            ProgressCutscene();

        if (m_FadeTime > 0.0f)
        {
            m_ElapsedFadeTime += Time.deltaTime;

            float c = m_FadeEffect == FADE_EFFECT.FADE_In ? (m_ElapsedFadeTime / m_FadeTime) : 1.0f - (m_ElapsedFadeTime / m_FadeTime);

            Color bwFade = m_BlackWhiteFade.GetComponent<Image>().color;

            m_BlackWhiteFade.GetComponent<Image>().color = new Color(bwFade.r, bwFade.g, bwFade.b, c);

            if (m_ElapsedFadeTime > m_FadeTime)
            {
                m_FadeTime = 0.0f;
                m_ElapsedFadeTime = 0.0f;

                m_Waiting = false;
                m_EndWaiting = true;
                m_WaitStoppedByAction = false;
            }
        }

        // occurs simultaneously, so no waiting
        if (m_FadeTimeBGM > 0.0f)
        {
            m_ElapsedFadeBGM += Time.deltaTime;

            if (m_AudioAction == AudioFadeAction.In)
                m_SourceBGM.volume = (m_ElapsedFadeBGM / m_FadeTimeBGM);
            else if (m_AudioAction == AudioFadeAction.Out)
                m_SourceBGM.volume = 1.0f - (m_ElapsedFadeBGM / m_FadeTimeBGM);

            if (m_ElapsedFadeBGM > m_FadeTimeBGM)
            {
                if (m_AudioAction == AudioFadeAction.Out)
                {
                    m_AudioAction = AudioFadeAction.None;
                    m_SourceBGM.Stop();
                    m_SourceBGM.clip = null;
                    m_SourceBGM.volume = m_OriginalVolumeBGM;
                }

                m_ElapsedFadeBGM = 0.0f;
                m_FadeTimeBGM = 0.0f;
            }
        }

        if (m_FindTargetLine)
        {
            FindTargetLine();
        }
    }

    private void ProgressCutscene()
    {
        // If clicking and the current text line is finished AND it's not at the end of the read-in document
        if (m_TextBox.m_FinishedCurrLine && (Mathf.Approximately(i_LMB.ReadValue<float>(), 1.0f) || m_EndWaiting) &&
            m_LoadedTxtIdx < m_LoadedCutscene.Count && !m_ButtonChoiceActive)
        {
            string toParse = m_LoadedCutscene[m_LoadedTxtIdx++];
            List<string> tokens = ParseLine(toParse, ' ');

            switch (tokens[0])
            {
                // modify backgrounds (disabled for now)
                case "bg":
                    //if (string.Compare(tokens[1], "set") == 0)
                    //{
                    //    string key = tokens[2];
                    //
                    //    m_Background.SetActive(true);
                    //    m_Background.transform.SetSiblingIndex(0);
                    //    m_Background.GetComponent<Image>().sprite = m_Backgrounds[key];
                    //}
                    break;

                // modifying bgm
                case "bgm":
                    // instantly set bgm
                    if (string.Compare(tokens[1], "set") == 0)
                    {
                        string key = tokens[2];
                        m_SourceBGM.clip = m_BGMs[key];
                        m_SourceBGM.Play();
                    }

                    // fade in/out bgm
                    else if (string.Compare(tokens[1], "fade") == 0)
                    {
                        float bfTime = float.Parse(tokens[3]);

                        m_AudioAction = string.Compare(tokens[2], "out") == 0 ? AudioFadeAction.Out : AudioFadeAction.In;
                        m_FadeTimeBGM = bfTime;
                    }
                    break;

                // playing sfx
                case "se":
                    if (string.Compare(tokens[1], "set") == 0)
                    {
                        string key = tokens[2];
                        float volume = tokens.Count > 3 && tokens[3] != null ?
                            Mathf.Max(0.0f, Mathf.Min(float.Parse(tokens[3]), 1.0f)) : 1.0f;

                        m_SourceSFX.PlayOneShot(m_SFXs[key], volume);
                    }
                    break;

                // Handling text lines
                case "t":
                case "txt":
                    if (m_EndWaiting)
                        m_EndWaiting = false;

                    // Clear out any existing text
                    m_TextBox.ClearText();

                    // Set the line to be checked and then parse it
                    m_TextBox.m_LineToParse = toParse;
                    m_TextBox.ParseTextLine();

                    // Set flags for typing
                    m_TextBox.m_IsTyping = true;
                    m_TextBox.m_FinishedCurrLine = false;
                    break;

                // wait
                case "wait":
                case "w":
                    float time = float.Parse(tokens[1]);
                    WaitCommand(time);
                    break;

                // fade in/out
                case "fade":
                case "f":
                    // "fade in black 0.2"
                    // AND
                    // "fade out 0.5"
                    // are both valid
                    float fTime = float.Parse((tokens.Count > 3) ? tokens[3] : tokens[2]);
                    string color = (tokens.Count > 3) && (tokens[2] != null) ? tokens[2] : "";

                    // color, effect (in/out), time
                    Fade(color, tokens[1], fTime);
                    break;

                // create buttons
                case "button":
                case "b":
                    CreateButtons(tokens);
                    break;

                // jump to point
                case "jump":
                case "j":
                    string target = tokens[1];
                    m_TargetLine = target;
                    m_FindTargetLine = true;
                    return;

                // by default, modify sprites
                default:
                    // Object to modify
                    GameObject toModify = m_CharacterPortraits.Find(x =>
                    string.Compare(x.GetComponent<Portrait>().m_PortraitName, tokens[0]) == 0);

                    SelectCommand(toModify, tokens);
                    break;
            }
        }
    }

    #region Commands
    private void CreateButtons(List<string> tokens)
    {
        // Get the previous line (since the index would have moved by this point)
        string parseThis = m_LoadedCutscene[m_LoadedTxtIdx - 1];

        List<string> parts = parseThis.Split('"').Select((element, index) => index % 2 == 0 ?
        element.Split(new[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries)
        : new string[] { element }).SelectMany(element => element).ToList();

        m_ButtonChoiceActive = true;

        // assume 1 button will always be created
        int bCount = 1;

        string txt = parts[1].Trim('"');
        m_ActiveChoiceButtons.Add(Instantiate(m_ChoiceButtonPrefab, m_ChoiceLayoutGroup.transform));
        m_ActiveChoiceButtons[bCount - 1].GetComponentInChildren<TextMeshProUGUI>().text = txt;
        m_ActiveChoiceButtons[bCount - 1].GetComponent<ChoiceButton>().m_Target = parts[2];

        if (parts.Count > 3)
        {
            bCount += 1;
            txt = parts[3].Trim('\"');
            m_ActiveChoiceButtons.Add(Instantiate(m_ChoiceButtonPrefab, m_ChoiceLayoutGroup.transform));
            m_ActiveChoiceButtons[bCount - 1].GetComponentInChildren<TextMeshProUGUI>().text = txt;
            m_ActiveChoiceButtons[bCount - 1].GetComponent<ChoiceButton>().m_Target = parts[4];
        }

        if (parts.Count > 5)
        {
            bCount += 1;
            txt = parts[5].Trim('\"');
            m_ActiveChoiceButtons.Add(Instantiate(m_ChoiceButtonPrefab, m_ChoiceLayoutGroup.transform));
            m_ActiveChoiceButtons[bCount - 1].GetComponentInChildren<TextMeshProUGUI>().text = txt;
            m_ActiveChoiceButtons[bCount - 1].GetComponent<ChoiceButton>().m_Target = parts[6];
        }
    }

    private void SelectCommand(GameObject toModify, List<string> tokens)
    {
        switch (tokens[1])
        {
            // Changing sprite face state
            case "state":
            case "st":
                ChangeSpriteState(toModify, tokens);
                break;

            // Moving sprite
            case "move":
            case "mv":
                MoveSprite(toModify, tokens);
                break;

            // shaking sprite
            case "shake":
            case "sh":
                ShakeSprite(toModify, tokens);
                break;

            // zooming in/out sprite
            case "zoom":
            case "zm":
                ZoomSprite(toModify, tokens);
                break;

            // fade in/out sprite
            case "fade":
            case "f":
                FadeSprite(toModify, tokens);
                break;

            // instantly show sprite
            case "show":
            case "s":
                DisplaySprite(toModify, tokens, true);
                break;

            // instantly hide sprite
            case "hide":
            case "h":
                DisplaySprite(toModify, tokens, false);
                break;

            // switch sprite ordering
            case "priority":
            case "pr":
                int pIdx = int.Parse(tokens[2]);
                int pRealIdx = Mathf.Max(m_CharacterPortraits.Count - pIdx, 0);

                toModify.transform.SetSiblingIndex(pRealIdx);
                break;

            // highlight sprite
            case "highlight":
            case "hl":
                float val = float.Parse(tokens[2]);

                toModify.GetComponent<SpriteMovement>().Highlight(val);
                break;
        }
    }

    private void ChangeSpriteState(GameObject toModify, List<string> tokens)
    {
        Portrait pr = toModify.GetComponent<Portrait>();

        // Sprite sheet index
        string idx = tokens[2];
        pr.m_SprFace.sprite = pr.m_SprContainer[idx];

        // Recursive call to progress to the next line
        ProgressCutscene();
    }

    private void MoveSprite(GameObject toModify, List<string> tokens)
    {
        string mAxis = tokens[2];
        float mPos = float.Parse(tokens[3]);
        float mTime = float.Parse(tokens[4]);

        string mWaitToken = tokens.Count > 5 ? tokens[5] : null;
        m_Waiting = (mWaitToken != null && string.Compare(mWaitToken, "wait") == 0) ? true : false;
        m_WaitStoppedByAction = m_Waiting;

        toModify.GetComponent<SpriteMovement>().MovePortrait(mAxis, mPos, mTime);
    }

    private void ShakeSprite(GameObject toModify, List<string> tokens)
    {
        string sAxis = tokens[2];
        float sSpeed = float.Parse(tokens[3]);
        float sTime = float.Parse(tokens[4]);
        float sAmp = float.Parse(tokens[5]);

        string sWaitToken = tokens.Count > 6 ? tokens[6] : null;
        m_Waiting = (sWaitToken != null && string.Compare(sWaitToken, "wait") == 0) ? true : false;
        m_WaitStoppedByAction = m_Waiting;

        toModify.GetComponent<SpriteMovement>().ShakePortrait(sAxis, sSpeed, sTime, sAmp);
    }

    private void ZoomSprite(GameObject toModify, List<string> tokens)
    {
        string zDir = tokens[2];

        if (string.Compare(zDir, "close") == 0)
        {
            toModify.GetComponent<SpriteMovement>().CloseUp();
        }
        else if (string.Compare(zDir, "back") == 0)
        {
            toModify.GetComponent<SpriteMovement>().MoveBack();
        }
    }

    private void FadeSprite(GameObject toModify, List<string> tokens)
    {
        string action = tokens[2];
        m_Waiting = true;
        m_WaitStoppedByAction = true;

        if (string.Compare(action, "in") == 0)
        {
            toModify.GetComponent<SpriteMovement>().Show();
        }
        else if (string.Compare(action, "out") == 0)
        {
            toModify.GetComponent<SpriteMovement>().Hide();
        }
    }

    private void DisplaySprite(GameObject toModify, List<string> tokens, bool show)
    {
        if (show)
        {
            toModify.GetComponent<SpriteMovement>().Show(true);
        }

        else
        {
            toModify.GetComponent<SpriteMovement>().Hide(true);
        }
    }

    // Split a line into different tokens based on a character (' ', '\n', etc.)
    // and then store them into a list
    private List<string> ParseLine(string line, char c)
    {
        return line.Split(c).Select((element, index) => index % 2 == 0 ?
            element.Split(new[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries)
            : new string[] { element }).SelectMany(element => element).ToList();
    }
    #endregion
    #region LoadingUtility
    // Loading text files in the StreamingAssets folder
    public void LoadTextFile(string name, NPCController npc)
    {
        m_LayoutBack.SetActive(true);

        string path = Application.streamingAssetsPath + "/VN Events/" + name + ".txt";
        m_LoadedCutscene = File.ReadAllLines(path).Where(line => line != "" && !line.Contains("//")).ToList();

        // Load any assets into the cutscene (portraits, music, bg, etc.)
        PreLoadCutscene(npc);

        // force set both flags to true (cannot assume first line after pre-loading is text)
        m_EndWaiting = true;
        m_TextBox.m_FinishedCurrLine = true;

        // Parse the line after loading
        //m_TextBox.m_LineToParse = m_LoadedCutscene[m_LoadedTxtIdx++];
        //m_TextBox.ParseTextLine();
    }

    public void PreLoadCutscene(NPCController npc)
    {
        // Each loading token should start with "load"
        foreach (string s in m_LoadedCutscene)
        {
            // token, command, name, file
            List<string> tokens = ParseLine(s, ' ');
            string what = tokens[1];

            if (string.Compare(what, "prtrC") == 0)
            {
                string prtName = tokens[2];
                string file = tokens[3];

                string[] faceOffset = tokens[4].Split(':');
                string[] xy = faceOffset[1].Split(',');
                Vector2 offset = new Vector2(int.Parse(xy[0]), int.Parse(xy[1]));

                string[] startPos = tokens[5].Split(':');
                float x = float.Parse(startPos[1]);

                LoadCharacterPortrait(prtName, file, offset, x, npc);
            }

            else if (string.Compare(what, "bgm") == 0)
            {
                string key = tokens[2];
                string file = tokens[3];

                AddAudio(AudioAdd.BGM, key, file);
            }

            else if (string.Compare(what, "se") == 0)
            {
                string key = tokens[2];
                string file = tokens[3];

                AddAudio(AudioAdd.SFX, key, file);
            }

            else if (string.Compare(what, "bg") == 0)
            {
                string key = tokens[2];
                string file = tokens[3];
                AddBackground(key, file);
            }

            else if (string.Compare(what, "end") == 0)
            {
                ++m_LoadedTxtIdx;
                return;
            }
            ++m_LoadedTxtIdx;
        }
    }

    // Load a character portrait if specified
    public void LoadCharacterPortrait(string name, string tex, Vector2 offset, float xStart, NPCController npc)
    {
        // Instantiate it as a game object
        GameObject portrait = Instantiate(m_PortraitBase);

        // Parent it to the back layout UI
        portrait.transform.SetParent(m_LayoutBack.transform);

        // Center its position and scale it to be normal size
        portrait.GetComponent<SpriteMovement>().m_LayoutBack = m_LayoutBack;
        portrait.GetComponent<SpriteMovement>().m_PortraitBase = portrait;
        portrait.GetComponent<SpriteMovement>().SetPortraitPosition(new Vector3(xStart, 0, 0));
        portrait.transform.localScale = new Vector3(1, 1, 1);

        // Set its index to move it behind the text background
        portrait.transform.SetSiblingIndex(m_CharacterPortraits.Count);

        // Set portrait name for switching later, texture name to read in,
        // and face pivot for specific positioning of the face
        portrait.GetComponent<Portrait>().m_PortraitName = name;
        portrait.GetComponent<Portrait>().m_TexName = tex;
        portrait.GetComponent<Portrait>().m_FacePivot = offset;

        // Load it after setting the above
        portrait.GetComponent<Portrait>().LoadPortrait();
        npc.NPCPortrait = portrait.GetComponent<Portrait>();

        // Add new portrait to container of portraits in VN handler
        m_CharacterPortraits.Add(portrait);
    }

    public void AddBackground(string key, string path)
    {
        Sprite spr = Resources.Load<Sprite>("Backgrounds/" + path);
        m_Backgrounds.Add(key, spr);
    }

    public void AddAudio(AudioAdd what, string key, string path)
    {
        string dir = what == AudioAdd.BGM ? "Audio/BGM/" : "Audio/SFX/";

        AudioClip acl = Resources.Load<AudioClip>(dir + path);

        switch (what)
        {
            case AudioAdd.BGM:
                m_BGMs.Add(key, acl);
                break;
            case AudioAdd.SFX:
                m_SFXs.Add(key, acl);
                break;
        }

    }
    #endregion
    #region Misc
    private void WaitCommand(float time)
    {
        m_Waiting = true;
        m_WaitingTime = time;
    }

    private void FindTargetLine()
    {
        bool found = false;
        while (!found)
        {
            string toParse = m_LoadedCutscene[m_LoadedTxtIdx++];
            List<string> tokens = ParseLine(toParse, ' ');

            if (string.Compare(tokens[0], "target") != 0)
                continue;

            if (string.Compare(tokens[1], m_TargetLine) == 0)
                found = true;
        }

        // should not reach this return
        if (!found)
            return;

        m_FindTargetLine = false;
        m_EndWaiting = true;

        if (m_ButtonChoiceActive)
        {
            m_ButtonChoiceActive = false;
            foreach (GameObject g in m_ActiveChoiceButtons)
            {
                Destroy(g);
            }
            m_ActiveChoiceButtons.Clear();
        }
    }

    // Waits + also counts as an action
    public void Fade(string color, string effect, float time)
    {
        bool fadeIn = string.Compare(effect, "in") == 0 ? true : false;
        m_FadeEffect = fadeIn ? FADE_EFFECT.FADE_In : FADE_EFFECT.FADE_Out;

        m_BlackWhiteFade.SetActive(true);

        // color only matters if fading in
        if (fadeIn)
        {
            if (string.Compare(color, "white") == 0)
                m_BlackWhiteFade.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 0.0f);

            // black by default
            else
                m_BlackWhiteFade.GetComponent<Image>().color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
        }

        m_FadeTime = time;

        // if it's 0, then instant fade
        if (Mathf.Approximately(time, 0.0f))
        {
            m_BlackWhiteFade.SetActive(false);
            m_Waiting = false;
            m_EndWaiting = true;
            m_WaitStoppedByAction = false;
            return;
        }

        m_Waiting = true;
        m_WaitStoppedByAction = true;
    }

    #endregion
}
