using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Upgrade : MonoBehaviour
{
    public enum UpgradeType
    {
        Currency,
        Exp,
        Tier
    }

    public UpgradeType m_UpgradeType;
    public int m_CurrLevel, m_MaxLevel;

    [Min(1)]
    public int m_CurrCost;
}
