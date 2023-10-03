using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMover : MonoBehaviour
{
    // TODO: Automatically assign this by finding player
    public Transform m_CamPosition;

    private void Update()
    {
        transform.position = m_CamPosition.position;
    }
}
