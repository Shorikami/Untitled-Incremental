using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PrestigeBoard : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_PrestigeName;
    [SerializeField] private TextMeshProUGUI m_Description;
    [SerializeField] private TextMeshProUGUI m_CurrAmount;

    [SerializeField] private GameObject m_PrestigePanel;
    [SerializeField] private GameObject m_BoardLock;

    [SerializeField] private GameObject m_PrestigeBoardUnlock;

    public PrestigeConditions m_Conditions;

    private bool m_Displaying;

    void Start()
    {
        m_BoardLock.GetComponent<BoardLock>().m_LockedText.text = "Unlocks at Level " + m_Conditions.m_ReqBaseLevel.ToString();
        m_PrestigeName.text = m_Conditions.m_ActionName;
        m_Description.text = m_Conditions.m_DescriptionText;
        m_CurrAmount.text = "You will earn 0 " + m_Conditions.m_PrestigeCurrName;

        Initialize();
    }

    private void Update()
    {
        if (!m_Displaying)
        {
            CheckForDisplayingSelf();
            return;
        }

        CheckForBoardLock();
        UpdateCurrAmountText();
    }

    private void Initialize(bool display = false)
    {
        m_Displaying = display;
        m_BoardLock.SetActive(display);
        m_PrestigePanel.SetActive(display);
        gameObject.GetComponent<MeshRenderer>().enabled = display;
        m_PrestigeBoardUnlock.SetActive(display);

        if (display)
        {
            if (StatsManager.m_Instance.FindGameCurrency(m_Conditions.m_ResetFor) == null)
                DataManager.m_Instance.NewCurrency(m_Conditions.m_ResetFor, Collectable.CollectableType.None);

            AddUpgradesToBoard();
            m_PrestigeBoardUnlock.GetComponent<BoardPanel>().m_DisplayThis = 
                StatsManager.m_Instance.FindGameCurrency(m_Conditions.m_ResetFor);
        }

        m_CurrAmount.enabled = m_Conditions.m_DisplayEarnings;
    }

    private void UpdateCurrAmountText()
    {
        m_CurrAmount.text = "You will earn " + m_Conditions.PayoutFormula() + " " + m_Conditions.m_PrestigeCurrName;
    }

    private void CheckForDisplayingSelf()
    {
        if (m_Conditions.ToCheck().m_ExpData.m_CurrLevel >= (m_Conditions.m_ReqBaseLevel - 10))
            Initialize(true);
    }

    private void CheckForBoardLock()
    {
        m_BoardLock.SetActive(m_Conditions.ToCheck().m_ExpData.m_CurrLevel < m_Conditions.m_ReqBaseLevel ? true : false);
    }

    private void AddUpgradesToBoard()
    {
        // first add the upgrades if necessary
        // there will (most likely) never be a case where this will be null or empty
        // when the upgrades are already created during the game load
        List<Upgrade> upgrades = StatsManager.m_Instance.FindUpgrades(m_Conditions.m_ResetFor);

        if ((upgrades == null || upgrades.Count == 0) && m_Conditions.m_UsesDynamicBoard)
        {
            m_Conditions.AddUpgradeAdditions();
        }

        // then initialize the board panel
        m_PrestigeBoardUnlock.GetComponent<BoardPanel>().Initialize();
    }

    public void Prestige()
    {
        m_Conditions.Prestige();
    }
}
