using System;
using UnityEngine;

public class TimeController : MonoBehaviour
{
    [SerializeField] private float m_TimeMultiplier;
    [SerializeField] private float m_StartHour;

    [SerializeField] private Light m_SunsLight;
    [SerializeField] private float m_SunriseHour;
    [SerializeField] private float m_SunsetHour;

    [SerializeField] private float m_MaxSunIntensity;

    [SerializeField] private Light m_Moonlight;
    [SerializeField] private float m_MaxMoonIntensity;

    [SerializeField] private Color m_DayAmbient;
    [SerializeField] private Color m_NightAmbient;

    [SerializeField] private AnimationCurve m_LightCurve;

    private DateTime m_CurrTime;
    private TimeSpan m_SunriseTime;
    private TimeSpan m_SunsetTime;

    void Start()
    {
        m_CurrTime = DateTime.Now.Date + TimeSpan.FromHours(m_StartHour);

        m_SunriseTime = TimeSpan.FromHours(m_SunriseHour);
        m_SunsetTime = TimeSpan.FromHours(m_SunsetHour);
    }

    void Update()
    {
        UpdateTime();
        RotateSun();
        UpdateLightSettings();
    }

    void UpdateTime()
    {
        m_CurrTime = m_CurrTime.AddSeconds(Time.deltaTime * m_TimeMultiplier);
    }

    void UpdateLightSettings()
    {
        float dotP = Vector3.Dot(m_SunsLight.transform.forward, Vector3.down);
        m_SunsLight.intensity = Mathf.Lerp(0.0f, m_MaxSunIntensity, m_LightCurve.Evaluate(dotP));
        m_Moonlight.intensity = Mathf.Lerp(m_MaxMoonIntensity, 0.0f, m_LightCurve.Evaluate(dotP));
        RenderSettings.ambientLight = Color.Lerp(m_NightAmbient, m_DayAmbient, m_LightCurve.Evaluate(dotP));
    }

    void RotateSun()
    {
        float sunRot = 0.0f;

        if (m_CurrTime.TimeOfDay > m_SunriseTime && m_CurrTime.TimeOfDay < m_SunsetTime)
        {
            TimeSpan sunriseToSunset = CalcTimeDifference(m_SunriseTime, m_SunsetTime);
            TimeSpan timeSinceSunrise = CalcTimeDifference(m_SunriseTime, m_CurrTime.TimeOfDay);

            double percentage = timeSinceSunrise.TotalMinutes / sunriseToSunset.TotalMinutes;

            sunRot = Mathf.Lerp(0.0f, 180.0f, (float)percentage);
        }

        else
        {
            TimeSpan sunsetToSunrise = CalcTimeDifference(m_SunsetTime, m_SunriseTime);
            TimeSpan timeSinceSunset = CalcTimeDifference(m_SunsetTime, m_CurrTime.TimeOfDay);

            double percentage = timeSinceSunset.TotalMinutes / sunsetToSunrise.TotalMinutes;

            sunRot = Mathf.Lerp(180.0f, 360.0f, (float)percentage);
        }

        m_SunsLight.transform.rotation = Quaternion.AngleAxis(sunRot, Vector3.right);
    }

    TimeSpan CalcTimeDifference(TimeSpan from, TimeSpan to)
    {
        TimeSpan diff = to - from;

        if (diff.TotalSeconds < 0)
        {
            diff += TimeSpan.FromHours(24);
        }

        return diff;
    }
}
