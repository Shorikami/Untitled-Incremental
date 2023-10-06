using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameCurrency : MonoBehaviour, ISavableData
{
    // this might overflow but this is mostly for debugging
    public int m_TotalCount;

    // How many (in current rank) e.g. 0-9
    public int m_Count;

    // Current tenths position e.g. 10s, 100s, 1000s...
    [Min(0)]
    public int m_Rank;

    public StatsManager.GameCurrencyType m_CurrencyType;

    public void SaveData(ref GameData data)
    {
        throw new System.NotImplementedException();
    }

    public void LoadData(GameData data)
    {
        throw new System.NotImplementedException();
    }

    public void UpdateCurrency(int toAdd)
    {
        m_TotalCount += toAdd;
    }
}
