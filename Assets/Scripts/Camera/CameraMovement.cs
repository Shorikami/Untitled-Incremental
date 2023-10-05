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
    [Min(0)]
    private float m_DistFromTarget = 3.0f;

    [SerializeField]
    [Range(1, 15)]
    private float m_MaxDistFromTarget = 15.0f;

    private Vector3 m_CurrRot;
    private Vector3 m_SmoothingVelocity = Vector3.zero;
    private Vector3 m_LastClickedPosition = Vector3.zero;

    [SerializeField]
    private float m_SmoothingTime = 0.1f;

    [SerializeField]
    private float m_MinMaxViewAngle = 65.0f;

    public bool m_InFirstPerson = false;

    void Start()
    {
        
    }

    void Update()
    {
        m_InFirstPerson = Mathf.Approximately(m_DistFromTarget, 0.0f) ? true : false;
        Cursor.lockState = m_InFirstPerson ? CursorLockMode.Locked : CursorLockMode.None;
        //Cursor.visible = true;
    }
    

    void LateUpdate()
    {
        RotateCameraAndTarget(m_InFirstPerson);
        ZoomCamera();
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

        m_RotX = Mathf.Clamp(m_RotX, -m_MinMaxViewAngle, m_MinMaxViewAngle);

        Vector3 nextRot = new Vector3(m_RotX, m_RotY);
        m_CurrRot = Vector3.SmoothDamp(m_CurrRot, nextRot, ref m_SmoothingVelocity, m_SmoothingTime);

        if (firstPerson)
        {
            transform.localEulerAngles = m_CurrRot;
        }

        // https://www.youtube.com/watch?v=rDJOilo4Xrg
        else
        {
            if (m_PlayerController.RightClick.ReadValue<float>() > 0.0f)
            {
                Camera cam = GetComponent<Camera>();
                if (m_PlayerController.RightClick.triggered)
                    m_LastClickedPosition = cam.ScreenToViewportPoint(m_PlayerController.MousePosition);

                Vector3 currMousePos = cam.ScreenToViewportPoint(m_PlayerController.MousePosition);
                Vector3 direction = m_LastClickedPosition - currMousePos;

                //Debug.Log("LAST Y: " + m_LastClickedPosition.y + " CURR Y: " + currMousePos.y);

                transform.position = new Vector3();

                float verticalAngle = direction.y * 180.0f * m_Sensitivity * Time.deltaTime;

                // These checks are in to prevent a bug where rotating the camera vertically
                // while reaching the max zoom threshold will cause the camera to freak out.
                // the cases are as such
                // - if the X-axis is within the min/max bounds
                // - if the X-axis is on the max threshold and the player is attempting to look down
                // - if the X-axis is on the min threshold and the player is attempting to look up
                // if any of these are true, then the player can move the camera vertically
                if ((!Mathf.Approximately(transform.rotation.eulerAngles.x, m_MinMaxViewAngle) &&
                    !Mathf.Approximately(transform.rotation.eulerAngles.x, 360.0f - m_MinMaxViewAngle)) ||
                    (Mathf.Approximately(transform.rotation.eulerAngles.x, m_MinMaxViewAngle) 
                    && currMousePos.y > m_LastClickedPosition.y) || 
                    (Mathf.Approximately(transform.rotation.eulerAngles.x, 360.0f - m_MinMaxViewAngle)
                    && currMousePos.y < m_LastClickedPosition.y))
                {
                    //Debug.Log("Hit1");
                    transform.Rotate(Vector3.right, verticalAngle);
                }
                    

                transform.Rotate(Vector3.up, -direction.x * 180.0f * m_Sensitivity * Time.deltaTime, Space.World);

                Vector3 camAngle = transform.rotation.eulerAngles;

                camAngle.x = (camAngle.x > 180.0f) ? camAngle.x - 360.0f : camAngle.x;
                camAngle.x = Mathf.Clamp(camAngle.x, -m_MinMaxViewAngle, m_MinMaxViewAngle);

                transform.rotation = Quaternion.Euler(camAngle.x, camAngle.y, 0.0f);

                m_LastClickedPosition = cam.ScreenToViewportPoint(m_PlayerController.MousePosition);
            }
        }

#region Old
        //Debug.Log(transform.rotation.eulerAngles.x);

        //else if (!firstPerson && m_PlayerController.RightClick.ReadValue<float>() > 0.0f)
        //{
        //    ////transform.Rotate(m_TargetOrientation.up, -mX * 5.0f, Space.World);
        //    //float angleX = -mY * m_Sensitivity * Time.deltaTime;
        //    //float angleY = mX * m_Sensitivity * Time.deltaTime;
        //    //
        //    //transform.Rotate(transform.up, angleY, Space.World);
        //    //
        //    ////Debug.Log(angle);
        //    ////Debug.Log("OLD ANGLE: " + transform.eulerAngles.x);
        //    //transform.Rotate(transform.right, angleX, Space.World);
        //    ////Debug.Log("NEW ANGLE: " + transform.eulerAngles.x);
        //    //
        //    //float correctRotX = transform.rotation.eulerAngles.x;
        //    //
        //    //if (transform.rotation.eulerAngles.x > 90.0f)
        //    //{
        //    //    if (transform.rotation.eulerAngles.x + angleX < 360.0f - m_MinMaxViewAngle)
        //    //        correctRotX = 360.0f - m_MinMaxViewAngle;
        //    //
        //    //}
        //    //else if (transform.rotation.eulerAngles.x < 90.0f)
        //    //{
        //    //    if (transform.rotation.eulerAngles.x + angleX > m_MinMaxViewAngle)
        //    //        correctRotX = m_MinMaxViewAngle;
        //    //}
        //    //
        //    //transform.rotation = Quaternion.Euler(correctRotX, transform.eulerAngles.y, 0.0f);
        //
        //    //Vector3 transformEulerAngles = transform.rotation.eulerAngles;
        //    //transformEulerAngles.x = Mathf.Clamp(transformEulerAngles.x, -m_MinMaxViewAngle, m_MinMaxViewAngle);
        //    //Debug.Log(transformEulerAngles.x);
        //    //
        //    //transform.localEulerAngles = new Vector3(transformEulerAngles.x, transform.localEulerAngles.y, transform.localEulerAngles.z);
        //
        //    //transform.rotation = Quaternion.Euler(transformEulerAngles);
        //
        //    //transform.rotation = Quaternion.Euler(m_RotX, 0.0f, 0.0f);
        //}
#endregion Old

        if (firstPerson)
            m_TargetOrientation.rotation = Quaternion.Euler(0.0f, m_RotY, 0.0f);

        transform.position = m_Target.position - transform.forward * m_DistFromTarget;
    }
}
