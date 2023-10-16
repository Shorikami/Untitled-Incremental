using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCInteractable : Interactable
{
    public override void OnFocus()
    {
        Debug.Log(gameObject.name);
    }

    public override void OnInteract()
    {

    }

    public override void OnLoseFocus()
    {
        // this can only happen if the player is in first person
        StatsManager.m_Instance.Player.GetComponent<PlayerController>().PlayerUI.DisplayNPCPrompt(false);
    }
}
