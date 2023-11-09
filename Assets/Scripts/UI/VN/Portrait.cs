using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Portrait : MonoBehaviour
{
    public NPCController m_OwnerNPC;

    public Dictionary<string, Sprite> m_SprContainer;

    private List<Sprite> m_Eyes;
    private List<Sprite> m_Mouths;
    private List<Sprite> m_TalkingMouths;
    private Sprite m_CurrMouth;

    public string m_PortraitName;
    public string m_TexName;

    public Image m_SprBody;
    public Image m_SprEyes;
    public Image m_SprMouth;

    public Vector2 m_EyesPivot;
    public Vector2 m_MouthPivot;

    [Min(0)] public float m_TalkingSpeed;
    private float m_CurrTalkingFrame;

    private List<int> m_AllowedIndices = new List<int>();
    private int m_SelectedIndex;

    void Awake()
    {
    }

    public void LoadPortrait(bool hasEyes, NPCController npc)
    {
        // Read in sprites from .psb file in Resources/Sprites folder
        Sprite[] sprites = Resources.LoadAll<Sprite>("Sprites/" + m_TexName);

        // array to dictionary, key/value pairing by sprite name, sprite
        m_SprContainer = sprites.ToDictionary(x => x.name.ToLower(), x => x);

        // Always set defaults first
        m_SprBody.sprite = m_SprContainer["body"];

        if (hasEyes)
        {
            m_Eyes = m_SprContainer.Where(e => e.Key.Contains("eyes")).Select(e => e.Value).ToList();
            m_SprEyes.sprite = m_Eyes[0];
            m_SprEyes.enabled = true;
        }

        m_Mouths = m_SprContainer.Where(e => e.Key.Contains("mouth")).Select(e => e.Value).ToList();
        m_TalkingMouths = m_SprContainer.Where(e => e.Key.Contains("talking")).Select(e => e.Value).ToList();

        m_SprMouth.sprite = m_Mouths[0];
        m_CurrMouth = m_SprMouth.sprite;

        // Move sprite face local position to appropriate position
        m_SprEyes.rectTransform.localPosition = new Vector3(m_EyesPivot.x, m_EyesPivot.y, 0.0f);
        m_SprMouth.rectTransform.localPosition = new Vector3(m_MouthPivot.x, m_MouthPivot.y, 0.0f);

        // Set body/face sprites to native size from original sprite sheet/.psb file
        m_SprBody.SetNativeSize();

        if (hasEyes)
            m_SprEyes.SetNativeSize();

        m_SprMouth.SetNativeSize();

        m_CurrTalkingFrame = m_TalkingSpeed;
        m_OwnerNPC = npc;

        // set up the talking mouth container
        m_SelectedIndex = Random.Range(0, m_TalkingMouths.Count);
        for (int i = 0; i < m_TalkingMouths.Count; ++i)
        {
            if (i == m_SelectedIndex)
                continue;

            m_AllowedIndices.Add(i);
        }
    }

    void Update()
    {
        if (!VNHandler.m_Instance.m_TextBox.m_FinishedCurrLine)
        {
            m_CurrTalkingFrame -= Time.deltaTime;
            if (m_CurrTalkingFrame <= 0.0f)
            {
                m_CurrTalkingFrame = m_TalkingSpeed;

                int idx = Random.Range(0, m_AllowedIndices.Count);
                int selected = m_AllowedIndices[idx];
                m_SprMouth.sprite = m_TalkingMouths[selected];

                m_AllowedIndices.Remove(selected);
                m_AllowedIndices.Add(m_SelectedIndex);

                m_SelectedIndex = selected;

                m_SprMouth.SetNativeSize();
                m_OwnerNPC.PlayTalkingSFX();
            }
        }
        else
        {
            // might be bad??
            if (m_SprMouth.sprite != m_CurrMouth)
            {
                m_SprMouth.sprite = m_CurrMouth;
                m_SprMouth.SetNativeSize();
            }
        }
    }
}
