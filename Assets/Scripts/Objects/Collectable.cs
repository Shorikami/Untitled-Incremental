using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    // designates what statistics this specific collectable modifies.
    // this is an attempt to prevent having to search the StatsManager
    // container every time for the appropriate data
    public enum CollectableType
    { 
        None = 0,
        Default
    }

    [SerializeField] private float m_BaseCurrencyValue = 1.0f;
    [SerializeField] private float m_BaseExpValue = 1.0f;

    [HideInInspector] public GameCurrency m_CurrencyToModify;
    [HideInInspector] public Experience m_ExpToModify;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Collect();
        }
    }

    private void Collect()
    {
        m_ExpToModify.AddExperience(Mathf.FloorToInt(m_BaseExpValue));
        Destroy(gameObject);
    }
}
