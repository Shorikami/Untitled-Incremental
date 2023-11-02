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

    private UpgradeBoardUI m_BoardPanel;

    public override void Awake()
    {
        m_UpgradeStats = transform.parent.GetComponent<Upgrade>();
        m_BoardPanel = transform.GetComponentInParent<UpgradeBoardUI>();
        m_UpgradeDisplay = m_BoardPanel.m_UpgradeDisplay;
        base.Awake();
    }

    public override void Interaction()
    {
        if (!m_BoardPanel.m_UpgradeDisplay.activeSelf)
        {
            UpgradePanel upgrp = m_UpgradeDisplay.GetComponent<UpgradePanel>();
            upgrp.UpdateText(m_UpgradeStats);

            upgrp.m_BuyOne.gameObject.GetComponent<MultiplierInteractable>()
                .UpdateHandledUpgrade(m_UpgradeStats);

            // not guaranteed the buy next button will exist on the board
            if (upgrp.m_BuyNext != null)
                upgrp.m_BuyNext.gameObject.GetComponent<MultiplierInteractable>()
                    .UpdateHandledUpgrade(m_UpgradeStats);

            upgrp.m_BuyMax.gameObject.GetComponent<MultiplierInteractable>()
                .UpdateHandledUpgrade(m_UpgradeStats);
        }

        m_BoardPanel.ToggleUpgradeMenu(true);
    }
}
