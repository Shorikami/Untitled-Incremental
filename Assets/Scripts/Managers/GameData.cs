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
    
    public ExperienceData FindExperienceType(StatsManager.GameCurrencyType type)
    {
        foreach (ExperienceData e in m_ExperienceTypes)
        {
            if (e.m_ExpType == type)
                return e;
        }
        return default(ExperienceData);
    }

    public CurrencyData FindGameCurrency(StatsManager.GameCurrencyType type)
    {
        foreach (CurrencyData gc in m_GameCurrencies)
        {
            if (gc.m_CurrencyType == type)
                return gc;
        }
        return default(CurrencyData);
    }

    public UpgradeData FindUpgrade(string name)
    {
        foreach (UpgradeData u in m_AvailableUpgrades)
        {
            if (u.m_UpgradeName.CompareTo(name) == 0)
                return u;
        }
        return default(UpgradeData);
    }


}
