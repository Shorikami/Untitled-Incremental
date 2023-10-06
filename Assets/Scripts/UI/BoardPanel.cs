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

        // todo: serializable data
        m_Upgrades.Add(CreateNewUpgrade("test", "lorem ipsum", 0, 100));
    }

    private GameObject CreateNewUpgrade(string name, string desc, int curLev, int maxLev)
    {
        GameObject upgr = Instantiate(m_UpgradePrefab, m_PanelContent.transform);
        upgr.GetComponent<Upgrade>().m_CurrencyPanel = this.gameObject;

        upgr.GetComponent<Upgrade>().m_UpgradeName = name;
        upgr.GetComponent<Upgrade>().m_Description = desc;
        upgr.GetComponent<Upgrade>().m_CurrLevel = curLev;
        upgr.GetComponent<Upgrade>().m_MaxLevel = maxLev;

        return upgr;
    }

    public void ToggleUpgradeMenu(bool open)
    {
        m_ScrollPanel.SetActive(!open);
        m_UpgradeDisplay.SetActive(open);
    }
}
