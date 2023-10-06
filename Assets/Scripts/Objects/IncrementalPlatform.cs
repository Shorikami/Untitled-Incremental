using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncrementalPlatform : MonoBehaviour
{
    [SerializeField] private GameObject m_Collectable;
    [Min(0)] public float m_TimerMultiplier;
    private float m_CurrTime;

    public Collectable.CollectableType m_CollectableType;

    private GameCurrency m_CurrencyToModify;
    private Experience m_ExpToModify;
    private List<GameObject> m_Upgrades;

    private Upgrade m_GrowthUpgrade;

    private void Start()
    {
        InitializeCollectable();
        m_CurrTime = 1.0f / m_TimerMultiplier;
    }

    // Update is called once per frame
    void Update()
    {
        m_CurrTime -= Time.deltaTime;

        if (m_CurrTime <= 0.0f)
        {
            m_CurrTime = 1.0f / (m_TimerMultiplier * m_GrowthUpgrade.m_UpgradeData.m_UpgradeBonuses.m_CurrentBonus);

            // TODO: make the collectables guaranteed to spawn without overlap
            float halfScaleX = transform.localScale.x / 2.0f;
            float halfScaleZ = transform.localScale.z / 2.0f;

            Vector3 spawnPos = new Vector3(Random.Range(transform.position.x - halfScaleX, transform.position.x + halfScaleX), 
                transform.position.y + ((m_Collectable.transform.localScale.y + transform.localScale.y) / 2.0f),
                Random.Range(transform.position.z - halfScaleZ, transform.position.z + halfScaleZ));

            GameObject spawnedCol = Instantiate(m_Collectable, spawnPos, Quaternion.identity);
            spawnedCol.transform.parent = transform;
            spawnedCol.GetComponent<Collectable>().m_CurrencyToModify = m_CurrencyToModify;
            spawnedCol.GetComponent<Collectable>().m_ExpToModify = m_ExpToModify;
        }
    }

    private void InitializeCollectable()
    {
        switch (m_CollectableType)
        {
            case Collectable.CollectableType.Default:
                m_CurrencyToModify = StatsManager.m_Instance.FindContainer<GameCurrency>(StatsManager.GameCurrencyType.Coins).GetComponent<GameCurrency>();
                m_ExpToModify = StatsManager.m_Instance.FindContainer<Experience>(StatsManager.GameCurrencyType.Experience).GetComponent<Experience>();

                m_Upgrades = StatsManager.m_Instance.FindUpgrades(m_CollectableType);
                m_GrowthUpgrade = m_Upgrades.Find(search => search.GetComponent<Upgrade>().m_UpgradeData.m_UpgradeName.CompareTo("Growth") == 0).GetComponent<Upgrade>();
                break;
        }
    }
}
