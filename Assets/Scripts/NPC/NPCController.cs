using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour
{
    
    [Tooltip("This should be the same name as the portrait this NPC is intended to be in the text file.")] 
    public string m_NPCName;

    [SerializeField] private Portrait m_NPCPortrait;
    public Portrait NPCPortrait
    {
        get { return m_NPCPortrait; }
        set { m_NPCPortrait = value; }
    }

    [SerializeField] private GameObject m_InteractionRange;

    public List<string> m_VNEvents = new List<string>();
    [Min(0)] public int m_VNEventIdx;

    // Start is called before the first frame update
    void Start()
    {
        m_VNEventIdx = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ActivateEvent()
    {
        VNHandler.m_Instance.LoadTextFile(m_VNEvents[m_VNEventIdx], this);
    }
}