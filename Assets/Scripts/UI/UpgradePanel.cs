using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UpgradePanel : MonoBehaviour
{
    public TextMeshProUGUI m_UpgrName;
    public TextMeshProUGUI m_Description;
    public TextMeshProUGUI m_Level;
    public TextMeshProUGUI m_CostText;
    public TextMeshProUGUI m_BonusText;

    public void UpdateText(Upgrade upgr)
    {
        m_UpgrName.text = upgr.m_UpgradeData.m_UpgradeName;
        m_Description.text = upgr.m_UpgradeData.m_Description;
        m_BonusText.text = ""; //upgr.m_UpgradeData.m_BonusDesc;

        UpdateLevelText(upgr);
    }

    public void UpdateLevelText(Upgrade upgr)
    {
        m_CostText.text = "Cost: " + upgr.m_UpgradeData.IntCost().ToString();

        m_Level.text =
        "Level " + upgr.m_UpgradeData.m_CurrLevel.ToString() + " / " + upgr.m_UpgradeData.m_MaxLevel.ToString();
    }
}
