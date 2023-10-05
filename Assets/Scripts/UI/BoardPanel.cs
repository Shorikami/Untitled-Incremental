using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardPanel : MonoBehaviour
{
    [Header("UI Board")]
    [SerializeField] private GameObject m_ScrollPanel;
    [SerializeField] private GameObject m_PanelContent; 

    [SerializeField] private GameObject m_UpgradePrefab;

    private List<GameObject> m_Upgrades = new List<GameObject>();

    private void Awake()
    {
        
    }

    private void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        CreateUpgrades();
    }

    public void CreateUpgrades(int spacing = 5)
    {
        m_PanelContent.GetComponent<HorizontalLayoutGroup>().spacing = spacing;

        // loop
        for (int i = 0; i < 6; ++i)
        {
            m_Upgrades.Add(CreateNewUpgrade());
        }
    }

    private GameObject CreateNewUpgrade()
    {
        GameObject upgr = Instantiate(m_UpgradePrefab, m_PanelContent.transform);
        upgr.GetComponent<Upgrade>().m_CurrencyPanel = this.gameObject;

        return upgr;
    }
}
