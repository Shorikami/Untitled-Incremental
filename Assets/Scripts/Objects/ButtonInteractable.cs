using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonInteractable : Interactable
{
    public override void OnFocus()
    {
        Debug.Log("Looking at: " + gameObject.name);
    }

    public override void OnInteract()
    {
        GetComponent<Button>().onClick.Invoke();
        Debug.Log("Interacted with: " + gameObject.name);
    }

    public override void OnLoseFocus()
    {
        Debug.Log("Looked away from: " + gameObject.name);
    }

    public void Test()
    {
        Debug.Log("I've been clicked!");
    }
}
