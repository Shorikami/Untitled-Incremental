using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoiceButton : MonoBehaviour
{
    public string m_Target;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void DeactivateChoicePause()
    {
        VNHandler.m_Instance.m_FindTargetLine = true;
        VNHandler.m_Instance.m_TargetLine = m_Target;
    }
}
