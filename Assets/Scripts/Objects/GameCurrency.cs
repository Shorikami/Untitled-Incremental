using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameCurrency : MonoBehaviour, ISavableData
{
    public CurrencyData m_Currency;

    public void SaveData(ref GameData data)
    {
        CurrencyData toSave = data.FindGameCurrency(m_Currency.m_CurrencyType);
        toSave.m_Count = m_Currency.m_Count;
        toSave.m_Rank = m_Currency.m_Rank;
        toSave.m_TotalCount = m_Currency.m_TotalCount;
    }

    public void LoadData(GameData data)
    {
        CurrencyData toLoad = data.FindGameCurrency(m_Currency.m_CurrencyType);
        m_Currency.m_Count = toLoad.m_Count;
        m_Currency.m_Rank = toLoad.m_Rank;
        m_Currency.m_TotalCount = toLoad.m_TotalCount;
    }

    public void UpdateCurrency(int toAdd)
    {
        m_Currency.m_TotalCount += toAdd;
    }
}

[System.Serializable]
public class CurrencyData : Data
{
    // this might overflow but this is mostly for debugging
    public int m_TotalCount;

    // How many (in current rank) e.g. 0-9
    public int m_Count;

    // Current tenths position e.g. 10s, 100s, 1000s...
    [Min(0)]
    public int m_Rank;

    public CurrencyData()
    {
        m_TotalCount = 0;
        m_Count = 0;
        m_Rank = 0;
        m_CurrencyType = StatsManager.GameCurrencyType.None;
        m_CollectableType = Collectable.CollectableType.None;
    }

    public void Save(CurrencyData other)
    {
        m_TotalCount = other.m_TotalCount;
        m_Count = other.m_Count;
        m_Rank = other.m_Rank;
        m_CurrencyType = other.m_CurrencyType;
    }
}
