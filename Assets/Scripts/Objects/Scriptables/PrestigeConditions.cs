using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Prestige Conditions", menuName = "Scriptables/Prestige Conditions")]
public class PrestigeConditions : ScriptableObject
{
    [System.Serializable]
    public struct UpgradeBoardAddition
    {
        public string s_UpgradeName;
        public string s_Description;
        public int s_MaxLevel;
        public double s_BaseCost;
        public bool s_StaticCost;
        public int s_BonusReqLevels;
        public float s_LevelBonusMulti;
        public float s_BonusMulti;

        public StatsManager.GameCurrencyType s_Currtype;
        public StatsManager.GameCurrencyType s_BoughtWith;
        public StatsManager.NonCurrencyUpgrades s_NonCurrType;
        public Collectable.CollectableType s_Colltype;
    }

    // Required level to presitge e.g. 31
    [Min(0)] public int m_ReqBaseLevel;

    // Whether or not this prestige scales after reaching the minimum level
    public bool m_Scales;
    // How much this prestige scales by, if it scales at all
    public AnimationCurve m_LevelScale;
    // How much this prestige pays out without any modifiers
    [Min(1)] public int m_BasePayoutValue;

    // Whether or not the currency earned is scaled with player's current level
    public bool m_ScalesWithExpLevel;

    // What experience type to check
    public StatsManager.GameCurrencyType m_ExpToCheck;
    // what currency this prestige will give
    public StatsManager.GameCurrencyType m_ResetFor;
    // What does prestiging reset
    public List<StatsManager.GameCurrencyType> m_Resets;

    public string m_PrestigeCurrName;
    public string m_DescriptionText;
    public string m_ActionName;

    // Whether or not the earnings text is displayed
    // e.g. "You will earn X currency" right above the prestige button
    public bool m_DisplayEarnings;

    // Whether or not this prestige type uses a dynamic board (this will
    // determine if the AddUpgradesToBoard function is needed)
    public bool m_UsesDynamicBoard;
    // The dynamic upgrades to add
    public List<UpgradeBoardAddition> m_UpgradeAdditions;

    public Experience ToCheck()
    {
        return StatsManager.m_Instance.FindExperience(m_ExpToCheck);
    }

    public int EvaluateRequiredLevel(int currLevel)
    {
        // Should start at parameter level - base level so the curve doesn't
        // have to start at extreme values
        return Mathf.CeilToInt(m_LevelScale.Evaluate(currLevel - m_ReqBaseLevel));
    }

    public void AddUpgradeAdditions()
    {
        foreach (UpgradeBoardAddition upb in m_UpgradeAdditions)
        {
            DataManager.m_Instance.NewUpgrade(upb.s_UpgradeName, upb.s_Description, upb.s_MaxLevel, upb.s_BaseCost, 
                upb.s_StaticCost, upb.s_BonusReqLevels, upb.s_LevelBonusMulti, upb.s_BonusMulti, upb.s_Currtype, upb.s_BoughtWith,
                upb.s_NonCurrType, upb.s_Colltype);
        }
    }

    public void Prestige()
    {
        GameCurrency prestCurr = StatsManager.m_Instance.FindGameCurrency(m_ResetFor);

        if (prestCurr == null)
        {
            DataManager.m_Instance.NewCurrency(m_ResetFor, Collectable.CollectableType.None);
            prestCurr = StatsManager.m_Instance.FindGameCurrency(m_ResetFor);
        }

        prestCurr.m_Currency.UpdateValue(PayoutFormula());

        foreach (StatsManager.GameCurrencyType type in m_Resets)
        {
            StatsManager.m_Instance.ResetUpgrades(type);
            GameObject go = StatsManager.m_Instance.FindStatContainer(type);

            if (go.TryGetComponent<GameCurrency>(out GameCurrency gc))
            {
                gc.ResetData();
                StatsManager.m_Instance.Player.GetComponent<PlayerController>().PlayerUI.UpdateText(gc);
            }

            else if (go.TryGetComponent<Experience>(out Experience ex))
                ex.ResetData();
        }
    }

    public int PayoutFormula()
    {
        if (!m_ScalesWithExpLevel)
            return m_BasePayoutValue;

        // TODO: editor values for this formula
        return m_BasePayoutValue 
            + Mathf.FloorToInt(11.0f * Mathf.Pow(1.7f, (ToCheck().m_ExpData.m_CurrLevel / 10.0f)));
    }

    //// Whether or not this prestige type has additional conditions
    //// e.g. after reaching level 41, start using the additional conditions
    //public bool m_UsesAdditionalConditions;
    //
    //public struct AdditionalConditions
    //{
    //    public int m_ReqCurrLevelMin;
    //    public int m_ReqCurrLevelMax;
    //
    //    public int m_AdditiveScale;
    //};
    //
    //public List<AdditionalConditions> m_Conditions;
}
