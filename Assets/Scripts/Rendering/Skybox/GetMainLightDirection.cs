using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GetMainLightDirection : MonoBehaviour
{
    [SerializeField] private Material m_SkyboxMat;

    void Update()
    {
        m_SkyboxMat.SetVector("_Main_Light_Direction", transform.forward);
    }
}
