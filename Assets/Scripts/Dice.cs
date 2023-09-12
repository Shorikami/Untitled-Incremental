using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dice : MonoBehaviour
{
    private float m_RollDelay = 1.0f;
    private float m_CurrRoll;
    private bool m_PaidOut = true;

    public Incr2DHandler m_2DHandler;
    public Experience m_ExperienceHandler;

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
        int res = Random.Range(1, 7);
        Debug.Log("You rolled a " + res + "!");
        m_2DHandler.UpdateValue(res);
    }
}
