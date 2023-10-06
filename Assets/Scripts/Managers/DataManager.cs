using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DataManager : MonoBehaviour
{
    [Header("File Storage Configuration\n====================")]
    [SerializeField] private string m_FileName;
    public static DataManager m_Instance { get; private set; }

    [SerializeField] private GameObject m_LoadedCurrencyPrefab;
    [SerializeField] private GameObject m_LoadedUpgradePrefab;

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
        m_SavedData = FindSavedData();
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

        GameObject gO = Instantiate(m_LoadedCurrencyPrefab, StatsManager.m_Instance.gameObject.transform);
        gO.AddComponent<GameCurrency>();
        gO.GetComponent<GameCurrency>().m_Currency.m_CurrencyType = StatsManager.GameCurrencyType.Coins;
        gO.GetComponent<GameCurrency>().m_Currency.m_Count = 0;

        m_GameData.m_GameCurrencies.Add(gO.GetComponent<GameCurrency>().m_Currency);
        StatsManager.m_Instance.m_LoadedDataNodes.Add(gO);

        gO = Instantiate(m_LoadedCurrencyPrefab, StatsManager.m_Instance.gameObject.transform);
        gO.AddComponent<Experience>();
        gO.GetComponent<Experience>().m_ExpData.m_ExpType = StatsManager.GameCurrencyType.Experience;
        gO.GetComponent<Experience>().m_ExpData.m_TotalExperience = 0;

        m_GameData.m_ExperienceTypes.Add(gO.GetComponent<Experience>().m_ExpData);
        StatsManager.m_Instance.m_LoadedDataNodes.Add(gO);

        gO = Instantiate(m_LoadedCurrencyPrefab, StatsManager.m_Instance.gameObject.transform);
        gO.AddComponent<Upgrade>();
        gO.GetComponent<Upgrade>().m_UpgradeData.m_UpgradeName = "Coins Value";
        gO.GetComponent<Upgrade>().m_UpgradeData.m_Description = "Increases value of coins by +25% per level. Coin value is doubled every 25 levels.";
        gO.GetComponent<Upgrade>().m_UpgradeData.m_CurrLevel = 0;
        gO.GetComponent<Upgrade>().m_UpgradeData.m_MaxLevel = 500;
        gO.GetComponent<Upgrade>().m_UpgradeData.m_BaseCost = 25;

        m_GameData.m_AvailableUpgrades.Add(gO.GetComponent<Upgrade>().m_UpgradeData);
        StatsManager.m_Instance.m_LoadedDataNodes.Add(gO);

        gO = Instantiate(m_LoadedCurrencyPrefab, StatsManager.m_Instance.gameObject.transform);
        gO.AddComponent<Upgrade>();
        gO.GetComponent<Upgrade>().m_UpgradeData.m_UpgradeName = "Growth";
        gO.GetComponent<Upgrade>().m_UpgradeData.m_Description = "Increases spawn rate by +20% per level.";
        gO.GetComponent<Upgrade>().m_UpgradeData.m_CurrLevel = 0;
        gO.GetComponent<Upgrade>().m_UpgradeData.m_MaxLevel = 10;
        gO.GetComponent<Upgrade>().m_UpgradeData.m_BaseCost = 200;

        m_GameData.m_AvailableUpgrades.Add(gO.GetComponent<Upgrade>().m_UpgradeData);
        StatsManager.m_Instance.m_LoadedDataNodes.Add(gO);

        gO = Instantiate(m_LoadedCurrencyPrefab, StatsManager.m_Instance.gameObject.transform);
        gO.AddComponent<Upgrade>();
        gO.GetComponent<Upgrade>().m_UpgradeData.m_UpgradeName = "XP Value";
        gO.GetComponent<Upgrade>().m_UpgradeData.m_Description = "Increases XP value by +25% per level. XP Value is doubled every 25 levels.";
        gO.GetComponent<Upgrade>().m_UpgradeData.m_CurrLevel = 0;
        gO.GetComponent<Upgrade>().m_UpgradeData.m_MaxLevel = 500;
        gO.GetComponent<Upgrade>().m_UpgradeData.m_BaseCost = 50;

        m_GameData.m_AvailableUpgrades.Add(gO.GetComponent<Upgrade>().m_UpgradeData);
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
            gO = Instantiate(m_LoadedCurrencyPrefab, StatsManager.m_Instance.gameObject.transform);
            gO.AddComponent<GameCurrency>();
            gO.GetComponent<GameCurrency>().m_Currency = cd;
            StatsManager.m_Instance.m_LoadedDataNodes.Add(gO);
        }

        foreach (ExperienceData ed in m_GameData.m_ExperienceTypes)
        {
            gO = Instantiate(m_LoadedCurrencyPrefab, StatsManager.m_Instance.gameObject.transform);
            gO.AddComponent<Experience>();
            gO.GetComponent<Experience>().m_ExpData = ed;
            StatsManager.m_Instance.m_LoadedDataNodes.Add(gO);
        }

        foreach (UpgradeData ud in m_GameData.m_AvailableUpgrades)
        {
            gO = Instantiate(m_LoadedCurrencyPrefab, StatsManager.m_Instance.gameObject.transform);
            gO.AddComponent<Upgrade>();
            gO.GetComponent<Upgrade>().m_UpgradeData = ud;
            StatsManager.m_Instance.m_LoadedDataNodes.Add(gO);
        }
    }
}
