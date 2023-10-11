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

    // what currency this prestige will give
    public StatsManager.GameCurrencyType m_ResetFor;

    public List<StatsManager.GameCurrencyType> m_Resets; 

    // What experience type to check
    public StatsManager.GameCurrencyType m_ExpToCheck;

    // how much currency the player will get based on the level of this experience
    // (also determines if the player can prestige)
    private Experience m_MultiplierBasing;

    // level required for prestiging
    [Min(0)] public int m_RequiredLevelToPrestige;

    public string m_PrestigeCurrName;
    public string m_DescriptionText;
    public string m_ActionName;

    private bool m_Displaying;

    void Start()
    {
        m_BoardLock.GetComponent<BoardLock>().m_LockedText.text = "Unlocks at Level " + m_RequiredLevelToPrestige.ToString();
        m_PrestigeName.text = m_ActionName;
        m_Description.text = m_DescriptionText;
        m_CurrAmount.text = "You will earn 0 " + m_PrestigeCurrName;

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
        m_MultiplierBasing = StatsManager.m_Instance.FindExperience(m_ExpToCheck);
        m_BoardLock.SetActive(display);
        m_PrestigePanel.SetActive(display);
        gameObject.GetComponent<MeshRenderer>().enabled = display;
        m_PrestigeBoardUnlock.SetActive(display);

        if (display)
            AddUpgradesToBoard();
    }

    private void UpdateCurrAmountText()
    {
        m_CurrAmount.text = "You will earn " + PayoutFormula() + " " + m_PrestigeCurrName;
    }

    private void CheckForDisplayingSelf()
    {
        if (m_MultiplierBasing.m_ExpData.m_CurrLevel >= (m_RequiredLevelToPrestige - 10))
            Initialize(true);
    }

    private void CheckForBoardLock()
    {
        m_BoardLock.SetActive(m_MultiplierBasing.m_ExpData.m_CurrLevel < m_RequiredLevelToPrestige ? true : false);
    }

    private void AddUpgradesToBoard()
    {
        // first add the upgrades if necessary
        // there will (most likely) never be a case where this will be null or empty
        // when the upgrades are already created during the game load
        List<Upgrade> upgrades = StatsManager.m_Instance.FindUpgrades(m_ResetFor);

        if (upgrades == null || upgrades.Count == 0)
        { 
            DataManager.m_Instance.NewUpgrade("Credits Coins Value", 
                "Increases value of coins by +25% per level. Coin value is doubled every 25 levels.",
            500, 5, false, 25, 2.0f, 0.25f, StatsManager.GameCurrencyType.Coins, StatsManager.GameCurrencyType.Credits,
            StatsManager.NonCurrencyUpgrades.Invalid, Collectable.CollectableType.None);

            DataManager.m_Instance.NewUpgrade("Credits EXP Value",
                "Increases value of EXP by +25% per level. EXP value is doubled every 25 levels.",
            500, 5, false, 25, 2.0f, 0.25f, StatsManager.GameCurrencyType.Coins, StatsManager.GameCurrencyType.Credits,
            StatsManager.NonCurrencyUpgrades.Invalid, Collectable.CollectableType.None);
        }

        // then initialize the board panel
        m_PrestigeBoardUnlock.GetComponent<BoardPanel>().Initialize();
    }

    public void Prestige()
    {
        GameCurrency prestCurr = StatsManager.m_Instance.FindGameCurrency(StatsManager.GameCurrencyType.Credits);

        if (prestCurr == null)
        {
            DataManager.m_Instance.NewCurrency(m_ResetFor, Collectable.CollectableType.None);
            prestCurr = StatsManager.m_Instance.FindGameCurrency(StatsManager.GameCurrencyType.Credits);
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

    private int PayoutFormula()
    {
        return Mathf.FloorToInt(11.0f * Mathf.Pow(1.7f, (m_MultiplierBasing.m_ExpData.m_CurrLevel / 10.0f)));
    }
}
