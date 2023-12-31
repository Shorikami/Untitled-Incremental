using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Upgrade : MonoBehaviour, ISavableData
{
    public GameObject m_CurrencyPanel;
    public GameObject m_MainButton;
    public GameObject m_AutobuyButton;

    public UpgradeData m_UpgradeData;

    public enum Operation { Add = 0, Multiply };

    public void SaveData(ref GameData data)
    {
        UpgradeData upgr = data.FindUpgrade(m_UpgradeData);
        upgr.m_CurrLevel = m_UpgradeData.m_CurrLevel;
        upgr.m_MaxLevel = m_UpgradeData.m_MaxLevel;
        upgr.m_BaseCost = m_UpgradeData.m_BaseCost;
        upgr.m_CollectableType = m_UpgradeData.m_CollectableType;
        upgr.m_CurrencyType = m_UpgradeData.m_CurrencyType;
        upgr.m_NonCurrType = m_UpgradeData.m_NonCurrType;
        upgr.m_UpgradeBonuses = m_UpgradeData.m_UpgradeBonuses;
        upgr.m_Description = m_UpgradeData.m_Description;
        upgr.m_BoughtWith = m_UpgradeData.m_BoughtWith;
    }

    public void LoadData(GameData data)
    {
        UpgradeData upgr = data.FindUpgrade(m_UpgradeData);
        m_UpgradeData.m_CurrLevel = upgr.m_CurrLevel;
        m_UpgradeData.m_MaxLevel = upgr.m_MaxLevel;
        m_UpgradeData.m_BaseCost = upgr.m_BaseCost;
        m_UpgradeData.m_CollectableType = upgr.m_CollectableType;
        m_UpgradeData.m_CurrencyType = upgr.m_CurrencyType;
        m_UpgradeData.m_NonCurrType = upgr.m_NonCurrType;
        m_UpgradeData.m_UpgradeBonuses = upgr.m_UpgradeBonuses;
        m_UpgradeData.m_Description = upgr.m_Description;
        m_UpgradeData.m_BoughtWith = upgr.m_BoughtWith;
    }

    public void ResetData()
    {
        m_UpgradeData.m_CurrLevel = 0;
        m_UpgradeData.m_UpgradeBonuses.m_CurrentBonus = 1.0f;
    }

    public void OpenUpgradeMenu()
    {
        m_CurrencyPanel.GetComponent<UpgradeBoardUI>().ToggleUpgradeMenu(true);
    }

    public static void UpdateMultiplier(Upgrade ug, Operation what, float val)
    {
        switch (what)
        {
            case Operation.Add:
                ug.m_UpgradeData.m_UpgradeBonuses.m_CurrentBonus += val;
                break;
            case Operation.Multiply:
                ug.m_UpgradeData.m_UpgradeBonuses.m_CurrentBonus *= val;
                break;
        }

        // Apply bonus if modulo returns 0, current level isn't 0, and required levels is greater
        // than 0 (anything equal or less implies there's no bonus)
        if (ug.m_UpgradeData.m_UpgradeBonuses.m_RequiredLevels > 0 &&
            ug.m_UpgradeData.m_CurrLevel % ug.m_UpgradeData.m_UpgradeBonuses.m_RequiredLevels == 0 && ug.m_UpgradeData.m_CurrLevel != 0)
            ug.m_UpgradeData.m_UpgradeBonuses.m_CurrentBonus *= ug.m_UpgradeData.m_UpgradeBonuses.m_LevelBonus;
    }
}

[System.Serializable]
public class UpgradeData : Data
{
    // If an upgrade is for a specified currency (e.g. game currency, exp),
    // then this will be used. If it's of an unspecified type (e.g. UpgradeType == UNSPECIFIED),
    // then the description will be used.
    [System.Serializable]
    public class Bonus
    {
        // current bonus to specific upgrade
        public float m_CurrentBonus = 1.0f;

        // how much the bonus is
        public float m_BonusMultiplier;

        // "X bonus is given every Y levels..."
        // LevelBonus is X
        // RequiredLevels is Y
        public int m_RequiredLevels;
        public float m_LevelBonus;
    }

    public string m_UpgradeName;

    // These will be empty if the bonus is specified. Otherwise,
    // they will be manually filled
    public string m_Description;
    public string m_BonusDesc;

    public string m_Formula;

    public int m_CurrLevel, m_MaxLevel;

    // whether or not this upgrade increases cost per level
    public bool m_StaticCost;

    public Bonus m_UpgradeBonuses;
    public StatsManager.NonCurrencyUpgrades m_NonCurrType = StatsManager.NonCurrencyUpgrades.Invalid;

    // What this upgrade is bought with
    public StatsManager.GameCurrencyType m_BoughtWith;

    [Min(1)]
    public double m_BaseCost;

    public void Save(UpgradeData other)
    {
        m_UpgradeName = other.m_UpgradeName;
        m_Description = other.m_Description;
        m_BonusDesc = other.m_BonusDesc;
        m_UpgradeBonuses = other.m_UpgradeBonuses;
        m_CurrLevel = other.m_CurrLevel;
        m_MaxLevel = other.m_MaxLevel;
        m_BaseCost = other.m_BaseCost;
        m_CollectableType = other.m_CollectableType;
        m_CurrencyType = other.m_CurrencyType;
        m_BoughtWith = other.m_BoughtWith;
    }

    public bool CanBuyLevel(out GameCurrency currToModify)
    {
        if (m_CurrLevel >= m_MaxLevel)
        {
            currToModify = null;
            return false;
        }

        currToModify = StatsManager.m_Instance.FindGameCurrency(m_CollectableType, m_BoughtWith);
        return currToModify.m_Currency.m_TotalValue >= CostToNextLevel();
    }

    public double CostToNextLevel()
    {
        if (m_StaticCost)
            return m_BaseCost;

        return Formula();
    }

    public int IntCostToNextLevel()
    {
        return System.Convert.ToInt32(CostToNextLevel());
    }

    public double Cost(int lvlAmountToCheck)
    {
        double total = 0.0;

        total += Formula(lvlAmountToCheck);

        if (lvlAmountToCheck == 0)
            return total;

        return total + Cost(lvlAmountToCheck - 1);
    }

    private double Formula(int addToLevel = 0)
    {
        return System.Math.Floor(m_BaseCost + (m_BaseCost * Mathf.Pow(1.1f, m_CurrLevel + addToLevel)));
    }
}
