using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public float m_MouseSens = 100.0f;
    private float m_RotationX = 0.0f;

    float m_RotX;
    float m_RotY;

    // TODO: Automatically assign this by finding player
    public Transform m_Orientation;

    private void Start()
    {
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
    }

    private void Update()
    {
        float mX = Input.GetAxis("Mouse X") * m_MouseSens * Time.deltaTime;
        float mY = Input.GetAxis("Mouse Y") * m_MouseSens * Time.deltaTime;

        m_RotY += mX;
        m_RotX -= mY;
        m_RotX = Mathf.Clamp(m_RotX, -90.0f, 90.0f);

        transform.rotation = Quaternion.Euler(m_RotX, m_RotY, 0.0f);
        m_Orientation.rotation = Quaternion.Euler(0.0f, m_RotY, 0.0f);
    }
}
