using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class PlayerController : MonoBehaviour
{
    private GameObject m_Camera;

    public GameObject Camera
    {
        get { return m_Camera; }
        set { m_Camera = value; }
    }

    [SerializeField] 
    float m_MinViewDist = 90.0f;

    [SerializeField] Transform m_Player;

    InputAction i_LMB;
    InputAction i_RMB;

    public Transform m_Orientation;

    private Vector2 m_MoveDir;
    private Vector2 m_MousePos;
    public float m_Speed;
    
    [Header("Ground Check")]
    private bool m_Grounded;
    public float m_PlayerHeight;
    public LayerMask m_Ground;
    public float m_GroundDrag;

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

        m_RB = GetComponent<Rigidbody>();
    }

    private void Start()
    {

    }

    private void Update()
    {
        LimitSpeed();
        GroundCheck();
        transform.rotation = m_Orientation.rotation;
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    void Move(InputAction.CallbackContext ctx)
    {
        m_MoveDir = ctx.ReadValue<Vector2>();
    }

    void MousePos(InputAction.CallbackContext ctx)
    {
        m_MousePos = ctx.ReadValue<Vector2>();
    }

    private void MovePlayer()
    {
        Vector3 moveDir = m_Orientation.forward * m_MoveDir.y + m_Orientation.right * m_MoveDir.x;

        // TODO: Change arbitrary 10.0f value
        m_RB.AddForce(moveDir.normalized * m_Speed * 10.0f, ForceMode.Force);
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
