using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardPanel : MonoBehaviour
{
    [Header("UI Board")]
    [SerializeField] private GameObject m_Board;
    [SerializeField] private GameObject m_ScrollPanel;
    public GameObject m_UpgradeDisplay;
    [SerializeField] private GameObject m_PanelContent;

    [SerializeField] private GameObject m_UpgradePrefab;

    private List<GameObject> m_Upgrades = new List<GameObject>();

    private void Awake()
    {
        Camera mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        m_Board.GetComponent<Canvas>().worldCamera = mainCamera;
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

        foreach (GameObject u in StatsManager.m_Instance.m_LoadedDataNodes)
        {
            if (u.TryGetComponent<Upgrade>(out Upgrade upgr))
            {
                m_Upgrades.Add(CreateNewUpgrade(upgr.m_UpgradeData));
            }
        }
        //m_Upgrades.Add(CreateNewUpgrade("test", "lorem ipsum", 0, 100));
    }

    private GameObject CreateNewUpgrade(UpgradeData loadedUpgr)
    {
        GameObject upgr = Instantiate(m_UpgradePrefab, m_PanelContent.transform);
        upgr.GetComponent<Upgrade>().m_CurrencyPanel = this.gameObject;

        upgr.GetComponent<Upgrade>().m_UpgradeData.m_UpgradeName = loadedUpgr.m_UpgradeName;
        upgr.GetComponent<Upgrade>().m_UpgradeData.m_Description = loadedUpgr.m_Description;
        upgr.GetComponent<Upgrade>().m_UpgradeData.m_CurrLevel = loadedUpgr.m_CurrLevel;
        upgr.GetComponent<Upgrade>().m_UpgradeData.m_MaxLevel = loadedUpgr.m_MaxLevel;
        upgr.GetComponent<Upgrade>().m_UpgradeData.m_BaseCost = loadedUpgr.m_BaseCost;

        return upgr;
    }

    public void ToggleUpgradeMenu(bool open)
    {
        m_ScrollPanel.SetActive(!open);
        m_UpgradeDisplay.SetActive(open);
    }
}
