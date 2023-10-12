using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Portrait : MonoBehaviour
{
    public Dictionary<string, Sprite> m_SprContainer;

    public string m_PortraitName;
    public string m_TexName;

    public Image m_SprBody;
    public Image m_SprFace;
    public Image m_SprHalo;

    public Vector2 m_FacePivot;

    void Awake()
    {
    }

    public void LoadPortrait()
    {
        // Read in sprites from .psb file in Resources/Sprites folder
        Sprite[] sprites = Resources.LoadAll<Sprite>("Sprites/" + m_TexName);

        // array to dictionary, key/value pairing by sprite name, sprite
        m_SprContainer = sprites.ToDictionary(x => x.name.ToLower(), x => x);

        // Always set defaults first
        m_SprBody.sprite = m_SprContainer["body"];
        m_SprFace.sprite = m_SprContainer["face1"];

        // Move sprite face local position to appropriate position
        m_SprFace.rectTransform.localPosition = new Vector3(m_FacePivot.x, m_FacePivot.y, 0.0f);

        // Set body/face sprites to native size from original sprite sheet/.psb file
        m_SprBody.SetNativeSize();
        m_SprFace.SetNativeSize();
    }

    void Update()
    {

    }
}
