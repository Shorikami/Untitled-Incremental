using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MultiplierInteractable : ButtonInteractable
{
    [SerializeField] private UpgradePanel m_UpgradePanel;
    [SerializeField] private Upgrade m_UpgrToHandle;

    public enum BuyButtonType
    { 
        None = 0,
        One = 1,
        NextBonus,
        Max
    }

    public BuyButtonType m_BuyButtonType;

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

            switch (m_BuyButtonType)
            {
                case BuyButtonType.One:
                    // update the multiplier of the upgrade
                    Upgrade.UpdateMultiplier(m_UpgrToHandle, Upgrade.Operation.Add,
                        m_UpgrToHandle.m_UpgradeData.m_UpgradeBonuses.m_BonusMultiplier);

                    // update the value of the currency that was used to buy this upgrade
                    buying.UpdateCurrency(-m_UpgrToHandle.m_UpgradeData.IntCostToNextLevel());

                    // increase level by 1 (do this last)
                    m_UpgrToHandle.m_UpgradeData.m_CurrLevel += 1;
                    break;

                // TODO: next bonus + max can probably be optimized without loops to calculate how
                // many upgrades can be bought
                case BuyButtonType.NextBonus:
                    // How many levels to buy
                    // if the required levels isn't specified, then it will buy 10 by default on this button
                    int lvlDiff = m_UpgrToHandle.m_UpgradeData.m_UpgradeBonuses.m_RequiredLevels == 0 ? 10 :
                        m_UpgrToHandle.m_UpgradeData.m_UpgradeBonuses.m_RequiredLevels  -
                        (m_UpgrToHandle.m_UpgradeData.m_CurrLevel % m_UpgrToHandle.m_UpgradeData.m_UpgradeBonuses.m_RequiredLevels);

                    // buy as many levels until they cannot buy anymore
                    for (int i = 0; i < lvlDiff; ++i)
                    {
                        if (buying.m_Currency.m_TotalValue >= m_UpgrToHandle.m_UpgradeData.CostToNextLevel())
                        {
                            Upgrade.UpdateMultiplier(m_UpgrToHandle, Upgrade.Operation.Add,
                                m_UpgrToHandle.m_UpgradeData.m_UpgradeBonuses.m_BonusMultiplier);

                            buying.UpdateCurrency(-m_UpgrToHandle.m_UpgradeData.IntCostToNextLevel());

                            m_UpgrToHandle.m_UpgradeData.m_CurrLevel += 1;
                            continue;
                        }
                        break;
                    }
                    break;

                case BuyButtonType.Max:
                    while (buying.m_Currency.m_TotalValue >= m_UpgrToHandle.m_UpgradeData.CostToNextLevel())
                    {
                        Upgrade.UpdateMultiplier(m_UpgrToHandle, Upgrade.Operation.Add,
                            m_UpgrToHandle.m_UpgradeData.m_UpgradeBonuses.m_BonusMultiplier);

                        buying.UpdateCurrency(-m_UpgrToHandle.m_UpgradeData.IntCostToNextLevel());

                        m_UpgrToHandle.m_UpgradeData.m_CurrLevel += 1;
                    }
                    break;
            }
            StatsManager.m_Instance.Player.
                GetComponent<PlayerController>().PlayerUI.UpdateText(buying);

            m_UpgradePanel.UpdateLevelText(m_UpgrToHandle);
        }
    }
}
