using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Light Conditions", menuName = "Scriptables/Lighting Conditions", order=1)]
public class LightingConditions : ScriptableObject
{
    public Gradient m_AmbientColor;
    public Gradient m_DirectionalColor;
    public Gradient m_FogColor;
}
