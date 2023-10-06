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

    private bool m_TaggedForDeletion = false;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && !m_TaggedForDeletion)
        {
            m_TaggedForDeletion = true;
            Collect(other.gameObject.GetComponentInParent<PlayerController>());
        }
    }

    private void Collect(PlayerController pc)
    {
        m_ExpToModify.AddExperience(Mathf.FloorToInt(m_BaseExpValue * StatsManager.m_Instance.m_Multipliers[m_ExpToModify.m_ExpData.m_ExpType]));
        m_CurrencyToModify.UpdateCurrency(Mathf.FloorToInt(m_BaseCurrencyValue * StatsManager.m_Instance.m_Multipliers[m_CurrencyToModify.m_Currency.m_CurrencyType]));

        pc.PlayerUI.UpdateText(m_CurrencyToModify.m_Currency.m_TotalCount.ToString());
        Destroy(gameObject);
    }
}
