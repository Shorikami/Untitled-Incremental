using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerInterface : MonoBehaviour
{
    [HideInInspector] public PlayerController m_OwnerPlayer;
    [SerializeField] private TextMeshProUGUI m_CurrencyNumber;
    [SerializeField] private GameObject m_NPCPrompt;
    //private TextMeshProUGUI m_PromptText;

    public StatsManager.GameCurrencyType m_DisplayWhatCurr = StatsManager.GameCurrencyType.Coins;
    public Collectable.CollectableType m_DisplayWhatColl = Collectable.CollectableType.Default;

    private void Start()
    {
        m_NPCPrompt.SetActive(false);
        m_CurrencyNumber.text = StatsManager.m_Instance.FindStatContainer(m_DisplayWhatCurr, m_DisplayWhatColl)
            .GetComponent<GameCurrency>().m_Currency.m_TotalValue.ToString();
    }

    public void UpdateText(GameCurrency gc)
    {
        if (m_DisplayWhatCurr == gc.m_Currency.m_CurrencyType)
            m_CurrencyNumber.text = gc.m_Currency.m_TotalValue.ToString();
    }

    public void DisplayNPCPrompt(bool show, NPCController target)
    {
        if (show)
        {
            if (m_OwnerPlayer.m_FirstPerson)
            {
                if (m_OwnerPlayer.CurrentlyLookingAt != null)
                {
                    m_NPCPrompt.SetActive(show);
                    m_OwnerPlayer.m_CanInteractWithNPCs = show;
                    m_OwnerPlayer.m_CurrNPC = target;
                }
            }
            else
            {
                m_NPCPrompt.SetActive(show);
                m_OwnerPlayer.m_CanInteractWithNPCs = show;
                m_OwnerPlayer.m_CurrNPC = target;
            }
            return;
        }
        m_NPCPrompt.SetActive(show);
        m_OwnerPlayer.m_CanInteractWithNPCs = show;
        m_OwnerPlayer.m_CurrNPC = target;
    }
}
