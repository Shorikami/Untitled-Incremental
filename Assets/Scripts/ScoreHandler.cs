using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreHandler : MonoBehaviour
{
    public static ScoreHandler m_Instance { get; private set; }

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

    public List<Currency> m_Currencies;

    private void Start()

    {
        //// temp load in manually. no saving
        //// todo: load in currencies from save
        //for (int i = 0; i < 6; ++i)
        //{
        //    m_Currencies.Add(new Currency());
        //}
    }
}
