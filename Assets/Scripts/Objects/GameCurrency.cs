using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameCurrency : MonoBehaviour, ISavableData
{
    public CurrencyData m_Currency;

    public void SaveData(ref GameData data)
    {
        CurrencyData toSave = data.FindGameCurrency(m_Currency.m_CurrencyType);
        toSave.m_Value = m_Currency.m_Value;
        toSave.m_Rank = m_Currency.m_Rank;
        toSave.m_TotalValue = m_Currency.m_TotalValue;
    }

    public void LoadData(GameData data)
    {
        CurrencyData toLoad = data.FindGameCurrency(m_Currency.m_CurrencyType);
        m_Currency.m_Value = toLoad.m_Value;
        m_Currency.m_Rank = toLoad.m_Rank;
        m_Currency.m_TotalValue = toLoad.m_TotalValue;
    }

    public void UpdateCurrency(int val)
    {
        m_Currency.UpdateValue(val);
    }
}

[System.Serializable]
public class CurrencyData : Data
{
    public CurrencyData()
    {
        m_TotalValue = 0;
        m_Value = 0;
        m_Rank = 0;
        m_CurrencyType = StatsManager.GameCurrencyType.None;
        m_CollectableType = Collectable.CollectableType.None;
    }

    public void Save(CurrencyData other)
    {
        m_TotalValue = other.m_TotalValue;
        m_Value = other.m_Value;
        m_Rank = other.m_Rank;
        m_CurrencyType = other.m_CurrencyType;
    }

    public override void UpdateValue(double toAdd)
    {
        base.UpdateValue(toAdd);
    }
}
