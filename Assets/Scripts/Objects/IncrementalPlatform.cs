using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncrementalPlatform : MonoBehaviour
{
    [SerializeField] private GameObject m_Collectable;
    [Min(0)] public float m_BaseTimer;
    private float m_CurrTime;

    public Collectable.CollectableType m_CollectableType;

    private GameCurrency m_CurrencyToModify;
    private Experience m_ExpToModify;
    private List<GameObject> m_Upgrades;

    private Upgrade m_GrowthUpgrade;

    private void Start()
    {
        InitializeCollectable();
        m_CurrTime = 1.0f / m_BaseTimer;
    }

    // Update is called once per frame
    void Update()
    {
        m_CurrTime -= Time.deltaTime;

        if (m_CurrTime <= 0.0f)
        {
            m_CurrTime = CalculateTime();

            // TODO: make the collectables guaranteed to spawn without overlap
            float halfScaleX = transform.localScale.x / 2.0f;
            float halfScaleZ = transform.localScale.z / 2.0f;

            Vector3 spawnPos = new Vector3(Random.Range(transform.position.x - halfScaleX, transform.position.x + halfScaleX), 
                transform.position.y + ((m_Collectable.transform.localScale.y + transform.localScale.y) / 2.0f),
                Random.Range(transform.position.z - halfScaleZ, transform.position.z + halfScaleZ));

            GameObject spawnedCol = Instantiate(m_Collectable, spawnPos, Quaternion.identity);
            spawnedCol.transform.parent = transform;

            Collectable modifyC = spawnedCol.GetComponent<Collectable>();
            modifyC.m_CurrencyToModify = m_CurrencyToModify;
            modifyC.m_ExpToModify = m_ExpToModify;
            modifyC.CollectType = m_CollectableType;
            modifyC.RefreshBaseValues();
        }
    }

    private float CalculateTime()
    { 
        return 1.0f / (m_BaseTimer * m_GrowthUpgrade.m_UpgradeData.m_UpgradeBonuses.m_CurrentBonus);
    }

    private void InitializeCollectable()
    {
        m_CurrencyToModify = StatsManager.m_Instance.FindStatContainer
            (StatsManager.GameCurrencyType.Coins, m_CollectableType).GetComponent<GameCurrency>();

        m_ExpToModify = StatsManager.m_Instance.FindStatContainer
            (StatsManager.GameCurrencyType.Experience, m_CollectableType).GetComponent<Experience>();

        m_Upgrades = StatsManager.m_Instance.FindUpgrades(m_CollectableType);

        // todo: improve the search of this?
        m_GrowthUpgrade = m_Upgrades
            .Find(search => search.GetComponent<Upgrade>().m_UpgradeData.m_UpgradeName.Contains("Growth"))
            .GetComponent<Upgrade>();
    }
}
