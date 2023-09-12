using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class GameData
{
    public List<Currency> m_Currencies;
    public int m_Experience;

    // Constructor for when starting new game
    public GameData()
    {
        m_Currencies = new List<Currency>();
        m_Experience = 0;
    }
}
