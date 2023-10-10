using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Experience : MonoBehaviour, ISavableData
{
    public ExperienceData m_ExpData;

    public void SaveData(ref GameData data)
    {
        data.SaveExperience(m_ExpData);
    }
    public void LoadData(GameData data)
    {
        ExperienceData toLoad = data.FindExperienceType(m_ExpData.m_CurrencyType);
        m_ExpData.m_TotalValue = toLoad.m_TotalValue;
        CalculateLevel();
    }

    // Calculate level on start up (after loading)
    public void CalculateLevel()
    {
        // early break if this instance is a new game
        if (m_ExpData.m_TotalValue <= 0)
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

        if (m_ExpData.m_CurrExp >= m_ExpData.RequiredExperience())
        {
            m_ExpData.m_CurrExp -= m_ExpData.RequiredExperience();
            leveledUp = true;
        }

        // that way the level can be applied independently from resetting
        // the required experience
        if (leveledUp)
            LevelUp();

        return leveledUp;
    }

    public void AddExperience(int exp)
    {
        m_ExpData.m_TotalValue += exp;
        m_ExpData.m_CurrExp += exp;
        CalculateForNextLevel();
    }

    public void LevelUp()
    {
        ++m_ExpData.m_CurrLevel;
        StatsManager.m_Instance.FindStatContainer(m_ExpData.m_PerkType, m_ExpData.m_CollectableType)
            .GetComponent<GameCurrency>()
            .UpdateCurrency(1);
    }
}

[System.Serializable]
public class ExperienceData : Data
{
    public int m_CurrLevel;
    public int m_CurrExp;

    // what kind of level perk currency this specific EXP container affects
    // (there may be more than 1 type of perk currency)
    public StatsManager.GameCurrencyType m_PerkType;

    // Required amount of experience to level up
    // formula: 25 + (5 * currLevel * 1.05^currLevel)
    public int RequiredExperience()
    {
        return 25 + Mathf.RoundToInt(5 * m_CurrLevel * Mathf.Pow(1.05f, m_CurrLevel));
    }

    public void Save(ExperienceData other)
    {
        m_TotalValue = other.m_TotalValue;
        m_CurrLevel = other.m_CurrLevel;
        m_CurrExp = other.m_CurrExp;
        m_PerkType = other.m_PerkType;
        m_CurrencyType = other.m_CurrencyType;
    }

    public override void UpdateValue(double toAdd)
    {
        base.UpdateValue(toAdd);
    }
}