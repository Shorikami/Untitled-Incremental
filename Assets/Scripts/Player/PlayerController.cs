using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using System.Linq;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private GameObject m_Camera;

    public GameObject Camera
    {
        get { return m_Camera; }
        set { m_Camera = value; }
    }
    [HideInInspector] public bool m_FirstPerson;

    [Header("NPC Interactions")]
    [HideInInspector] public bool m_CanInteractWithNPCs = false;
    [HideInInspector] public NPCController m_CurrNPC;

    [SerializeField] private Transform m_Player;

    [SerializeField] private GameObject m_PrefabPlayerUI;
    private PlayerInterface m_PlayerUI;

    [SerializeField] private GameObject m_PickupRange;
    private Vector2 m_BaseRangePU;

    private List<Upgrade> m_PickupRangeUpgrades;
    private List<Upgrade> m_MoveSpeedUpgrades;

    public PlayerInterface PlayerUI
    { 
        get { return m_PlayerUI; }
        private set { m_PlayerUI = value; }
    }

    InputAction i_LMB;
    InputAction i_RMB;
    InputAction i_MouseWheel;

    public InputAction RightClick
    {
        get { return i_RMB; }
        private set { i_RMB = value; }
    }

    public InputAction MouseScroll
    {
        get { return i_MouseWheel; }
        private set { i_MouseWheel = value; }
    }

    public Transform m_Orientation;

    private Vector2 m_MoveDir;
    private Vector2 m_MousePos;

    public Vector2 MousePosition
    {
        get { return m_MousePos; }
        private set { m_MousePos = value; }
    }

    [SerializeField] private Animator m_Animator;

    public float m_BaseSpeed;
    private float m_CurrentSpeed;
    
    [Header("Ground Check")]
    private bool m_Grounded;
    public float m_PlayerHeight;
    public LayerMask m_Ground;
    public float m_GroundDrag;

    [Header("First-Person Interaction")]
    [SerializeField] private Vector3 m_InteractionRayPoint = default;
    [SerializeField] private float m_InteractionDist = default;
    [SerializeField] private LayerMask m_InteractionLayer = default;
    
    private Interactable m_CurrInteractable;
    public Interactable CurrentlyLookingAt
    {
        get { return m_CurrInteractable; }
        private set { m_CurrInteractable = value; }
    }

    

    public Rigidbody m_RB;

    [SerializeField]
    PlayerInput m_PlayerInput;

    public PlayerInput InputScheme
    {
        get { return m_PlayerInput; }
        set { m_PlayerInput = value; }
    }

    private void OnEnable()
    {
        m_PlayerInput.Player.Enable();
    }

    private void OnDisable()
    {
        m_PlayerInput.Player.Disable();
    }

    private void Awake()
    {
        m_PlayerInput = new PlayerInput();
        m_PlayerInput.Player.Movement.performed += Move;
        m_PlayerInput.Player.Movement.canceled += Move;

        m_PlayerInput.Player.MousePosition.performed += MousePos;

        m_PlayerInput.Player.Interact.performed += Interact;

        i_LMB = m_PlayerInput.Player.LMB;
        i_RMB = m_PlayerInput.Player.RMB;
        i_MouseWheel = m_PlayerInput.Player.MouseWheel;

        m_RB = GetComponent<Rigidbody>();
        m_PlayerUI = Instantiate(m_PrefabPlayerUI, transform).GetComponent<PlayerInterface>();
        m_PlayerUI.m_OwnerPlayer = this;
    }

    private void Start()
    {
        m_PickupRangeUpgrades = StatsManager.m_Instance.FindUpgrades(StatsManager.NonCurrencyUpgrades.PickupRange);
        m_MoveSpeedUpgrades = StatsManager.m_Instance.FindUpgrades(StatsManager.NonCurrencyUpgrades.MoveSpeed);

        m_BaseRangePU = new Vector2(m_PickupRange.transform.localScale.x, m_PickupRange.transform.localScale.z);
    }

    private void Update()
    {
        if (VNHandler.m_Instance.CutsceneActive)
            return;

        LimitSpeed();
        GroundCheck();

        if (!m_Camera.GetComponent<CameraMovement>().m_InFirstPerson)
            MouseRaycast();

        if (m_Camera.GetComponent<CameraMovement>().m_InFirstPerson)
        {
            m_Camera.GetComponent<CameraMovement>().m_PreventZoom = false;
            transform.rotation = m_Orientation.rotation;
            HandleInteractionCheck();
            HandleInteractionInput();

            // initial state (do this after interaction check to see if the player isn't already looking at someone)
            if (!m_FirstPerson && CurrentlyLookingAt == null)
                PlayerUI.DisplayNPCPrompt(false, null);

            m_FirstPerson = true;
        }

        UpdateSpeed();
        UpdatePickupRange();
        UpdateAnimationProperties();
    }

    private void FixedUpdate()
    {
        if (VNHandler.m_Instance.CutsceneActive)
            return;

        MovePlayer(m_Camera.transform);
    }

    private void UpdateAnimationProperties()
    {
        m_Animator.SetFloat("Speed", m_RB.velocity.z);
    }

    private void UpdateSpeed()
    {
        float bonus = m_MoveSpeedUpgrades.ConvertAll(x => x.m_UpgradeData.m_UpgradeBonuses.m_CurrentBonus).Aggregate((a, b) => a * b);
        m_CurrentSpeed = m_BaseSpeed * bonus;
    }

    private void UpdatePickupRange()
    {
        float bonus = m_PickupRangeUpgrades.ConvertAll(x => x.m_UpgradeData.m_UpgradeBonuses.m_CurrentBonus).Aggregate((a, b) => a * b);
        m_PickupRange.transform.localScale = new Vector3(m_BaseRangePU.x * bonus, m_PickupRange.transform.localScale.y, m_BaseRangePU.y * bonus);
    }

    private void MouseRaycast()
    {
        m_FirstPerson = false;
        Ray r = m_Camera.GetComponent<Camera>().ScreenPointToRay(m_MousePos);
        if (Physics.Raycast(r, out RaycastHit hit))
        {
            if (hit.collider.gameObject.layer == 8 || hit.collider.gameObject.layer == 7)
                m_Camera.GetComponent<CameraMovement>().m_PreventZoom = true;

            else
                m_Camera.GetComponent<CameraMovement>().m_PreventZoom = false;
        }
    }

    private void HandleInteractionCheck()
    {
        // todo: make this its own variable
        // reverse bit makes it so that it ignores these layers
        int mask = 1 << LayerMask.NameToLayer("Default") | 1 << LayerMask.NameToLayer("Ignore Raycast") |
            1<< LayerMask.NameToLayer("Ground") |  1 << LayerMask.NameToLayer("CameraZoomLock");
        mask = ~mask;

        if (Physics.Raycast(m_Camera.GetComponent<Camera>().ViewportPointToRay(m_InteractionRayPoint), 
            out RaycastHit hit, m_InteractionDist, mask))
        {
            if (hit.collider.gameObject.layer == 7 && 
                (m_CurrInteractable == null || hit.collider.gameObject.GetInstanceID() != m_CurrInteractable.GetInstanceID()))
            {
                hit.collider.TryGetComponent(out m_CurrInteractable);

                if (m_CurrInteractable)
                    m_CurrInteractable.OnFocus();
            }
        }

        else if (m_CurrInteractable)
        {
            m_CurrInteractable.OnLoseFocus();
            m_CurrInteractable = null;
        }
    }

    private void HandleInteractionInput()
    {
        if (i_LMB.triggered && m_CurrInteractable != null && 
            Physics.Raycast(m_Camera.GetComponent<Camera>().ViewportPointToRay(m_InteractionRayPoint), out RaycastHit hit,
            m_InteractionDist, m_InteractionLayer))
        {
            m_CurrInteractable.OnInteract();
        }
    }

    void Move(InputAction.CallbackContext ctx)
    {
        m_MoveDir = ctx.ReadValue<Vector2>();
    }

    void MousePos(InputAction.CallbackContext ctx)
    {
        m_MousePos = ctx.ReadValue<Vector2>();
    }

    void Interact(InputAction.CallbackContext ctx)
    {
        if (m_CanInteractWithNPCs && m_CurrNPC != null)
        {
            if (!VNHandler.m_Instance.CutsceneActive)
                m_CurrNPC.ActivateEvent();
        }
    }

    // https://www.youtube.com/watch?v=4HpC--2iowE
    // https://www.youtube.com/watch?v=BJzYGsMcy8Q
    // https://forum.unity.com/threads/how-to-turn-input-axes-to-vectors-relative-to-the-way-an-object-is-facing.1375653/
    // https://catlikecoding.com/unity/tutorials/movement/orbit-camera/
    private void MovePlayer(Transform cameraTransf)
    {
        Vector3 inputDir = m_Orientation.forward * m_MoveDir.y + m_Orientation.right * m_MoveDir.x;
        Vector3 targetDir = Vector3.zero;
        //Vector3 inputDir = new Vector3(m_MoveDir.x, 0.0f, m_MoveDir.y);

        if (inputDir != Vector3.zero)
        {
            // First-person
            if (m_Camera.GetComponent<CameraMovement>().m_InFirstPerson)
            {
                targetDir = inputDir.normalized;
            }

            // Third-person
            else
            {
                Vector3 forward = cameraTransf.forward;
                forward.y = 0.0f;
                forward.Normalize();

                Vector3 right = cameraTransf.right;
                right.y = 0.0f;
                right.Normalize();

                //targetDir = cameraTransf.TransformDirection(m_MoveDir.x, 0.0f, m_MoveDir.y);
                targetDir = (forward * m_MoveDir.y + right * m_MoveDir.x);
                //targetDir = new Vector3(targetDir.x, 0.0f, targetDir.z);
            }

            //transform.forward = endRes;
            transform.forward = Vector3.Slerp(transform.forward, targetDir, Time.deltaTime * 5.0f);
        }

        // TODO: Change arbitrary 10.0f value
        m_RB.AddForce(targetDir * m_CurrentSpeed * 10.0f, ForceMode.Force);

        //transform.Translate(inputDir.normalized * m_Speed * Time.deltaTime, Space.World);
    }

    private void GroundCheck()
    {
        // TODO: Make player height based on character OR hitbox + change the arbitrary 0.2f value
        m_Grounded = Physics.Raycast(transform.position, Vector3.down, m_PlayerHeight * 0.5f + 0.2f, m_Ground);

        m_RB.drag = m_Grounded ? m_GroundDrag : 0.0f;
    }

    // Control object speed if it starts to move faster than max speed
    private void LimitSpeed()
    {
        Vector3 vel = new Vector3(m_RB.velocity.x, 0.0f, m_RB.velocity.z);
        
        if (vel.magnitude > m_CurrentSpeed)
        {
            Vector3 limitedVelocity = vel.normalized * m_CurrentSpeed;
            m_RB.velocity = new Vector3(limitedVelocity.x, m_RB.velocity.y, limitedVelocity.z);
        }
    }
}
