using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Persistent Bonuses", menuName = "Scriptables/Persistent Bonuses")]
public class PersistentGlobals : ScriptableObject
{
    [System.Serializable]
    public struct PersistentData
    {
        public string m_Description;
    }

    public StatsManager.GameCurrencyType m_PersistentCurrency;
    public List<PersistentData> m_DataDescriptions;
}
