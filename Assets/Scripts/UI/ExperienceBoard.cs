using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ExperienceBoard : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_LevelNumberDisplay;
    [SerializeField] private GameObject m_ExpBar;
    private Slider m_ExpSlider;

    public StatsManager.GameCurrencyType m_ExpType;
    private Experience m_ExpDataToHandle;

    void Start()
    {
        m_ExpDataToHandle = StatsManager.m_Instance.FindContainer<Experience>(m_ExpType).GetComponent<Experience>();
        m_ExpSlider = m_ExpBar.GetComponent<Slider>();
    }

    void Update()
    {
        m_LevelNumberDisplay.text = m_ExpDataToHandle.m_ExpData.m_CurrLevel.ToString();
        m_ExpSlider.value = m_ExpDataToHandle.m_ExpData.m_CurrExp / (float)m_ExpDataToHandle.m_ExpData.RequiredExperience();
    }
}
