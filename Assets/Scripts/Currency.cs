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

    // How many (in current rank) e.g. 0-9
    private int m_Count = 0;

    // Current tenths position e.g. 10s, 100s, 1000s...
    [Min(1)]
    private int m_Rank;

    public CurrencyType m_CurrencyType;
}
