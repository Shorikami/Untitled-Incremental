using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StatsManager : MonoBehaviour
{
    public enum GameCurrencyType
    {
        None = 0,
        Coins = 1,
        Credits,

        Experience,
        Tier
    };

    public static StatsManager m_Instance { get; private set; }

    public List<GameObject> m_LoadedDataNodes = new List<GameObject>();
    public Dictionary<GameCurrencyType, float> m_Multipliers = new Dictionary<GameCurrencyType, float>();

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

    public void UpdateMultiplier(GameCurrencyType gct, float val)
    {
        if (gct == GameCurrencyType.None)
            return;

        m_Multipliers[gct] = val;
    }

    // todo: binary search
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

    public List<GameObject> FindUpgrades(Collectable.CollectableType type)
    {
        List<GameObject> res = new List<GameObject>();
        var cont = m_LoadedDataNodes.FindAll(searched => searched.GetComponent<Upgrade>() != null);
        res = cont.FindAll(searched => searched.GetComponent<Upgrade>().m_UpgradeData.m_CollectableType == type);
        return res;
    }
}
