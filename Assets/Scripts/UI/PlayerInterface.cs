using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerInterface : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_CurrencyNumber;

    public StatsManager.GameCurrencyType m_DisplayWhatCurr = StatsManager.GameCurrencyType.Coins;
    public Collectable.CollectableType m_DisplayWhatColl = Collectable.CollectableType.Default;

    private void Start()
    {
        m_CurrencyNumber.text = StatsManager.m_Instance.FindStatContainer(m_DisplayWhatCurr, m_DisplayWhatColl)
            .GetComponent<GameCurrency>().m_Currency.m_TotalValue.ToString();
    }

    public void UpdateText(string text)
    {
        m_CurrencyNumber.text = text;
    }
}
