using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Incr2DHandler : MonoBehaviour, ISavableData
{
    // THIS IS NOT A SINGLETON SO IT WILL BE DESTROYED AND ANY 2D INFO WILL BE LOST!!
    // THIS IS GOOD BECAUSE THE GAME WILL EVENTUALLY TRANSITION TO 3D!!!!


    // 2D Game canvas
    public GameObject m_GameCanvas;
    
    // exp bar, currency display, maybe tier bar soon?
    public GameObject m_ExpBar;
    public TextMeshProUGUI m_ExpText;
    public GameObject m_CurrencyText;

    // Prefab of currencies
    public GameObject m_CurrencyPrefab;

    // Currently active currencies (this should be private)
    public List<GameObject> m_ActivePanels;
    private int m_ActiveCurrIdx;

    [Min(1)]
    private int m_Width, m_Height;

    public void Awake()
    {
        m_ActivePanels = new List<GameObject>();
    }

    public void SaveData(ref GameData data)
    {
        //// this only happens when it's a new game
        //// todo: make this more efficient?
        //if (data.m_Currencies.Count == 0)
        //{
        //    data.m_Currencies.Add(m_ActivePanels[0].GetComponent<CurrencyPanel>().m_Currency);
        //}
        //
        //else
        //{
        //    for (int i = 0; i < m_ActivePanels.Count; ++i)
        //    {
        //        Currency c = m_ActivePanels[i].GetComponent<CurrencyPanel>().m_Currency;
        //        data.m_Currencies[i] = c;
        //    }
        //}
    }

    public void LoadData(GameData data)
    {
       //foreach (Currency c in data.m_Currencies)
       //{
       //    GameObject addedCurr = Instantiate(m_CurrencyPrefab, m_GameCanvas.transform);
       //    addedCurr.GetComponent<CurrencyPanel>().m_Currency = c;
       //
       //    m_ActivePanels.Add(addedCurr);
       //}
       //
       //// will only hit if currencies are empty
       //// 1 currency must always be initialized: the starting currency (dollars)
       //if (data.m_Currencies.Count == 0)
       //{
       //    GameObject firstCurr = Instantiate(m_CurrencyPrefab, m_GameCanvas.transform);
       //    firstCurr.GetComponent<CurrencyPanel>().m_Currency.m_Rank = 0;
       //    firstCurr.GetComponent<CurrencyPanel>().m_Currency.m_CurrencyType = Currency.CurrencyType.Dollars;
       //
       //    m_ActivePanels.Add(firstCurr);
       //}
       //
       //// initialize to dollars display first. always safe?
       //// todo: check active status of other currencies to switch this dynamically on start up
       //m_ActiveCurrIdx = 0;
    }

    void Start()
    {
        m_Width = Mathf.RoundToInt(m_GameCanvas.GetComponent<RectTransform>().rect.width);
        m_Height = Mathf.RoundToInt(m_GameCanvas.GetComponent<RectTransform>().rect.height);

        //for (int i = 0; i < 2; ++i)
        //{
        //    m_Currencies.Add(Instantiate(m_CurrencyPrefab, m_GameCanvas.transform));
        //}

        // initialize in order
        int c = 0;
        foreach (GameObject g in m_ActivePanels)
        {
            g.GetComponent<CurrencyPanel>().Initialize(m_Width, m_Height, c, 200, 200,
                c < 3 ? CurrencyPanel.Side.Left : CurrencyPanel.Side.Right);

            ++c;
        }

        UpdateText();
    }

    void Update()
    {
        
    }

    public void UpdateValue(int val)
    {
        m_ActivePanels[m_ActiveCurrIdx].GetComponent<CurrencyPanel>().m_Currency.UpdateCurrency(val);
        gameObject.GetComponent<Experience>().AddExperience(val);
        UpdateText();
    }

    public void UpdateText(string color = "#5c5c5c")
    {
        m_CurrencyText.GetComponent<TextMeshProUGUI>().text = 
            "<color=" + color + ">" + 
            m_ActivePanels[m_ActiveCurrIdx].GetComponent<CurrencyPanel>().m_Currency.m_TotalCount.ToString()
            + "</color>";

        Experience xp = gameObject.GetComponent<Experience>();

        m_ExpText.text = "<color=#0ae0f0><b>" + xp.m_ExpData.m_CurrExp + " / " + xp.m_ExpData.RequiredExperience() + "</b></color>";
        m_ExpBar.GetComponent<Slider>().value = (float)xp.m_ExpData.m_CurrExp / (float)xp.m_ExpData.RequiredExperience();
    }
}
