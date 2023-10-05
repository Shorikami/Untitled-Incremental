using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private GameObject m_Camera;

    public GameObject Camera
    {
        get { return m_Camera; }
        set { m_Camera = value; }
    }

    [SerializeField] Transform m_Player;

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

    public float m_Speed;
    
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

        i_LMB = m_PlayerInput.Player.LMB;
        i_RMB = m_PlayerInput.Player.RMB;
        i_MouseWheel = m_PlayerInput.Player.MouseWheel;

        m_RB = GetComponent<Rigidbody>();
    }

    private void Start()
    {

    }

    private void Update()
    {
        LimitSpeed();
        GroundCheck();

        if (m_Camera.GetComponent<CameraMovement>().m_InFirstPerson)
        {
            transform.rotation = m_Orientation.rotation;
            HandleInteractionCheck();
            HandleInteractionInput();
        }
    }

    private void FixedUpdate()
    {
        MovePlayer(m_Camera.transform);
    }

    private void HandleInteractionCheck()
    {
        if (Physics.Raycast(m_Camera.GetComponent<Camera>().ViewportPointToRay(m_InteractionRayPoint), out RaycastHit hit, m_InteractionDist))
        {
            if (hit.collider.gameObject.layer == 7 && (m_CurrInteractable == null || hit.collider.gameObject.GetInstanceID() != m_CurrInteractable.GetInstanceID()))
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
        m_RB.AddForce(targetDir * m_Speed * 10.0f, ForceMode.Force);

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
        
        if (vel.magnitude > m_Speed)
        {
            Vector3 limitedVelocity = vel.normalized * m_Speed;
            m_RB.velocity = new Vector3(limitedVelocity.x, m_RB.velocity.y, limitedVelocity.z);
        }
    }
}
