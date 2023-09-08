using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Incr2DHandler : MonoBehaviour
{
    // 2D Game canvas
    public GameObject m_GameCanvas;

    // Prefab of currencies
    public GameObject m_CurrencyPrefab;

    // Currently active currencies (this should be private) 
    public List<GameObject> m_CurrCurrencies;
    
    [Min(1)]
    private int m_Width, m_Height;

    void Start()
    {
        m_Width = Mathf.RoundToInt(m_GameCanvas.GetComponent<RectTransform>().rect.width);
        m_Height = Mathf.RoundToInt(m_GameCanvas.GetComponent<RectTransform>().rect.height);

        for (int i = 0; i < 6; ++i)
        {
            m_CurrCurrencies.Add(Instantiate(m_CurrencyPrefab, m_GameCanvas.transform));
        }

        // initialize in order
        int c = 0;
        foreach (GameObject g in m_CurrCurrencies)
        {
            g.GetComponent<CurrencyPanel>().Initialize(m_Width, m_Height, c,
                c < 3 ? CurrencyPanel.Side.Left : CurrencyPanel.Side.Right);

            ++c;
        }
    }

    void Update()
    {
        
    }
}
