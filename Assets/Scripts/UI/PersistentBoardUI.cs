using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentBoardUI : MonoBehaviour
{
    [Header("UI Board")]
    [SerializeField] private GameObject m_Board;
    [SerializeField] private GameObject m_ScrollPanel;
    [SerializeField] private GameObject m_PanelContent;
    [SerializeField] private GameObject m_HorizontalBlankPrefab;

    [SerializeField] private PersistentGlobals m_PersistentDisplayContainer;

    [Header("Board Layout")]
    public bool m_ModifyLayout = false;
    [Min(0)] public int m_PreferredWidth;
    [Min(0)] public int m_PreferredHeight;

    [Min(0)] public float m_MainButtonSize;
    public float m_MainButtonPosY;

    [Min(0)] public float m_AutobuyButtonSize;
    public float m_AutobuyButtonPosY;

    void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        GameObject currPanel = null;

        foreach (PersistentGlobals.PersistentData pd in m_PersistentDisplayContainer.m_DataDescriptions)
        {
            currPanel = Instantiate(m_HorizontalBlankPrefab, m_PanelContent.transform);
        }
    }
}
