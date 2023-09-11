using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DataManager : MonoBehaviour
{
    [Header("File Storage Configuration")]
    [SerializeField] private string m_FileName;
    public static DataManager m_Instance { get; private set; }

    private GameData m_GameData;
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
    }

    private void Start()
    {
        m_FileDataHandler = new FileHandler(Application.persistentDataPath, m_FileName);
        m_SavedData = FindSavedData();
        Load();
    }

    public void NewGame()
    {
        m_GameData = new GameData();
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
        m_GameData = m_FileDataHandler.Load();

        if (m_GameData == null)
        {
            Debug.Log("No data found. Initializing new data..");
            NewGame();
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
}
