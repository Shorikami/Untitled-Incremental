using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Audio Container", menuName = "Audio/Container")]
public class AudioContainer : ScriptableObject
{
    public AudioClip[] m_Sounds;

    // If range is not specified, return a random index
    public AudioClip GetSound(int idx = -1)
    {
        switch (idx)
        {
            case -1:
                return m_Sounds[Random.Range(0, m_Sounds.Length)];

            default:
                return m_Sounds[idx];
        }
    }

    public void PlayAll(AudioSource src)
    {
        foreach (AudioClip a in m_Sounds)
        {
            src.PlayOneShot(a);
        }
    }
}
