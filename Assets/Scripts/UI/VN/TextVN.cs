using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class TextVN : MonoBehaviour
{
    public bool m_IsTyping = true;
    public bool m_FinishedCurrLine = false;
    private bool m_ParsedLine = false;
    public bool m_TypingSFX = false;

    private int m_CurrTxtPos = 0;

    public string m_LineToParse;

    private string m_TxName;
    private string m_TxLocation;
    private string m_Text;

    private float m_CurrTime;

    [Min(0.0f)]
    public float m_TimeBetweenTxt;

    public TextMeshProUGUI m_DialogTMP;
    public TextMeshProUGUI m_NameTMP;
    public TextMeshProUGUI m_LocationTMP;

    void Update()
    {
        if (m_ParsedLine)
        {
            TypeText(m_Text);
            return;
        }

    }

    // https://stackoverflow.com/questions/14655023/split-a-string-that-has-white-spaces-unless-they-are-enclosed-within-quotes
    public void ParseTextLine()
    {
        // Note: there should always be 4 elements for a dialog line (token, name, school, dialog)
        List<string> parts = m_LineToParse.Split('"').Select((element, index) => index % 2 == 0 ?
        element.Split(new[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries)
        : new string[] { element }).SelectMany(element => element).ToList();

        m_TxName = parts[1];
        m_TxLocation = parts[2];
        m_Text = parts[3];

        m_NameTMP.text = m_TxName;
        m_LocationTMP.text = m_TxLocation;

        m_ParsedLine = true;
    }

    public void ClearText()
    {
        m_DialogTMP.text = "";
    }

    public void ClearName()
    {
        m_NameTMP.text = "";
    }

    public void ClearLocation()
    {
        m_LocationTMP.text = "";
    }

    public void TypeText(string line)
    {
        // Only type if flag is set
        if (m_IsTyping)
        {
            // Increment time
            m_CurrTime += Time.fixedDeltaTime;

            // If time is greater than specified delay between text...
            if (m_CurrTime > m_TimeBetweenTxt)
            {
                // Reset current time
                m_CurrTime = 0.0f;

                // Add a single character of line onto screen
                m_DialogTMP.text += line.Substring(m_CurrTxtPos, 1);
                ++m_CurrTxtPos;

                if (m_TypingSFX)
                    VNHandler.m_Instance.m_SourceSFX.PlayOneShot(VNHandler.m_Instance.m_TypingSFX.GetSound(1));

                // If current position is at end of line, then set flags to indicate
                // that this line is finished typing
                if (m_CurrTxtPos >= line.Length)
                {
                    m_FinishedCurrLine = true;
                    m_IsTyping = false;
                    m_CurrTxtPos = 0;
                }
            }
        }
    }
}