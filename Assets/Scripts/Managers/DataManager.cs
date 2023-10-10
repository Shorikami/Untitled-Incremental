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

        GameObject gO = Instantiate(m_EmptyCurrencyPrefab, StatsManager.m_Instance.gameObject.transform);
        gO.AddComponent<GameCurrency>();
        GameCurrency gc = gO.GetComponent<GameCurrency>();

        gc.m_Currency = new CurrencyData();
        gc.m_Currency.m_CurrencyType = StatsManager.GameCurrencyType.Coins;
        gc.m_Currency.m_CollectableType = Collectable.CollectableType.Default;
        gc.m_Currency.m_Value = 0;

        m_GameData.m_GameCurrencies.Add(gc.m_Currency);
        StatsManager.m_Instance.m_LoadedDataNodes.Add(gO);

        gO = Instantiate(m_EmptyCurrencyPrefab, StatsManager.m_Instance.gameObject.transform);
        gO.AddComponent<GameCurrency>();
        gc = gO.GetComponent<GameCurrency>();

        gc.m_Currency = new CurrencyData();
        gc.m_Currency.m_CurrencyType = StatsManager.GameCurrencyType.Perks;
        gc.m_Currency.m_CollectableType = Collectable.CollectableType.Default;
        gc.m_Currency.m_Value = 0;

        m_GameData.m_GameCurrencies.Add(gc.m_Currency);
        StatsManager.m_Instance.m_LoadedDataNodes.Add(gO);

        gO = Instantiate(m_EmptyCurrencyPrefab, StatsManager.m_Instance.gameObject.transform);
        gO.AddComponent<Experience>();
        Experience ex = gO.GetComponent<Experience>();

        ex.m_ExpData = new ExperienceData();
        ex.m_ExpData.m_CurrencyType = StatsManager.GameCurrencyType.Experience;
        ex.m_ExpData.m_CollectableType = Collectable.CollectableType.Default;
        ex.m_ExpData.m_PerkType = StatsManager.GameCurrencyType.Perks;
        ex.m_ExpData.m_TotalValue = 0;

        m_GameData.m_ExperienceTypes.Add(ex.m_ExpData);
        StatsManager.m_Instance.m_LoadedDataNodes.Add(gO);

        gO = Instantiate(m_EmptyCurrencyPrefab, StatsManager.m_Instance.gameObject.transform);
        gO.AddComponent<Upgrade>();
        Upgrade up = gO.GetComponent<Upgrade>();

        up.m_UpgradeData = new UpgradeData();
        up.m_UpgradeData.m_UpgradeBonuses = new UpgradeData.Bonus();
        up.m_UpgradeData.m_UpgradeName = "Coins Value";
        up.m_UpgradeData.m_Description = "Increases value of coins by +25% per level. Coin value is doubled every 25 levels.";
        up.m_UpgradeData.m_CurrLevel = 0;
        up.m_UpgradeData.m_MaxLevel = 500;
        up.m_UpgradeData.m_BaseCost = 5;
        up.m_UpgradeData.m_StaticCost = false;
        up.m_UpgradeData.m_UpgradeBonuses.m_RequiredLevels = 25;
        up.m_UpgradeData.m_UpgradeBonuses.m_LevelBonus = 2.0f;
        up.m_UpgradeData.m_UpgradeBonuses.m_BonusMultiplier = 0.25f;
        up.m_UpgradeData.m_CurrencyType = StatsManager.GameCurrencyType.Coins;
        up.m_UpgradeData.m_BoughtWith = StatsManager.GameCurrencyType.Coins;
        up.m_UpgradeData.m_NonCurrType = StatsManager.NonCurrencyUpgrades.Invalid;
        up.m_UpgradeData.m_CollectableType = Collectable.CollectableType.Default;

        m_GameData.m_AvailableUpgrades.Add(up.m_UpgradeData);
        StatsManager.m_Instance.m_LoadedDataNodes.Add(gO);

        gO = Instantiate(m_EmptyCurrencyPrefab, StatsManager.m_Instance.gameObject.transform);
        gO.AddComponent<Upgrade>();
        up = gO.GetComponent<Upgrade>();

        up.m_UpgradeData = new UpgradeData();
        up.m_UpgradeData.m_UpgradeBonuses = new UpgradeData.Bonus();
        up.m_UpgradeData.m_UpgradeName = "Growth";
        up.m_UpgradeData.m_Description = "Increases spawn rate by +20% per level.";
        up.m_UpgradeData.m_CurrLevel = 0;
        up.m_UpgradeData.m_MaxLevel = 10;
        up.m_UpgradeData.m_BaseCost = 10;
        up.m_UpgradeData.m_StaticCost = false;
        up.m_UpgradeData.m_UpgradeBonuses.m_BonusMultiplier = 0.20f;
        up.m_UpgradeData.m_CurrencyType = StatsManager.GameCurrencyType.Coins;
        up.m_UpgradeData.m_BoughtWith = StatsManager.GameCurrencyType.Coins;
        up.m_UpgradeData.m_NonCurrType = StatsManager.NonCurrencyUpgrades.GrowthRate;
        up.m_UpgradeData.m_CollectableType = Collectable.CollectableType.Default;
        //StatsManager.m_Instance.UpdateMultiplier(up.m_UpgradeData, up.m_UpgradeData.m_UpgradeBonuses.m_CurrentBonus);

        m_GameData.m_AvailableUpgrades.Add(up.m_UpgradeData);
        StatsManager.m_Instance.m_LoadedDataNodes.Add(gO);

        gO = Instantiate(m_EmptyCurrencyPrefab, StatsManager.m_Instance.gameObject.transform);
        gO.AddComponent<Upgrade>();
        up = gO.GetComponent<Upgrade>();

        up.m_UpgradeData = new UpgradeData();
        up.m_UpgradeData.m_UpgradeBonuses = new UpgradeData.Bonus();
        up.m_UpgradeData.m_UpgradeName = "XP Value";
        up.m_UpgradeData.m_Description = "Increases XP value by +25% per level. XP Value is doubled every 25 levels.";
        up.m_UpgradeData.m_CurrLevel = 0;
        up.m_UpgradeData.m_MaxLevel = 500;
        up.m_UpgradeData.m_BaseCost = 7;
        up.m_UpgradeData.m_StaticCost = false;
        up.m_UpgradeData.m_UpgradeBonuses.m_RequiredLevels = 25;
        up.m_UpgradeData.m_UpgradeBonuses.m_LevelBonus = 2.0f;
        up.m_UpgradeData.m_UpgradeBonuses.m_BonusMultiplier = 0.25f;
        up.m_UpgradeData.m_CurrencyType = StatsManager.GameCurrencyType.Experience;
        up.m_UpgradeData.m_BoughtWith = StatsManager.GameCurrencyType.Coins;
        up.m_UpgradeData.m_NonCurrType = StatsManager.NonCurrencyUpgrades.Invalid;
        up.m_UpgradeData.m_CollectableType = Collectable.CollectableType.Default;

        m_GameData.m_AvailableUpgrades.Add(up.m_UpgradeData);
        StatsManager.m_Instance.m_LoadedDataNodes.Add(gO);

        gO = Instantiate(m_EmptyCurrencyPrefab, StatsManager.m_Instance.gameObject.transform);
        gO.AddComponent<Upgrade>();
        up = gO.GetComponent<Upgrade>();

        up.m_UpgradeData = new UpgradeData();
        up.m_UpgradeData.m_UpgradeBonuses = new UpgradeData.Bonus();
        up.m_UpgradeData.m_UpgradeName = "Pickup Range+";
        up.m_UpgradeData.m_Description = "Increase pickup range by 10% per level.";
        up.m_UpgradeData.m_CurrLevel = 0;
        up.m_UpgradeData.m_MaxLevel = 20;
        up.m_UpgradeData.m_BaseCost = 10000;
        up.m_UpgradeData.m_StaticCost = false;
        up.m_UpgradeData.m_UpgradeBonuses.m_BonusMultiplier = 0.1f;
        up.m_UpgradeData.m_CurrencyType = StatsManager.GameCurrencyType.Coins;
        up.m_UpgradeData.m_BoughtWith = StatsManager.GameCurrencyType.Coins;
        up.m_UpgradeData.m_NonCurrType = StatsManager.NonCurrencyUpgrades.PickupRange;
        up.m_UpgradeData.m_CollectableType = Collectable.CollectableType.Default;

        m_GameData.m_AvailableUpgrades.Add(up.m_UpgradeData);
        StatsManager.m_Instance.m_LoadedDataNodes.Add(gO);

        gO = Instantiate(m_EmptyCurrencyPrefab, StatsManager.m_Instance.gameObject.transform);
        gO.AddComponent<Upgrade>();
        up = gO.GetComponent<Upgrade>();

        up.m_UpgradeData = new UpgradeData();
        up.m_UpgradeData.m_UpgradeBonuses = new UpgradeData.Bonus();
        up.m_UpgradeData.m_UpgradeName = "Speed+";
        up.m_UpgradeData.m_Description = "Increase movement speed by 10% per level.";
        up.m_UpgradeData.m_CurrLevel = 0;
        up.m_UpgradeData.m_MaxLevel = 10;
        up.m_UpgradeData.m_BaseCost = 1500;
        up.m_UpgradeData.m_StaticCost = false;
        up.m_UpgradeData.m_UpgradeBonuses.m_BonusMultiplier = 0.1f;
        up.m_UpgradeData.m_CurrencyType = StatsManager.GameCurrencyType.Coins;
        up.m_UpgradeData.m_BoughtWith = StatsManager.GameCurrencyType.Coins;
        up.m_UpgradeData.m_NonCurrType = StatsManager.NonCurrencyUpgrades.MoveSpeed;
        up.m_UpgradeData.m_CollectableType = Collectable.CollectableType.Default;

        m_GameData.m_AvailableUpgrades.Add(up.m_UpgradeData);
        StatsManager.m_Instance.m_LoadedDataNodes.Add(gO);

        gO = Instantiate(m_EmptyCurrencyPrefab, StatsManager.m_Instance.gameObject.transform);
        gO.AddComponent<Upgrade>();
        up = gO.GetComponent<Upgrade>();

        up.m_UpgradeData = new UpgradeData();
        up.m_UpgradeData.m_UpgradeBonuses = new UpgradeData.Bonus();
        up.m_UpgradeData.m_UpgradeName = "Perks Coins Value";
        up.m_UpgradeData.m_Description = "Increases value of coins by +10% per level.";
        up.m_UpgradeData.m_CurrLevel = 0;
        up.m_UpgradeData.m_MaxLevel = 100;
        up.m_UpgradeData.m_BaseCost = 1;
        up.m_UpgradeData.m_StaticCost = true;
        up.m_UpgradeData.m_UpgradeBonuses.m_BonusMultiplier = 0.1f;
        up.m_UpgradeData.m_CurrencyType = StatsManager.GameCurrencyType.Coins;
        up.m_UpgradeData.m_BoughtWith = StatsManager.GameCurrencyType.Perks;
        up.m_UpgradeData.m_NonCurrType = StatsManager.NonCurrencyUpgrades.Invalid;
        up.m_UpgradeData.m_CollectableType = Collectable.CollectableType.Default;

        m_GameData.m_AvailableUpgrades.Add(up.m_UpgradeData);
        StatsManager.m_Instance.m_LoadedDataNodes.Add(gO);

        gO = Instantiate(m_EmptyCurrencyPrefab, StatsManager.m_Instance.gameObject.transform);
        gO.AddComponent<Upgrade>();
        up = gO.GetComponent<Upgrade>();

        up.m_UpgradeData = new UpgradeData();
        up.m_UpgradeData.m_UpgradeBonuses = new UpgradeData.Bonus();
        up.m_UpgradeData.m_UpgradeName = "Perks EXP Value";
        up.m_UpgradeData.m_Description = "Increases value of EXP by +10% per level.";
        up.m_UpgradeData.m_CurrLevel = 0;
        up.m_UpgradeData.m_MaxLevel = 100;
        up.m_UpgradeData.m_BaseCost = 1;
        up.m_UpgradeData.m_StaticCost = true;
        up.m_UpgradeData.m_UpgradeBonuses.m_BonusMultiplier = 0.1f;
        up.m_UpgradeData.m_CurrencyType = StatsManager.GameCurrencyType.Experience;
        up.m_UpgradeData.m_BoughtWith = StatsManager.GameCurrencyType.Perks;
        up.m_UpgradeData.m_NonCurrType = StatsManager.NonCurrencyUpgrades.MoveSpeed;
        up.m_UpgradeData.m_CollectableType = Collectable.CollectableType.Default;

        m_GameData.m_AvailableUpgrades.Add(up.m_UpgradeData);
        StatsManager.m_Instance.m_LoadedDataNodes.Add(gO);

        gO = Instantiate(m_EmptyCurrencyPrefab, StatsManager.m_Instance.gameObject.transform);
        gO.AddComponent<Upgrade>();
        up = gO.GetComponent<Upgrade>();

        up.m_UpgradeData = new UpgradeData();
        up.m_UpgradeData.m_UpgradeBonuses = new UpgradeData.Bonus();
        up.m_UpgradeData.m_UpgradeName = "Perks Speed+";
        up.m_UpgradeData.m_Description = "Increase movement speed by 10% per level.";
        up.m_UpgradeData.m_CurrLevel = 0;
        up.m_UpgradeData.m_MaxLevel = 10;
        up.m_UpgradeData.m_BaseCost = 1;
        up.m_UpgradeData.m_UpgradeBonuses.m_BonusMultiplier = 0.1f;
        up.m_UpgradeData.m_CurrencyType = StatsManager.GameCurrencyType.None;
        up.m_UpgradeData.m_BoughtWith = StatsManager.GameCurrencyType.Perks;
        up.m_UpgradeData.m_NonCurrType = StatsManager.NonCurrencyUpgrades.MoveSpeed;
        up.m_UpgradeData.m_CollectableType = Collectable.CollectableType.Default;

        m_GameData.m_AvailableUpgrades.Add(up.m_UpgradeData);
        StatsManager.m_Instance.m_LoadedDataNodes.Add(gO);
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
