using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Upgrade : MonoBehaviour, ISavableData
{
    public GameObject m_CurrencyPanel;

    public UpgradeData m_UpgradeData;

    public void SaveData(ref GameData data)
    {
        UpgradeData upgr = data.FindUpgrade(m_UpgradeData.m_UpgradeName);
        upgr.m_CurrLevel = m_UpgradeData.m_CurrLevel;
        upgr.m_MaxLevel = m_UpgradeData.m_MaxLevel;
        upgr.m_BaseCost = m_UpgradeData.m_BaseCost;
    }

    public void LoadData(GameData data)
    {
        UpgradeData upgr = data.FindUpgrade(m_UpgradeData.m_UpgradeName);
        m_UpgradeData.m_CurrLevel = upgr.m_CurrLevel;
        m_UpgradeData.m_MaxLevel = upgr.m_MaxLevel;
        m_UpgradeData.m_BaseCost = upgr.m_BaseCost;
    }

    public void OpenUpgradeMenu()
    {
        m_CurrencyPanel.GetComponent<BoardPanel>().ToggleUpgradeMenu(true);
    }
}

[System.Serializable]
public struct UpgradeData
{
    // If an upgrade is for a specified currency (e.g. game currency, exp),
    // then this will be used. If it's of an unspecified type (e.g. UpgradeType == UNSPECIFIED),
    // then the description will be used.
    [System.Serializable]
    public struct Bonus
    {
        // current bonus to specific upgrade
        public float m_CurrentBonus;

        // "X bonus is given every Y levels..."
        // BonusMultiplier is X
        // RequiredLevels is Y
        public int m_RequiredLevels;
        public float m_BonusMultiplier;
        public enum UpgradeType
        {
            UNSPECIFIED,
            Currency,
            Exp,
            Tier
        }
    }

    public string m_UpgradeName;

    // These will be empty if the bonus is specified. Otherwise,
    // they will be manually filled
    public string m_Description;
    public string m_BonusDesc;

    public int m_CurrLevel, m_MaxLevel;

    public Bonus m_UpgradeType;

    [Min(1)]
    public int m_BaseCost;
}
