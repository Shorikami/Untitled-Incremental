using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dice : MonoBehaviour
{
    private float m_RollDelay = 1.0f;
    private float m_CurrRoll;
    private bool m_PaidOut = true;

    public void Update()
    {
        // todo: better timer?
        if (m_CurrRoll > 0.0f)
        {
            m_CurrRoll -= Time.deltaTime;
        }
        else
        {
            if (!m_PaidOut)
            {
                m_CurrRoll = 0.0f;
                Payout();
                m_PaidOut = true;
            }
        }
    }

    public void RollDice()
    {
        if (m_CurrRoll <= 0.0f)
        {
            m_PaidOut = false;
            m_CurrRoll = m_RollDelay;
        }
        else
        {
            Debug.Log("Cannot roll.");
        }
    }

    public void Payout()
    {
        Debug.Log("You rolled a " + Random.Range(1, 7) + "!");
    }
}
