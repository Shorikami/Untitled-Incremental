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
        m_BoardPanel = transform.GetComponentInParent<BoardPanel>();
        m_UpgradeDisplay = m_BoardPanel.m_UpgradeDisplay;
    }

    public override void Interaction()
    {
        if (!m_BoardPanel.m_UpgradeDisplay.activeSelf)
        {
            m_UpgradeDisplay.GetComponent<UpgradePanel>().UpdateText(m_UpgradeStats);

            m_UpgradeDisplay.GetComponent<UpgradePanel>()
                .m_BuyOne.gameObject.GetComponent<MultiplierInteractable>()
                .UpdateHandledUpgrade(m_UpgradeStats);

            m_UpgradeDisplay.GetComponent<UpgradePanel>()
                .m_BuyMax.gameObject.GetComponent<MultiplierInteractable>()
                .UpdateHandledUpgrade(m_UpgradeStats);
        }

        m_BoardPanel.ToggleUpgradeMenu(true);
    }
}
