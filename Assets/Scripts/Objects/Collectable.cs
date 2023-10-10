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

    [SerializeField] private float m_BaseCurrencyValue;
    [SerializeField] private float m_BaseExpValue;

    private CollectableType m_CollectableType;


    public float BaseCurrencyValue
    {
        get { return m_BaseCurrencyValue; }
        set { m_BaseCurrencyValue = value; }
    }

    public float BaseExpValue
    {
        get { return m_BaseExpValue; }
        set { m_BaseExpValue = value; }
    }

    public CollectableType CollectType
    {
        get { return m_CollectableType; }
        set { m_CollectableType = value; }
    }

    [HideInInspector] public GameCurrency m_CurrencyToModify;
    [HideInInspector] public Experience m_ExpToModify;

    private bool m_TaggedForDeletion = false;

    void Awake()
    {

    }

    void Start()
    {

    }

    void Update()
    {

    }

    public void RefreshBaseValues()
    { 
        switch (m_CollectableType)
        {
            case CollectableType.Default:
                m_BaseCurrencyValue = 10.0f;
                m_BaseExpValue = 5.0f;
                break;

            default:
                m_BaseCurrencyValue = 1.0f;
                m_BaseExpValue = 1.0f;
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && !m_TaggedForDeletion)
        {
            m_TaggedForDeletion = true;
            Collect();
        }
    }

    private void Collect()
    {
        m_ExpToModify.AddExperience(
            Mathf.FloorToInt(m_BaseExpValue * 
            StatsManager.m_Instance.FindAllUpgradeMultipliers(m_ExpToModify.m_ExpData.m_CurrencyType, false)));

        m_CurrencyToModify.UpdateCurrency(Mathf.
            FloorToInt(m_BaseCurrencyValue * 
            StatsManager.m_Instance.FindAllUpgradeMultipliers(m_CurrencyToModify.m_Currency.m_CurrencyType, false)));
        
        PlayerController pc = StatsManager.m_Instance.Player.GetComponentInParent<PlayerController>();
        pc.PlayerUI.UpdateText(m_CurrencyToModify);

        Destroy(gameObject);
    }
}
