using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncrementalPlatform : MonoBehaviour
{
    [SerializeField] private GameObject m_Collectable;
    [Min(0)] public float m_TimerMultiplier;
    private float m_CurrTime;

    private void Start()
    {
        m_CurrTime = 1.0f / m_TimerMultiplier;
    }

    // Update is called once per frame
    void Update()
    {
        m_CurrTime -= Time.deltaTime;

        if (m_CurrTime <= 0.0f)
        {
            m_CurrTime = 1.0f / m_TimerMultiplier;

            // TODO: make the collectables guaranteed to spawn without overlap
            float halfScaleX = transform.localScale.x / 2.0f;
            float halfScaleZ = transform.localScale.z / 2.0f;

            Vector3 spawnPos = new Vector3(Random.Range(transform.position.x - halfScaleX, transform.position.x + halfScaleX), 
                transform.position.y + ((m_Collectable.transform.localScale.y + transform.localScale.y) / 2.0f),
                Random.Range(transform.position.z - halfScaleZ, transform.position.z + halfScaleZ));

            GameObject spawnedCol = Instantiate(m_Collectable, spawnPos, Quaternion.identity);
            spawnedCol.transform.parent = transform;
        }
    }
}
