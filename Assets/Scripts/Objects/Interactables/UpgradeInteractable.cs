using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UpgradeInteractable : ButtonInteractable
{
    [SerializeField] private Upgrade m_UpgradeStats;
    [SerializeField] private GameObject m_UpgradeDisplay;

    private GameObject FindChildWithName(GameObject parent, string name)
    {
        GameObject res = null;
        foreach (Transform child in parent.transform)
        {

            if (child.name == name)
            {
                res = child.gameObject;
                break;
            }

            else
            {
                res = FindChildWithName(child.gameObject, name);
            }
        }
        return res;
    }

    private BoardPanel m_BoardPanel;

    public override void Awake()
    {
        m_UpgradeStats = transform.parent.GetComponent<Upgrade>();

        // this is awful
        m_BoardPanel = transform.parent.parent.parent.parent.parent.parent.GetComponent<BoardPanel>();
        m_UpgradeDisplay = m_BoardPanel.m_UpgradeDisplay;
    }

    public override void Interaction()
    {
        if (!m_BoardPanel.m_UpgradeDisplay.activeSelf)
        {
            FindChildWithName(m_UpgradeDisplay, "Name").GetComponent<TextMeshProUGUI>().text = m_UpgradeStats.m_UpgradeData.m_UpgradeName;
            FindChildWithName(m_UpgradeDisplay, "Description").GetComponent<TextMeshProUGUI>().text = m_UpgradeStats.m_UpgradeData.m_Description;

            FindChildWithName(m_UpgradeDisplay, "Level").GetComponent<TextMeshProUGUI>().text =
                "Level " + m_UpgradeStats.m_UpgradeData.m_CurrLevel.ToString() + " / " + m_UpgradeStats.m_UpgradeData.m_MaxLevel.ToString();
        }

        m_BoardPanel.ToggleUpgradeMenu(true);
    }
}