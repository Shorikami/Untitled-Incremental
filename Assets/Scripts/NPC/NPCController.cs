using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour
{
    
    [Tooltip("This should be the same name as the portrait this NPC is intended to be in the text file.")] 
    public string m_NPCName;

    [SerializeField] private Transform m_NPC;
    [SerializeField] private Portrait m_NPCPortrait;
    public Portrait NPCPortrait
    {
        get { return m_NPCPortrait; }
        set { m_NPCPortrait = value; }
    }

    [SerializeField] private GameObject m_InteractionRange;

    public List<string> m_VNEvents = new List<string>();
    [Min(0)] public int m_VNEventIdx;

    private bool m_RotateBeforeEvent;

    // Start is called before the first frame update
    void Start()
    {
        m_VNEventIdx = 0;
        m_RotateBeforeEvent = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_RotateBeforeEvent)
        {
            VNHandler.m_Instance.CutsceneActive = true;

            // has to be done separately since the NPC should rotate to the player
            // before the player rotates
            if (DoneRotatingNPCToPlayer())
            {
                if (DoneRotatingPlayerToNPC())
                {
                    m_RotateBeforeEvent = false;
                    ActivateEvent();
                }
            }

        }
    }

    private bool DoneRotatingNPCToPlayer()
    {
        Vector3 targetDir = (StatsManager.m_Instance.Player.transform.position - m_NPC.position).normalized;
        Quaternion targetRot = Quaternion.LookRotation(targetDir);
        m_NPC.rotation = Quaternion.Slerp(m_NPC.rotation, targetRot, Time.deltaTime * 10.0f);

        return m_NPC.rotation == targetRot;
    }

    private bool DoneRotatingPlayerToNPC()
    {
        Quaternion targetRot = Quaternion.LookRotation(-m_NPC.forward);
        Transform targetTransf = StatsManager.m_Instance.Player.transform;
        Transform cameraTransf = StatsManager.m_Instance.Player.GetComponent<PlayerController>().Camera.transform;

        targetTransf.rotation = Quaternion.Slerp(targetTransf.rotation, targetRot, Time.deltaTime * 10.0f);
        cameraTransf.rotation = Quaternion.Slerp(cameraTransf.rotation, targetRot, Time.deltaTime * 10.0f);

        return targetTransf.rotation == targetRot && cameraTransf.rotation == targetRot;
    }

    public void ActivateEvent()
    {
        if (!VNHandler.m_Instance.CutsceneActive)
        {
            m_RotateBeforeEvent = true;
            return;
        }

        VNHandler.m_Instance.LoadTextFile(m_VNEvents[m_VNEventIdx], this);
    }
}
