using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DataManager : MonoBehaviour
{
    [Header("File Storage Configuration\n====================")]
    [SerializeField] private string m_FileName;
    public static DataManager m_Instance { get; private set; }

    [SerializeField] private GameObject m_EmptyCurrencyPrefab;

    [SerializeField] private GameData m_GameData;
    private List<ISavableData> m_SavedData;
    private FileHandler m_FileDataHandler;

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

        m_FileDataHandler = new FileHandler(Application.persistentDataPath, m_FileName);
        //m_SavedData = FindSavedData();
        Load();
    }

    private void Start()
    {
    }

    public void NewCurrency(StatsManager.GameCurrencyType currtype, Collectable.CollectableType collType)
    {
        GameObject gO = Instantiate(m_EmptyCurrencyPrefab, StatsManager.m_Instance.gameObject.transform);
        gO.AddComponent<GameCurrency>();
        GameCurrency gc = gO.GetComponent<GameCurrency>();

        gc.m_Currency = new CurrencyData();
        gc.m_Currency.m_CurrencyType = currtype;
        gc.m_Currency.m_CollectableType = collType;
        gc.m_Currency.m_TotalValue = 0;

        m_GameData.m_GameCurrencies.Add(gc.m_Currency);
        StatsManager.m_Instance.m_LoadedDataNodes.Add(gO);
    }

    public void NewExperience(StatsManager.GameCurrencyType currtype, Collectable.CollectableType colltype,
        StatsManager.GameCurrencyType perktype)
    {
        GameObject gO = Instantiate(m_EmptyCurrencyPrefab, StatsManager.m_Instance.gameObject.transform);
        gO.AddComponent<Experience>();
        Experience ex = gO.GetComponent<Experience>();

        ex.m_ExpData = new ExperienceData();
        ex.m_ExpData.m_CurrencyType = currtype;
        ex.m_ExpData.m_CollectableType = colltype;
        ex.m_ExpData.m_PerkType = perktype;
        ex.m_ExpData.m_TotalValue = 0;

        m_GameData.m_ExperienceTypes.Add(ex.m_ExpData);
        StatsManager.m_Instance.m_LoadedDataNodes.Add(gO);
    }

    public void NewUpgrade(string name, string desc, int maxLevel, double baseCost, bool staticCost, int bonusReqLevels,
        float levelBonusMulti, float bonusMulti, StatsManager.GameCurrencyType currtype, StatsManager.GameCurrencyType boughtWith,
        StatsManager.NonCurrencyUpgrades nonCurrType, Collectable.CollectableType colltype)
    {
        GameObject gO = Instantiate(m_EmptyCurrencyPrefab, StatsManager.m_Instance.gameObject.transform);
        gO.AddComponent<Upgrade>();
        Upgrade up = gO.GetComponent<Upgrade>();

        up.m_UpgradeData = new UpgradeData();
        up.m_UpgradeData.m_UpgradeBonuses = new UpgradeData.Bonus();
        up.m_UpgradeData.m_UpgradeName = name;
        up.m_UpgradeData.m_Description = desc;
        up.m_UpgradeData.m_CurrLevel = 0;
        up.m_UpgradeData.m_MaxLevel = maxLevel;
        up.m_UpgradeData.m_BaseCost = baseCost;
        up.m_UpgradeData.m_StaticCost = staticCost;
        up.m_UpgradeData.m_UpgradeBonuses.m_RequiredLevels = bonusReqLevels;
        up.m_UpgradeData.m_UpgradeBonuses.m_LevelBonus = levelBonusMulti;
        up.m_UpgradeData.m_UpgradeBonuses.m_BonusMultiplier = bonusMulti;
        up.m_UpgradeData.m_CurrencyType = currtype;
        up.m_UpgradeData.m_BoughtWith = boughtWith;
        up.m_UpgradeData.m_NonCurrType = nonCurrType;
        up.m_UpgradeData.m_CollectableType = colltype;

        m_GameData.m_AvailableUpgrades.Add(up.m_UpgradeData);
        StatsManager.m_Instance.m_LoadedDataNodes.Add(gO);
    }

    public void NewGame()
    {
        // TODO: When starting completely fresh, the following will always be available from the start:
        // - initial currency type
        // - initial exp type
        // - 3 upgrades, which are the following:
        //  - increase initial currency value
        //  - increase initial currency spawn rate
        //  - increase initial exp value
        // update this as more initial values are added
        m_GameData = new GameData();

        NewCurrency(StatsManager.GameCurrencyType.Coins, Collectable.CollectableType.Default);
        NewCurrency(StatsManager.GameCurrencyType.Perks, Collectable.CollectableType.Default);

        NewExperience(StatsManager.GameCurrencyType.Experience, Collectable.CollectableType.Default, StatsManager.GameCurrencyType.Perks);

        NewUpgrade("Coins Value", "Increases value of coins by +25% per level. Coin value is doubled every 25 levels.",
            500, 5, false, 25, 2.0f, 0.25f, StatsManager.GameCurrencyType.Coins, StatsManager.GameCurrencyType.Coins,
            StatsManager.NonCurrencyUpgrades.Invalid, Collectable.CollectableType.Default);
        NewUpgrade("Growth", "Increases spawn rate by +20% per level.",
            100, 10, false, 0, 0.0f, 0.2f, StatsManager.GameCurrencyType.Coins, StatsManager.GameCurrencyType.Coins,
            StatsManager.NonCurrencyUpgrades.GrowthRate, Collectable.CollectableType.Default);
        NewUpgrade("XP Value", "Increases XP value by +25% per level. XP Value is doubled every 25 levels.",
            500, 7, false, 25, 2.0f, 0.25f, StatsManager.GameCurrencyType.Experience, StatsManager.GameCurrencyType.Coins,
            StatsManager.NonCurrencyUpgrades.Invalid, Collectable.CollectableType.Default);
        NewUpgrade("Pickup Range+", "Increase pickup range by 10% per level.",
            20, 10000, false, 0, 0.0f, 0.1f, StatsManager.GameCurrencyType.Coins, StatsManager.GameCurrencyType.Coins,
            StatsManager.NonCurrencyUpgrades.PickupRange, Collectable.CollectableType.Default);
        NewUpgrade("Speed+", "Increase movement speed by 10% per level.",
            7, 1500, false, 0, 0.0f, 0.1f, StatsManager.GameCurrencyType.Coins, StatsManager.GameCurrencyType.Coins,
            StatsManager.NonCurrencyUpgrades.MoveSpeed, Collectable.CollectableType.Default);

        NewUpgrade("Perks Coins Value", "Increases value of coins by +10% per level.",
            100, 1, true, 0, 0.0f, 0.1f, StatsManager.GameCurrencyType.Coins, StatsManager.GameCurrencyType.Perks,
            StatsManager.NonCurrencyUpgrades.Invalid, Collectable.CollectableType.Default);
        NewUpgrade("Perks EXP Value", "Increases value of EXP by +10% per level.",
            100, 1, true, 0, 0.0f, 0.1f, StatsManager.GameCurrencyType.Experience, StatsManager.GameCurrencyType.Perks,
            StatsManager.NonCurrencyUpgrades.Invalid, Collectable.CollectableType.Default);
        NewUpgrade("Perks Speed+", "Increase movement speed by 10% per level.",
            3, 1, true, 0, 0.0f, 0.1f, StatsManager.GameCurrencyType.None, StatsManager.GameCurrencyType.Perks,
            StatsManager.NonCurrencyUpgrades.MoveSpeed, Collectable.CollectableType.Default);
    }

    public void Save()
    {
        foreach (ISavableData d in m_SavedData)
        {
            d.SaveData(ref m_GameData);
        }

        m_FileDataHandler.Save(m_GameData);
    }

    public void Load()
    {
        bool initialLoad = false;
        m_GameData = m_FileDataHandler.Load();

        if (m_GameData == null)
        {
            initialLoad = true;
            Debug.Log("No data found. Initializing new data..");
            NewGame();
        }

        if (m_SavedData == null || m_SavedData.Count <= 0)
        {
            if (!initialLoad)
                LoadGlobalStatContainers();

            m_SavedData = FindSavedData();
        }

        foreach (ISavableData d in m_SavedData)
        {
            d.LoadData(m_GameData);
        }

        // after loading, set the player once
        StatsManager.m_Instance.RefreshPlayer();
    }

    private List<ISavableData> FindSavedData()
    {
        IEnumerable<ISavableData> savedData = FindObjectsOfType<MonoBehaviour>()
                                                    .OfType<ISavableData>();

        return new List<ISavableData>(savedData);
    }

    private void OnApplicationQuit()
    {
        Save();
    }

    private void LoadGlobalStatContainers()
    {
        GameObject gO;
        foreach (CurrencyData cd in m_GameData.m_GameCurrencies)
        {
            gO = Instantiate(m_EmptyCurrencyPrefab, StatsManager.m_Instance.gameObject.transform);
            gO.AddComponent<GameCurrency>();
            gO.GetComponent<GameCurrency>().m_Currency = new CurrencyData();
            gO.GetComponent<GameCurrency>().m_Currency = cd;
            StatsManager.m_Instance.m_LoadedDataNodes.Add(gO);
        }

        foreach (ExperienceData ed in m_GameData.m_ExperienceTypes)
        {
            gO = Instantiate(m_EmptyCurrencyPrefab, StatsManager.m_Instance.gameObject.transform);
            gO.AddComponent<Experience>();
            gO.GetComponent<Experience>().m_ExpData = new ExperienceData();
            gO.GetComponent<Experience>().m_ExpData = ed;
            StatsManager.m_Instance.m_LoadedDataNodes.Add(gO);
        }

        foreach (UpgradeData ud in m_GameData.m_AvailableUpgrades)
        {
            gO = Instantiate(m_EmptyCurrencyPrefab, StatsManager.m_Instance.gameObject.transform);
            gO.AddComponent<Upgrade>();
            Upgrade up = gO.GetComponent<Upgrade>();

            up.m_UpgradeData = new UpgradeData();
            up.m_UpgradeData.m_UpgradeBonuses = new UpgradeData.Bonus();
            up.m_UpgradeData = ud;
            StatsManager.m_Instance.m_LoadedDataNodes.Add(gO);
        }
    }
}
