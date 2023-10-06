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

    // binary search
    public GameObject FindContainer<T>(GameCurrencyType gct)
    {
        foreach (GameObject gO in m_LoadedDataNodes)
        {
            T res = gO.GetComponent<T>();

            if (res != null)
            {
                switch (res)
                {
                    case GameCurrency gc:
                        if (gc.m_Currency.m_CurrencyType == gct)
                            return gO;
                        break;

                    case Experience ex:
                        if (ex.m_ExpData.m_ExpType == gct)
                            return gO;
                        break;
                }
            }
        }

        return null;
    }
}
