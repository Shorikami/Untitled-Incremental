using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Upgrade : MonoBehaviour
{
    public GameObject m_CurrencyPanel;

    public enum UpgradeType
    {
        Currency,
        Exp,
        Tier
    }

    public string m_UpgradeName;
    public string m_Description;
    public string m_BonusDesc;

    public UpgradeType m_UpgradeType;
    public int m_CurrLevel, m_MaxLevel;
    public int m_CurrentBonus;

    [Min(1)]
    public int m_CurrCost;

    public void OpenUpgradeMenu()
    {
        m_CurrencyPanel.GetComponent<CurrencyPanel>().ToggleUpgradeMenu(true);
    }
}
