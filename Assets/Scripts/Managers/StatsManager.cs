using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StatsManager : MonoBehaviour, ISavableData
{
    public enum GameCurrencyType
    {
        None = 0,
        Coins = 1,
        Credits,

        Experience,
        Tier
    };

    public enum NonCurrencyUpgrades
    { 
        Invalid = -1,
        DefaultGrowthRate,

        MoveSpeed,
        PickupRange
    }

    public static StatsManager m_Instance { get; private set; }

    public List<GameObject> m_LoadedDataNodes = new List<GameObject>();
    public Dictionary<GameCurrencyType, float> m_Multipliers = new Dictionary<GameCurrencyType, float>();
    public SerializableDict<NonCurrencyUpgrades, float> m_NonCurrMultipliers = new SerializableDict<NonCurrencyUpgrades, float>();

    private GameObject m_Player;

    public GameObject Player
    { 
        get { return m_Player; }
        private set { m_Player = value; }
    }

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

    public void SaveData(ref GameData data)
    {
        foreach (var keyPair in m_NonCurrMultipliers)
        {
            if (!data.m_NonCurrMultipliers.ContainsKey(keyPair.Key))
                data.m_NonCurrMultipliers.Add(keyPair.Key, keyPair.Value);

            else
                data.m_NonCurrMultipliers[keyPair.Key] = keyPair.Value;
        }
    }

    public void LoadData(GameData data)
    {
        foreach (var keyPair in data.m_NonCurrMultipliers)
        {
            if (!m_NonCurrMultipliers.ContainsKey(keyPair.Key))
                m_NonCurrMultipliers.Add(keyPair.Key, keyPair.Value);

            else
                m_NonCurrMultipliers[keyPair.Key] = keyPair.Value;
        }
    }

    public void UpdateMultiplier(UpgradeData data, float val)
    {
        if (data.m_CurrencyType != GameCurrencyType.None)
            m_Multipliers[data.m_CurrencyType] = val;

        else
            m_NonCurrMultipliers[data.m_NonCurrType] = val;
    }

    public void RefreshPlayer()
    {
        m_Player =  GameObject.FindGameObjectWithTag("Player");
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
