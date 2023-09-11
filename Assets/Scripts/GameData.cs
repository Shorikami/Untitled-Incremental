using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class GameData
{
    public List<Currency> m_Currencies;

    public GameData()
    {
        m_Currencies = new List<Currency>();
    }
}
