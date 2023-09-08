using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrencyPanel : MonoBehaviour
{
    public GameObject FindChildWithName(GameObject parent, string name)
    {
        GameObject res = null;
        foreach (Transform child in parent.transform)
        {

            if (child.name == name)
            {
                res = child.gameObject;
                break;
            }

            else
            {
                res = FindChildWithName(child.gameObject, name);
            }
        }
        return res;
    }
    public enum Side
    {
        Left, Right
    }

    // button, background, all purchaseable upgrades
    // all purchaseable upgrades will be children of the background object
    public GameObject m_Panel;
    private RectTransform m_PanelBG;
    private RectTransform m_Button;

    public GameObject m_VerticalLayout;

    public GameObject m_HorizPanel;
    public GameObject m_UpgradePrefab;

    // which side the panel appears (either left or right)
    public Side m_Placement;

    // whether or not this panel is visible
    public bool m_Visible;

    // whether or not this panel is currently opened
    private bool m_Opened = false;

    public void Awake()
    {
        // Get button and Panel BG
        m_PanelBG = FindChildWithName(gameObject, "Panel BG").GetComponent<RectTransform>();
        m_Button = FindChildWithName(gameObject, "Button").GetComponent<RectTransform>();
    }

    public void Initialize(int w, int h, int iter, Side s = Side.Left)
    {
        m_Placement = s;

        // Resize Panel BG to appropriate size
        m_PanelBG.sizeDelta = new Vector2(w / 4, h);

        int btnWidth = Mathf.RoundToInt(m_Button.rect.width);
        int btnHeight = Mathf.RoundToInt(m_Button.rect.height);
        int multiplier = s == Side.Left ? -1 : 1;

        // Move entire panel to appropriate side
        transform.localPosition = new Vector3(Mathf.Abs((w - btnWidth) / 2) * multiplier,
           (1 - (iter % 3)) * (h - btnHeight) / 2, transform.localPosition.z);

        // Reposition Panel BG to correct location
        // y-position must always be 0 to align with screen
        m_PanelBG.transform.localPosition = new Vector3((btnWidth + Mathf.Abs((m_PanelBG.rect.width - btnWidth) / 2)) * multiplier, 
            -transform.localPosition.y, m_PanelBG.transform.localPosition.z);

        CreateUpgrades();
    }

    public void CreateUpgrades()
    {
        //GameObject horizPanel = Instantiate(m_HorizPanel);
        //horizPanel.transform.SetParent(m_VerticalLayout.transform);
        ////horizPanel.GetComponent<RectTransform>().anchoredPosition = m_VerticalLayout.GetComponent<RectTransform>().position;
        ////horizPanel.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0);
        ////horizPanel.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 1);
        ////horizPanel.GetComponent<RectTransform>().pivot = new Vector2(1, 1);
        ////horizPanel.GetComponent<RectTransform>().sizeDelta = m_VerticalLayout.GetComponent<RectTransform>().rect.size;
        //
        //GameObject newButton = Instantiate(m_UpgradePrefab) as GameObject;
        //newButton.transform.SetParent(horizPanel.transform);
    }

    public void Toggle()
    {
        transform.SetAsLastSibling();

        m_Opened = !m_Opened;
        m_PanelBG.gameObject.SetActive(m_Opened);

        int dir = m_Opened ? 1 : -1;
        int multiplier = m_Placement == Side.Left ? 1 : -1;

        transform.localPosition = new Vector3((multiplier * (dir * m_PanelBG.rect.width)) + transform.localPosition.x,
                                    transform.localPosition.y, transform.localPosition.z);
    }
}
