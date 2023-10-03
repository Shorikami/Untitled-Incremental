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

    public float m_MouseSens = 100.0f;
    private float m_RotationX = 0.0f;

    private Vector2 m_MousePos;
    private bool m_InFirstPerson = true;

    InputAction i_LMB;
    InputAction i_RMB;

    private Vector2 m_MoveDir;
    public float m_Speed;

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
        // TODO: Change this sometime pls
        m_Camera = GameObject.FindGameObjectWithTag("MainCamera");

        m_Camera.transform.SetParent(transform);
    }

    private void Update()
    {
        m_InFirstPerson = m_Camera.transform.localPosition == Vector3.zero ? true : false;
        RotateCameraWithMouse();

        Debug.Log(i_RMB.ReadValue<float>());

        m_Camera.transform.localPosition += m_Camera.transform.forward * Input.GetAxis("Mouse ScrollWheel") * 100.0f * Time.deltaTime;

    }

    private void FixedUpdate()
    {
        Vector3 playerVel = new Vector3(m_MoveDir.x * m_Speed, m_RB.velocity.y, m_MoveDir.y * m_Speed);
        m_RB.velocity = transform.TransformDirection(playerVel);
    }

    void Move(InputAction.CallbackContext ctx)
    {
        m_MoveDir = ctx.ReadValue<Vector2>();
    }

    void MousePos(InputAction.CallbackContext ctx)
    {
        m_MousePos = ctx.ReadValue<Vector2>();
    }

    private void RotateCameraWithMouse()
    {
        float mX = Input.GetAxis("Mouse X") * m_MouseSens * Time.deltaTime;
        float mY = Input.GetAxis("Mouse Y") * m_MouseSens * Time.deltaTime;

        //Debug.Log(mX + " " + mY);

        if (m_InFirstPerson || (!m_InFirstPerson && i_RMB.ReadValue<float>() > 0.0f))
        m_RotationX -= mY;
        m_RotationX = Mathf.Clamp(m_RotationX, -90.0f, m_MinViewDist);

        m_Camera.transform.localRotation = Quaternion.Euler(m_RotationX, 0.0f, 0.0f);

        if (m_InFirstPerson)
            transform.Rotate(Vector3.up * mX);
    }
}
