using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public List<CurrencyData> m_GameCurrencies;
    public List<ExperienceData> m_ExperienceTypes;
    public List<UpgradeData> m_AvailableUpgrades;

    // Constructor for when starting new game
    public GameData()
    {
        m_GameCurrencies = new List<CurrencyData>();
        m_ExperienceTypes = new List<ExperienceData>();
        m_AvailableUpgrades = new List<UpgradeData>();
    }

    // ----
    // TODO: Binary search? they'll be small containers so it might not be necessary but whatever
    // ----
    public void SaveCurrency(CurrencyData data)
    {
        m_GameCurrencies.Find(searched => searched.m_CurrencyType == data.m_CurrencyType).Save(data);
    }

    public void SaveExperience(ExperienceData data)
    {
        m_ExperienceTypes.Find(searched => searched.m_CurrencyType == data.m_CurrencyType).Save(data);
    }

    public void SaveUpgrade(UpgradeData data)
    {
        m_AvailableUpgrades.Find(searched => searched.m_UpgradeName.CompareTo(data.m_UpgradeName) == 0).Save(data);
    }

    public ExperienceData FindExperienceType(StatsManager.GameCurrencyType type)
    {
        return m_ExperienceTypes.Find(searched => searched.m_CurrencyType == type);
    }

    public CurrencyData FindGameCurrency(StatsManager.GameCurrencyType type)
    {
        return m_GameCurrencies.Find(searched => searched.m_CurrencyType == type);
    }

    public UpgradeData FindUpgrade(UpgradeData upgr)
    {
        return m_AvailableUpgrades.Find(searched => searched.m_UpgradeName.CompareTo(upgr.m_UpgradeName) == 0 &&
        searched.m_BoughtWith == upgr.m_BoughtWith);
    }
}
