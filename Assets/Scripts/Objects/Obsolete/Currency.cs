using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Currency
{
    public enum CurrencyType
    { 
        NONE,
        Dollars,
        Credits,
        Letters,
        Threads,
        Chroma,
        Paint
    }

    // this might overflow but this is mostly for debugging
    public int m_TotalCount;

    // How many (in current rank) e.g. 0-9
    public int m_Count;

    // Current tenths position e.g. 10s, 100s, 1000s...
    [Min(0)]
    public int m_Rank;

    public CurrencyType m_CurrencyType;

    public Currency(CurrencyType c = CurrencyType.NONE)
    {
        m_Count = 0;
        m_TotalCount = 0;
        m_Rank = 0;
        m_CurrencyType = c;
    }

    public Currency(Currency other)
    {
        m_Count = other.m_Count;
        m_TotalCount = other.m_TotalCount;
        m_Rank = other.m_Rank;
        m_CurrencyType = other.m_CurrencyType;
    }

    // kind of gross but c# doesn't allow operator= overload
    public void ExplicitCopy(Currency other)
    {
        m_Count = other.m_Count;
        m_TotalCount = other.m_TotalCount;
        m_Rank = other.m_Rank;
        m_CurrencyType = other.m_CurrencyType;
    }

    public void UpdateCurrency(int toAdd)
    {
        m_TotalCount += toAdd;
    }
}
