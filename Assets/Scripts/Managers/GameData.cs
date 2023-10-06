using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class GameData
{
    public List<GameCurrency> m_GameCurrencies;
    public List<Experience> m_ExperienceTypes;
    public List<Upgrade> m_AvailableUpgrades;

    // Constructor for when starting new game
    public GameData()
    {
        m_GameCurrencies = new List<GameCurrency>();
        m_ExperienceTypes = new List<Experience>();
        m_AvailableUpgrades = new List<Upgrade>();
    }

    // ----
    // TODO: Binary search? they'll be small containers so it might not be necessary but whatever
    // ----
    
    public Experience FindExperienceType(StatsManager.GameCurrencyType type)
    {
        foreach (Experience e in m_ExperienceTypes)
        {
            if (e.m_ExpType == type)
                return e;
        }
        return null;
    }

    public GameCurrency FindGameCurrency(StatsManager.GameCurrencyType type)
    {
        foreach (GameCurrency gc in m_GameCurrencies)
        {
            if (gc.m_CurrencyType == type)
                return gc;
        }
        return null;
    }

    public Upgrade FindUpgrade(string name)
    {
        foreach (Upgrade u in m_AvailableUpgrades)
        {
            if (u.m_UpgradeName.CompareTo(name) == 0)
                return u;
        }
        return null;
    }


}
