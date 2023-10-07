using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MultiplierInteractable : ButtonInteractable
{
    [SerializeField] private UpgradePanel m_UpgradePanel;
    [SerializeField] private Upgrade m_UpgrToHandle;
    [SerializeField] private bool m_BuyMax;

    public override void Awake()
    {

    }

    public void UpdateHandledUpgrade(Upgrade ugr)
    {
        m_UpgrToHandle = ugr;
    }

    public override void Interaction()
    {
        if (!m_BuyMax)
        {
            m_UpgrToHandle.m_UpgradeData.m_CurrLevel += 1;
            Upgrade.UpdateMultiplier(m_UpgrToHandle, Upgrade.Operation.Add, m_UpgrToHandle.m_UpgradeData.m_UpgradeBonuses.m_BonusMultiplier);
            StatsManager.m_Instance.UpdateMultiplier(m_UpgrToHandle.m_UpgradeData, m_UpgrToHandle.m_UpgradeData.m_UpgradeBonuses.m_CurrentBonus);
        }

        m_UpgradePanel.UpdateLevelText(m_UpgrToHandle);
    }
}
