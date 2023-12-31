using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class StatsManager : MonoBehaviour
{
    // THESE WILL BE STORED IN AN INSTANCED CLASS THAT IS
    // ATTACHED TO THE MANAGER (m_LoadedDataNodes, which is
    // taken from the data manager (they both know what currencies
    // exist throughout the game's lifetime))!!!!!
    public enum GameCurrencyType
    {
        None = 0,

        // Collectible currencies
        Wheat = 1,
        Bread,

        // Non-collectible currencies
        Perks,

        // Level-specific currencies
        Experience,

        // Global currencies
        Decryptions
    };

    public enum NonCurrencyUpgrades
    { 
        Invalid = -1,
        PermanentUpgrades,
        GrowthRate,
        MoveSpeed,
        PickupRange
    }

    public static StatsManager m_Instance { get; private set; }

    public List<GameObject> m_LoadedDataNodes = new List<GameObject>();
    //public Dictionary<GameCurrencyType, float> m_Multipliers = new Dictionary<GameCurrencyType, float>();
    //public SerializableDict<NonCurrencyUpgrades, float> m_NonCurrMultipliers = new SerializableDict<NonCurrencyUpgrades, float>();

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

    public float FindAllUpgradeMultipliers(GameCurrencyType gct, bool checkBoughtWith = true)
    {
        List<Upgrade> validUpgrades = FindUpgrades(gct, checkBoughtWith);
        return validUpgrades.ConvertAll(x => x.m_UpgradeData.m_UpgradeBonuses.m_CurrentBonus).Aggregate((a, b) => a * b);
    }

    public float FindAllUpgradeMultipliers(NonCurrencyUpgrades ncu)
    {
        List<Upgrade> validUpgrades = FindUpgrades(ncu);
        return validUpgrades.ConvertAll(x => x.m_UpgradeData.m_UpgradeBonuses.m_CurrentBonus).Aggregate((a, b) => a * b);
    }

    public void RefreshPlayer()
    {
        m_Player =  GameObject.FindGameObjectWithTag("Player");
    }

    // todo: binary search
    public GameObject FindStatContainer(GameCurrencyType gct, Collectable.CollectableType ct)
    {
        GameObject res = null;

        var cont = m_LoadedDataNodes.FindAll(s => s.GetComponent<GameCurrency>() != null);
        res = cont.Find(s => s.GetComponent<GameCurrency>().m_Currency.m_CurrencyType == gct &&
                            s.GetComponent<GameCurrency>().m_Currency.m_CollectableType == ct);

        // early return if container was already found (it was of type GameCurrency)
        if (res != null)
            return res;

        var cont2 = m_LoadedDataNodes.FindAll(s => s.GetComponent<Experience>() != null);
        res = cont2.Find(s => s.GetComponent<Experience>().m_ExpData.m_CurrencyType == gct &&
                            s.GetComponent<Experience>().m_ExpData.m_CollectableType == ct);

        // res at this point will either be of type Experience or null
        return res;
    }

    // todo: binary search
    public GameObject FindStatContainer(GameCurrencyType gct)
    {
        GameObject res = null;

        var cont = m_LoadedDataNodes.FindAll(s => s.GetComponent<GameCurrency>() != null);
        res = cont.Find(s => s.GetComponent<GameCurrency>().m_Currency.m_CurrencyType == gct);

        // early return if container was already found (it was of type GameCurrency)
        if (res != null)
            return res;

        var cont2 = m_LoadedDataNodes.FindAll(s => s.GetComponent<Experience>() != null);
        res = cont2.Find(s => s.GetComponent<Experience>().m_ExpData.m_CurrencyType == gct);

        // res at this point will either be of type Experience or null
        return res;
    }

    // there shouldn't be more than 1 of a specific currency based on collectable type
    public GameCurrency FindGameCurrency(Collectable.CollectableType ct, GameCurrencyType gct = GameCurrencyType.None)
    {
        GameCurrency res = null;

        var cont = m_LoadedDataNodes.FindAll(s => s.GetComponent<GameCurrency>() != null);

        if (gct == GameCurrencyType.None)
            res = cont.Find(s => 
            s.GetComponent<GameCurrency>().m_Currency.m_CollectableType == ct)
                .GetComponent<GameCurrency>();

        else
            res = cont.Find(s =>
            s.GetComponent<GameCurrency>().m_Currency.m_CollectableType == ct && 
            s.GetComponent<GameCurrency>().m_Currency.m_CurrencyType == gct)
                .GetComponent<GameCurrency>();

        return res;
    }

    public GameCurrency FindGameCurrency(GameCurrencyType gct)
    {
        GameCurrency res = null;

        var cont = m_LoadedDataNodes.FindAll(s => s.GetComponent<GameCurrency>() != null);

        if (cont == null)
            return res;

        GameObject searched = cont.Find(s =>
        s.GetComponent<GameCurrency>().m_Currency.m_CurrencyType == gct);

        if (searched == null)
            return res;

        res = searched.GetComponent<GameCurrency>();
        return res;
    }

    public Experience FindExperience(Collectable.CollectableType ct)
    {
        Experience res = null;

        var cont = m_LoadedDataNodes.FindAll(s => s.GetComponent<Experience>() != null);
        res = cont.Find(s =>
        s.GetComponent<Experience>().m_ExpData.m_CollectableType == ct).GetComponent<Experience>();

        return res;
    }

    public Experience FindExperience(GameCurrencyType gct)
    {
        Experience res = null;

        var cont = m_LoadedDataNodes.FindAll(s => s.GetComponent<Experience>() != null);
        res = cont.Find(s =>
        s.GetComponent<Experience>().m_ExpData.m_CurrencyType == gct).GetComponent<Experience>();

        return res;
    }

    public GameObject FindUpgrade(Collectable.CollectableType type, NonCurrencyUpgrades ncu)
    {
        var cont = m_LoadedDataNodes.FindAll(searched => searched.GetComponent<Upgrade>() != null);
        return cont.Find(s => s.GetComponent<Upgrade>().m_UpgradeData.m_CollectableType == type &&
                                s.GetComponent<Upgrade>().m_UpgradeData.m_NonCurrType == ncu);
    }

    public List<GameObject> FindUpgrades(Collectable.CollectableType type)
    {
        List<GameObject> res = new List<GameObject>();
        var cont = m_LoadedDataNodes.FindAll(searched => searched.GetComponent<Upgrade>() != null);
        res = cont.FindAll(searched => searched.GetComponent<Upgrade>().m_UpgradeData.m_CollectableType == type);
        return res;
    }

    public List<Upgrade> FindUpgrades(GameCurrencyType gct, bool checkBoughtWith = true)
    {
        List<Upgrade> res = new List<Upgrade>();
        var cont = m_LoadedDataNodes.FindAll(searched => searched.GetComponent<Upgrade>() != null);

        List<GameObject> matching = null;

        if (checkBoughtWith)
            matching = cont.FindAll(s => s.GetComponent<Upgrade>().m_UpgradeData.m_BoughtWith == gct);

        else
            matching = cont.FindAll(s => s.GetComponent<Upgrade>().m_UpgradeData.m_CurrencyType == gct);

        foreach (GameObject gO in matching)
            res.Add(gO.GetComponent<Upgrade>());

        return res;
    }

    public List<Upgrade> FindUpgrades(NonCurrencyUpgrades ncu)
    {
        List<Upgrade> res = new List<Upgrade>();
        var cont = m_LoadedDataNodes.FindAll(searched => searched.GetComponent<Upgrade>() != null);
        var matching = cont.FindAll(s => s.GetComponent<Upgrade>().m_UpgradeData.m_NonCurrType == ncu);

        foreach (GameObject gO in matching)
            res.Add(gO.GetComponent<Upgrade>());

        return res;
    }

    // reset upgrades based on what they're bought with
    public void ResetUpgrades(GameCurrencyType gct)
    {
        var cont = m_LoadedDataNodes.FindAll(searched => searched.GetComponent<Upgrade>() != null);
        List<GameObject> matching = cont.FindAll(s => s.GetComponent<Upgrade>().m_UpgradeData.m_BoughtWith == gct);

        if (matching == null)
            return;

        foreach (GameObject go in matching)
        {
            go.GetComponent<Upgrade>().ResetData();
        }
    }
}

public abstract class Data
{
    // this might overflow but this is mostly for debugging
    public double m_TotalValue;

    // How many (in current rank) e.g. 0-9
    public int m_Value;

    // Current tenths position e.g. 10s, 100s, 1000s...
    [Min(0)]
    public int m_Rank;

    // What this data container affects
    public StatsManager.GameCurrencyType m_CurrencyType;

    // What this collectible type is (there will eventually be more than 1
    // type of collectible)
    public Collectable.CollectableType m_CollectableType;

    public virtual void UpdateValue(double toAdd)
    {
        m_TotalValue += toAdd;
    }
}