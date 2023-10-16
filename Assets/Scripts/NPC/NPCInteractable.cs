using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCInteractable : Interactable
{
    [SerializeField] private NPCController m_OwnerNPC;

    public override void OnFocus()
    {
    }

    public override void OnInteract()
    {

    }

    public override void OnLoseFocus()
    {
        // this can only happen if the player is in first person
        StatsManager.m_Instance.Player.GetComponent<PlayerController>().PlayerUI.DisplayNPCPrompt(false, null);
    }
}
