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
        GameCurrency buying = null;
        // if you can buy the upgrade
        if (m_UpgrToHandle.m_UpgradeData.CanBuyLevel(out buying))
        {
            // safety check
            if (buying == null)
                return;

            // clicked buy once
            if (!m_BuyMax)
            {
                // update the multiplier of the upgrade
                Upgrade.UpdateMultiplier(m_UpgrToHandle, Upgrade.Operation.Add,
                    m_UpgrToHandle.m_UpgradeData.m_UpgradeBonuses.m_BonusMultiplier);

                // update the value of the currency that was used to buy this upgrade
                buying.UpdateCurrency(-m_UpgrToHandle.m_UpgradeData.IntCost());
                StatsManager.m_Instance.Player.
                    GetComponent<PlayerController>().PlayerUI.UpdateText(buying);

                // increase level by 1 (do this last)
                m_UpgrToHandle.m_UpgradeData.m_CurrLevel += 1;
            }

            // clicked buy max
            else
            { 
            
            }

            m_UpgradePanel.UpdateLevelText(m_UpgrToHandle);
        }
    }
}
