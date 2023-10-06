using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    [SerializeField] private float m_BaseCurrencyValue = 1.0f;
    [SerializeField] private float m_BaseExpValue = 1.0f;

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
        Destroy(gameObject);
    }
}
