using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour
{
    [SerializeField] private Portrait m_NPCPortrait;
    public Portrait NPCPotrait
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
}
