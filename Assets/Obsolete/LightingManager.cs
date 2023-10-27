using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class LightingManager : MonoBehaviour
{
    [SerializeField] private Light m_DirectionalLight;
    [SerializeField] private LightingConditions m_LightConditions;
    [SerializeField, Range(0, 24)] private float m_TimeOfDay;
    [SerializeField] private float m_TimeMultiplier;

    private void Update()
    {
        if (m_LightConditions == null)
            return;

        if (Application.isPlaying)
        {
            m_TimeOfDay += Time.deltaTime * m_TimeMultiplier;
            m_TimeOfDay %= 24.0f;
            UpdateLighting(m_TimeOfDay / 24.0f);
        }
        else
        {
            UpdateLighting(m_TimeOfDay / 24.0f);
        }
    }

    private void UpdateLighting(float time)
    {
        //RenderSettings.ambientLight = m_LightConditions.m_AmbientColor.Evaluate(time);
        //RenderSettings.fogColor = m_LightConditions.m_FogColor.Evaluate(time);

        if (m_DirectionalLight != null)
        {
            //m_DirectionalLight.color = m_LightConditions.m_DirectionalColor.Evaluate(time);
            m_DirectionalLight.transform.localRotation = Quaternion.Euler(new Vector3((time * 360.0f) - 90.0f, 170.0f, 0.0f));
        }
    }

    private void OnValidate()
    {
        if (m_DirectionalLight != null)
            return;

        if (RenderSettings.sun != null)
            m_DirectionalLight = RenderSettings.sun;

        else
        {
            Light[] lights = FindObjectsOfType<Light>();
            foreach (Light l in lights)
            {
                if (l.type == LightType.Directional)
                {
                    m_DirectionalLight = l;
                    return;
                }
            }
        }
    }
}
