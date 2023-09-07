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

    // which side the panel appears (either left or right)
    public Side m_Placement;

    // whether or not this panel is visible
    public bool m_Visible;

    public void Initialize(int w, int h, Side s = Side.Left)
    {
        // Get button and Panel BG
        RectTransform panelRect = FindChildWithName(gameObject, "Panel BG").GetComponent<RectTransform>();
        Rect button = FindChildWithName(gameObject, "Button").GetComponent<RectTransform>().rect;

        // Resize Panel BG to appropriate size
        panelRect.sizeDelta = new Vector2(w / 4, h);

        int btnWidth = Mathf.RoundToInt(button.width);
        int btnHeight = Mathf.RoundToInt(button.height);
        int multiplier = s == Side.Left ? -1 : 1;

        // Move entire panel to appropriate side
        transform.localPosition = new Vector3(Mathf.Abs((w - btnWidth) / 2) * multiplier, (h - btnHeight) / 2, -10);

        // Reposition Panel BG to correct location
        // y-position must always be 0 to align with screen
        panelRect.transform.localPosition = new Vector3((btnWidth + Mathf.Abs((panelRect.rect.width - btnWidth) / 2)) * multiplier, -transform.localPosition.y, -10);
    }

    public void Test()
    {
        Debug.Log("a");
    }
}
