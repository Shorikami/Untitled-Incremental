using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField]
    private float m_Sensitivity = 300.0f;

    private float m_RotX, m_RotY;

    [SerializeField]
    private Transform m_Target;

    [SerializeField]
    private PlayerController m_PlayerController;

    [SerializeField]
    private Transform m_TargetOrientation;

    [SerializeField]
    private float m_DistFromTarget = 3.0f;

    [SerializeField]
    private float m_MaxDistFromTarget = 15.0f;

    private Vector3 m_CurrRot;
    private Vector3 m_SmoothingVelocity = Vector3.zero;

    [SerializeField]
    private float m_SmoothingTime = 0.1f;

    void Start()
    {
        
    }

    void LateUpdate()
    {
        bool inFirstPerson = Mathf.Approximately(m_DistFromTarget, 0.0f) ? true : false;
        RotateCameraAndTarget(inFirstPerson);
        ZoomCamera();

        Debug.Log(inFirstPerson);
    }

    private void ZoomCamera()
    {
        // Windows has a built-in hard value for mouse scrolling. This is a fix to clamp the value as either -1, 0, or 1
        float readIn = m_PlayerController.MouseScroll.ReadValue<float>();
        float absScroll = Mathf.Abs(readIn);
        float res = !Mathf.Approximately(readIn, 0.0f) ? (readIn / absScroll) : 0.0f;

        if (Mathf.Approximately(res, 0.0f))
            return;

        // (m_DistFromTarget - res) to invert scrolling
        m_DistFromTarget = Mathf.Max(0.0f, Mathf.Min(m_MaxDistFromTarget, m_DistFromTarget - res));
    }

    private void RotateCameraAndTarget(bool firstPerson)
    {

        float mX = Input.GetAxis("Mouse X") * m_Sensitivity * Time.deltaTime;
        float mY = Input.GetAxis("Mouse Y") * m_Sensitivity * Time.deltaTime;

        m_RotY += mX;
        m_RotX -= mY;



        m_RotX = Mathf.Clamp(m_RotX, -80.0f, 80.0f);

        Vector3 nextRot = new Vector3(m_RotX, m_RotY);
        m_CurrRot = Vector3.SmoothDamp(m_CurrRot, nextRot, ref m_SmoothingVelocity, m_SmoothingTime);

        if (firstPerson || (!firstPerson && m_PlayerController.RightClick.ReadValue<float>() > 0.0f))
        {
            transform.localEulerAngles = m_CurrRot;
            
        }

        if (firstPerson)
            m_TargetOrientation.rotation = Quaternion.Euler(0.0f, m_RotY, 0.0f);

        transform.position = m_Target.position - transform.forward * m_DistFromTarget;
    }
}
