using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    public GameObject m_Scroll;
   

    private RectTransform m_PanelBG;
    private RectTransform m_Button;

    public GameObject m_PanelPrefab;
    public GameObject m_UpgradePrefab;
    public GameObject m_HorizPanelPrefab;

    private List<GameObject> m_Panels = new List<GameObject>();

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

    public void CreateUpgrades(int spacing = 30, int topPadding = 50, int bottomPadding = 50)
    {
        m_Scroll.GetComponent<RectTransform>().sizeDelta = new Vector2(Mathf.Abs(m_PanelBG.rect.width), m_PanelBG.rect.height);

        m_Scroll.GetComponent<ScrollRect>().content.GetComponent<VerticalLayoutGroup>().spacing = spacing;
        m_Scroll.GetComponent<ScrollRect>().content.GetComponent<VerticalLayoutGroup>().padding.top = topPadding;
        m_Scroll.GetComponent<ScrollRect>().content.GetComponent<VerticalLayoutGroup>().padding.bottom = bottomPadding;

        // Modify scroll rectangle's "content" panel
        m_Scroll.GetComponent<ScrollRect>().content.sizeDelta = new Vector2(Mathf.Abs(m_PanelBG.rect.width), m_PanelBG.rect.height);
        m_Scroll.GetComponent<ScrollRect>().content.anchorMin = new Vector2(0, 1);
        m_Scroll.GetComponent<ScrollRect>().content.anchorMax = new Vector2(0, 1);
        m_Scroll.GetComponent<ScrollRect>().content.pivot = new Vector2(0, 1);

        // Dynamically create layout grouping
        //DynamicLayout layout = m_Scroll.GetComponent<ScrollRect>().content.gameObject.AddComponent<DynamicLayout>();
        //DynamicLayout layout = m_Scroll.GetComponent<ScrollRect>().content.GetComponent<DynamicLayout>();
        //layout.m_FitType = DynamicLayout.FitType.FixedColumns;
        //layout.m_Columns = 1;
        //layout.fitX = layout.fitY = true;

        // Add the panels
        // TODO: Add different panels based on currency (i.e. coins include automation but chroma does not, etc.)
        CreatePanelImage("destiny2");
        CreateHorizontalPanels(3, 2);
        CreateEmptySpace(300);
        CreatePanelImage("destiny2");
        CreateHorizontalPanels();

        //m_Panels.Add(Instantiate(m_HorizPanelPrefab, m_Scroll.GetComponent<ScrollRect>().content.transform));

        // for each panel, add the appropriate upgrades
        //foreach (GameObject panel in m_Panels)
        //{
        //    Instantiate(m_UpgradePrefab, panel.transform);
        //}
    }

    // dynamically create image child
    private void CreatePanelImage(string path = null)
    {
        GameObject newImage = new GameObject("Upgrade Image");

        RectTransform rT = newImage.AddComponent<RectTransform>();
        rT.transform.SetParent(m_Scroll.GetComponent<ScrollRect>().content);
        rT.localScale = Vector3.one;
        rT.anchoredPosition = new Vector2(0, 0);
        rT.sizeDelta = new Vector2(150, 200); // <--- I don't think this matters? It's edited when the actual image appears

        Image img = newImage.AddComponent<Image>();
        Texture2D tex = Resources.Load<Texture2D>("Textures/" + path);
        img.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        img.transform.SetParent(m_Scroll.GetComponent<ScrollRect>().content);
    }

    // dynamically create empty space
    private void CreateEmptySpace(int height = 100)
    {
        GameObject newImage = new GameObject("Empty Panel Space");

        // regular transform -> rectangle transform (ui element)
        RectTransform rT = newImage.AddComponent<RectTransform>();
        rT.transform.SetParent(m_Scroll.GetComponent<ScrollRect>().content);
        rT.localScale = Vector3.one;
        rT.anchoredPosition = new Vector2(0, 0);
        rT.sizeDelta = new Vector2(150, height);

        // "empty" image (it's invisible)
        Image img = newImage.AddComponent<Image>();
        img.color = new Color(0, 0, 0, 0);
        img.transform.SetParent(m_Scroll.GetComponent<ScrollRect>().content);

        // ignores forced height scaling of layouts
        LayoutElement lE = newImage.AddComponent<LayoutElement>();
        lE.minHeight = height;
        lE.transform.SetParent(m_Scroll.GetComponent<ScrollRect>().content);
    }

    private void CreateHorizontalPanels(int numPanels = 1, int numUpgrades = 2)
    {
        for (int i = 0; i < numPanels; ++i)
        {
            GameObject newHorizPanel = Instantiate(m_HorizPanelPrefab, m_Scroll.GetComponent<ScrollRect>().content.transform);

            for (int j = 0; j < numUpgrades; ++j)
            {
                Instantiate(m_UpgradePrefab, newHorizPanel.transform);
            }

            m_Panels.Add(newHorizPanel);
        }
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
