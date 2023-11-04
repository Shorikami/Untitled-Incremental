using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PersistentUpgradeText : MonoBehaviour
{
    public TextMeshProUGUI m_Level;
    public TextMeshProUGUI m_Description;

    public void Initialize(int lvl, string w, int width)
    {
        m_Level.rectTransform.sizeDelta = new Vector2(width, m_Level.rectTransform.sizeDelta.y);
        m_Description.rectTransform.sizeDelta = new Vector2(width, m_Description.rectTransform.sizeDelta.y);

        m_Level.text = lvl.ToString();
        EditText(w);
    }

    private void EditText(string what)
    {
        m_Description.text = what;
    }
}
