using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCTriggerRange : MonoBehaviour
{
    [SerializeField] private NPCController m_OwnerNPC;

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "PlayerBody")
            other.gameObject.GetComponentInParent<PlayerController>().PlayerUI.DisplayNPCPrompt(true, m_OwnerNPC);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "PlayerBody")
            other.gameObject.GetComponentInParent<PlayerController>().PlayerUI.DisplayNPCPrompt(false, null);
    }
}
