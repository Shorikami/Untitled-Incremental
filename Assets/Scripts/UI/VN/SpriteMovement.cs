using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteMovement : MonoBehaviour
{
    // 16:9
    private float xEdge = 30.0f, yEdge = 16.875f;

    public GameObject m_LayoutBack;
    public GameObject m_PortraitBase;

    private float m_MoveTime;
    private float m_ElapsedMoveTime;

    private Vector3 m_StartPosition;
    private Vector3 m_TargetPosition;
    private Vector3 m_PositionBeforeZoom;

    private float m_OrigX, m_OrigY;

    private float m_SpeedX = 0.0f, m_ShakeTimeX = 0.0f, m_AmpX = 0.0f;
    private float m_SpeedY = 0.0f, m_ShakeTimeY = 0.0f, m_AmpY = 0.0f;
    private float m_ElapsedShakeTimeX;
    private float m_ElapsedShakeTimeY;

    private bool m_Fading;
    private float m_ElapsedFade;
    private float m_ElapsedFadePost;
    private VNHandler.FADE_EFFECT m_FadeEffect;

    private bool m_Moving = false;
    public bool Moving { get { return m_Moving; } set { m_Moving = value; } }

    private bool m_Shaking = false;
    public bool Shaking { get { return m_Shaking; } set { m_Shaking = value; } }


    void Update()
    {
        if (m_Moving)
        {
            m_ElapsedMoveTime += Time.deltaTime;

            // Not correct speed but creates a nice smoothing effect. Maybe use somewhere else?
            //m_PortraitBase.transform.localPosition = Vector3.Lerp(m_PortraitBase.transform.localPosition, m_TargetPosition, m_ElapsedTime / m_MoveTime);

            m_PortraitBase.transform.localPosition = Vector3.Lerp(m_StartPosition, m_TargetPosition, m_ElapsedMoveTime / m_MoveTime);

            if (Mathf.Abs(m_TargetPosition.x - transform.localPosition.x) <= 0.01f)
            {
                m_PortraitBase.transform.localPosition = m_TargetPosition;
                m_TargetPosition = new Vector3(0.0f, 0.0f, 0.0f);
                m_ElapsedMoveTime = 0.0f;
                m_Moving = false;

                // Switch flags if marked to wait (moving is an action)
                if (VNHandler.m_Instance.Waiting && VNHandler.m_Instance.WaitByAction)
                {
                    VNHandler.m_Instance.Waiting = false;
                    VNHandler.m_Instance.WaitFlag = true;
                    VNHandler.m_Instance.WaitByAction = false;
                }
            }
        }

        if (m_Shaking)
        {
            if (m_ShakeTimeX > 0.0f)
            {
                m_ElapsedShakeTimeX += Time.deltaTime;
                float currShakeX = Mathf.Sin(Mathf.PI * m_SpeedX * m_ElapsedShakeTimeX) * m_AmpX + m_OrigX;

                m_PortraitBase.transform.localPosition = new Vector3(currShakeX, m_PortraitBase.transform.localPosition.y, m_PortraitBase.transform.localPosition.z);

                if (m_ElapsedShakeTimeX >= m_ShakeTimeX)
                {
                    //m_PortraitBase.transform.localPosition = new Vector3(m_OrigX, m_OrigY, m_PortraitBase.transform.localPosition.z);
                    m_ElapsedShakeTimeY = m_ElapsedShakeTimeX = 0.0f;
                    m_Shaking = false;
                    m_ShakeTimeX = 0.0f;

                    // Switch flags if marked to wait (shaking is an action)
                    if (VNHandler.m_Instance.Waiting && VNHandler.m_Instance.WaitByAction)
                    {
                        VNHandler.m_Instance.Waiting = false;
                        VNHandler.m_Instance.WaitFlag = true;
                        VNHandler.m_Instance.WaitByAction = false;
                    }
                }
            }

            if (m_ShakeTimeY > 0.0f)
            {
                m_ElapsedShakeTimeY += Time.deltaTime;
                float currShakeY = Mathf.Sin(Mathf.PI * m_SpeedY * m_ElapsedShakeTimeY) * m_AmpY + m_OrigY;

                m_PortraitBase.transform.localPosition = new Vector3(m_PortraitBase.transform.localPosition.x, currShakeY, m_PortraitBase.transform.localPosition.z);

                if (m_ElapsedShakeTimeY >= m_ShakeTimeY)
                {
                    //m_PortraitBase.transform.localPosition = new Vector3(m_OrigX, m_OrigY, m_PortraitBase.transform.localPosition.z);
                    m_ElapsedShakeTimeY = m_ElapsedShakeTimeX = 0.0f;
                    m_Shaking = false;
                    m_ShakeTimeY = 0.0f;

                    // Switch flags if marked to wait (shaking is an action)
                    if (VNHandler.m_Instance.Waiting && VNHandler.m_Instance.WaitByAction)
                    {
                        VNHandler.m_Instance.Waiting = false;
                        VNHandler.m_Instance.WaitFlag = true;
                        VNHandler.m_Instance.WaitByAction = false;
                    }
                }
            }
        }

        if (m_Fading)
        {
            if (m_ElapsedFade <= 0.5f)
            {
                m_ElapsedFade += Time.deltaTime;

                float c;

                if (m_FadeEffect == VNHandler.FADE_EFFECT.FADE_Out)
                    c = 1.0f - (m_ElapsedFade / 0.5f);

                else
                    c = (m_ElapsedFade / 0.5f);

                m_PortraitBase.GetComponent<Portrait>().m_SprBody.color = new Color(c, c, c, 1.0f);
                m_PortraitBase.GetComponent<Portrait>().m_SprFace.color = new Color(c, c, c, 1.0f);
                m_PortraitBase.GetComponent<Portrait>().m_SprHalo.color = new Color(c, c, c, 1.0f);
            }

            else if (m_ElapsedFade > 0.5f)
            {
                if (m_FadeEffect == VNHandler.FADE_EFFECT.FADE_Out)
                {
                    m_ElapsedFadePost += Time.deltaTime;

                    float c;

                    if (m_FadeEffect == VNHandler.FADE_EFFECT.FADE_Out)
                    {
                        c = 1.0f - (m_ElapsedFadePost / 0.2f);

                        m_PortraitBase.GetComponent<Portrait>().m_SprBody.color = new Color(0.0f, 0.0f, 0.0f, c);
                        m_PortraitBase.GetComponent<Portrait>().m_SprFace.color = new Color(0.0f, 0.0f, 0.0f, c);
                        m_PortraitBase.GetComponent<Portrait>().m_SprHalo.color = new Color(0.0f, 0.0f, 0.0f, c);
                    }

                    if (m_ElapsedFadePost > 0.2f)
                    {
                        EndFade();
                    }
                    return;
                }

                EndFade();
            }
        }
    }

    private float GetXEdge()
    {
        Rect rect = m_LayoutBack.GetComponent<RectTransform>().rect;
        return (rect.width / 2.0f) / xEdge;
    }

    private float GetYEdge()
    {
        Rect rect = m_LayoutBack.GetComponent<RectTransform>().rect;
        return (rect.height / 2.0f) / yEdge;
    }

    private void EndFade()
    {
        m_ElapsedFade = 0.0f;
        m_ElapsedFadePost = 0.0f;

        m_Fading = false;

        // Switch flags - this action causes waiting by default
        if (VNHandler.m_Instance.Waiting && VNHandler.m_Instance.WaitByAction)
        {
            VNHandler.m_Instance.Waiting = false;
            VNHandler.m_Instance.WaitFlag = true;
            VNHandler.m_Instance.WaitByAction = false;
        }
    }

    public void SetPortraitPosition(Vector3 moveTo)
    {
        // GEOMETRY DASH REREFERENCE!!!!!!!!!!!!!!!!!!
        float xStep = GetXEdge();

        m_PortraitBase.transform.localPosition = new Vector3(xStep * moveTo.x, moveTo.y, moveTo.z);
    }

    public void MovePortrait(string axis, float pos, float time)
    {
        m_Moving = true;
        m_StartPosition = m_PortraitBase.transform.localPosition;
        m_TargetPosition = string.Compare(axis, "x") == 0 ? new Vector3(GetXEdge() * pos, 0, 0) : new Vector3(0, pos, 0);
        m_MoveTime = time;
    }

    public void ShakePortrait(string axis, float speed, float time, float amp)
    {
        m_Shaking = true;

        if (string.Compare(axis, "x") == 0)
        {
            m_ShakeTimeX = time;
            m_AmpX = amp * GetXEdge();
            m_SpeedX = speed;
        }
        else
        {
            m_ShakeTimeY = time;
            m_AmpY = amp * GetYEdge();
            m_SpeedY = speed;
        }

        m_OrigX = m_PortraitBase.transform.localPosition.x;
        m_OrigY = m_PortraitBase.transform.localPosition.y;
        m_StartPosition = m_PortraitBase.transform.localPosition;
    }

    public void CloseUp()
    {
        m_PortraitBase.transform.localScale = new Vector3(1.5f, 1.5f, 1.0f);
        m_PositionBeforeZoom = m_PortraitBase.transform.localPosition;
        m_PortraitBase.transform.localPosition = new Vector3(m_PortraitBase.transform.localPosition.x, GetYEdge() * -10.0f, m_PortraitBase.transform.localPosition.z);
    }

    public void MoveBack()
    {
        m_PortraitBase.transform.localScale = Vector3.one;
        m_PortraitBase.transform.localPosition = m_PositionBeforeZoom;
    }

    public void Show(bool instant = false)
    {
        if (!instant)
        {
            m_Fading = true;
            m_FadeEffect = VNHandler.FADE_EFFECT.FADE_In;
            return;
        }

        m_PortraitBase.GetComponent<Portrait>().m_SprBody.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        m_PortraitBase.GetComponent<Portrait>().m_SprFace.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        m_PortraitBase.GetComponent<Portrait>().m_SprHalo.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    }

    public void Hide(bool instant = false)
    {
        if (!instant)
        {
            m_Fading = true;
            m_FadeEffect = VNHandler.FADE_EFFECT.FADE_Out;
            return;
        }

        m_PortraitBase.GetComponent<Portrait>().m_SprBody.color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
        m_PortraitBase.GetComponent<Portrait>().m_SprFace.color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
        m_PortraitBase.GetComponent<Portrait>().m_SprHalo.color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
    }

    public void Highlight(float val)
    {
        float c = Mathf.Min(Mathf.Max(0, val), 1.0f);
        m_PortraitBase.GetComponent<Portrait>().m_SprBody.color = new Color(c, c, c, 1.0f);
        m_PortraitBase.GetComponent<Portrait>().m_SprFace.color = new Color(c, c, c, 1.0f);
        m_PortraitBase.GetComponent<Portrait>().m_SprHalo.color = new Color(c, c, c, 1.0f);
    }
}
