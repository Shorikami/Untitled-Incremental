using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Currency : MonoBehaviour
{
    public enum CurrencyType
    { 
        Dollars,
        Credits,
        Letters,
        Echoes,
        Chroma,
        Paint
    }

    // this might overflow but this is mostly for debugging
    public int m_TotalCount = 0;

    // How many (in current rank) e.g. 0-9
    public int m_Count = 0;

    // Current tenths position e.g. 10s, 100s, 1000s...
    [Min(1)]
    public int m_Rank;

    public CurrencyType m_CurrencyType;
}
