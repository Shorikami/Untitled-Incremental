using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AudioState { None, Play, Clip, Container }
public class AudioPlayer : MonoBehaviour
{
    public AudioSource m_Source;
    public AudioClip m_Clip;

    public AudioState m_State;

    public int contIdx = -1;
    public AudioContainer m_AudioCont;

    void Update()
    {
        switch (m_State)
        {
            case AudioState.Play:
                if (!m_Source.isPlaying)
                {
                    m_Source.Play();
                    m_State = AudioState.None;
                }
                break;

            case AudioState.Clip:
                m_Source.PlayOneShot(m_Clip);
                m_State = AudioState.None;
                break;

            case AudioState.Container:
                m_Source.PlayOneShot(m_AudioCont.GetSound(contIdx));
                m_State = AudioState.None;
                break;
        }
    }
}
