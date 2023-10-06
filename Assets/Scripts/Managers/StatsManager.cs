using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StatsManager : MonoBehaviour
{
    public static StatsManager m_Instance { get; private set; }

    public List<GameObject> m_LoadedDataNodes = new List<GameObject>();

    public enum GameCurrencyType
    { 
        None = 0,
        Coins = 1,
        Credits,

        Experience,

        Tier
    };

    // Singleton
    private void Awake()
    {
        if (m_Instance && m_Instance != this)
        {
            Destroy(this);
            return;
        }
        m_Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }
}
