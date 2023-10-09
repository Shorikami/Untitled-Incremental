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
    [SerializeField] private GameObject m_HorizontalBlankPrefab;

    [SerializeField] private bool m_IsVertical = false;
    [SerializeField][Min(0)] private float m_Spacing;

    [SerializeField] private StatsManager.GameCurrencyType m_CurrencyDisplay;

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
        if (m_IsVertical)
            CreateUpgradesVertical();
        else
            CreateUpgradesHorizontal();
    }

    private void CreateUpgradesHorizontal()
    {
        m_PanelContent.GetComponent<HorizontalLayoutGroup>().spacing = m_Spacing;

        List<Upgrade> upgrades = StatsManager.m_Instance.FindUpgrades(m_CurrencyDisplay);

        foreach (Upgrade ugr in upgrades)
            m_Upgrades.Add(CreateNewUpgrade(ugr.m_UpgradeData, m_PanelContent.transform));

        //m_Upgrades.Add(CreateNewUpgrade("test", "lorem ipsum", 0, 100));
    }

    private void CreateUpgradesVertical()
    {
        m_PanelContent.GetComponent<VerticalLayoutGroup>().spacing = m_Spacing;

        List<Upgrade> upgrades = StatsManager.m_Instance.FindUpgrades(m_CurrencyDisplay);
        int iter = 0;

        GameObject currHorizPanel = null;

        foreach (Upgrade ugr in upgrades)
        {
            if (iter % 2 == 0)
                currHorizPanel = Instantiate(m_HorizontalBlankPrefab, m_PanelContent.transform);

            m_Upgrades.Add(CreateNewUpgrade(ugr.m_UpgradeData, currHorizPanel.transform));
            ++iter;
        }
    }

    private GameObject CreateNewUpgrade(UpgradeData loadedUpgr, Transform parent)
    {
        GameObject upgr = Instantiate(m_UpgradePrefab, parent);
        upgr.GetComponent<Upgrade>().m_CurrencyPanel = this.gameObject;

        upgr.GetComponent<Upgrade>().m_UpgradeData = loadedUpgr;

        return upgr;
    }

    public void ToggleUpgradeMenu(bool open)
    {
        m_ScrollPanel.SetActive(!open);
        m_UpgradeDisplay.SetActive(open);
    }
}
