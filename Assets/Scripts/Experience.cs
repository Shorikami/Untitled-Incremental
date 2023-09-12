using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Experience : MonoBehaviour, ISavableData
{
    // TODO: Eventually crunch this down to handle super big numbers
    public int m_TotalExperience;

    public int m_CurrLevel;
    public int m_CurrExp;

    public void SaveData(ref GameData data)
    {
        data.m_Experience = m_TotalExperience;
    }
    public void LoadData(GameData data)
    {
        m_CurrExp = m_TotalExperience = data.m_Experience;
        CalculateLevel();
    }

    public void AddExperience(int exp)
    {
        m_TotalExperience += exp;
        m_CurrExp += exp;
        CalculateForNextLevel();
    }

    // Calculate level on start up (after loading)
    public void CalculateLevel()
    {
        // early break if this instance is a new game
        if (m_TotalExperience <= 0)
            return;

        // TODO: make this more efficient. don't think looping this
        // 9000 times will be efficient at all
        bool repeat = true;
        while (repeat)
        {
            repeat = CalculateForNextLevel();
        }
    }

    // Calculate for level after specific actions
    public bool CalculateForNextLevel()
    {
        // check if player has reached xp threshold first...
        bool leveledUp = false;

        if (m_CurrExp >= RequiredExperience())
        {
            m_CurrExp -= RequiredExperience();
            leveledUp = true;
        }

        // that way the level can be applied independently from resetting
        // the required experience
        m_CurrLevel = leveledUp ? m_CurrLevel + 1 : m_CurrLevel;

        return leveledUp;
    }

    // Required amount of experience to level up
    // formula: 25 + (5 * currLevel * 1.05^currLevel)
    public int RequiredExperience()
    {
        return 25 + Mathf.RoundToInt(5 * m_CurrLevel * Mathf.Pow(1.05f, m_CurrLevel));
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
