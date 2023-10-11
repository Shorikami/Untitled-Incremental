using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BoardLock : MonoBehaviour
{
    public TextMeshProUGUI m_LockedText;

    public void UpdateText(int lvl)
    {
        m_LockedText.text = "Unlocks at Level " + lvl.ToString();
    }
}
