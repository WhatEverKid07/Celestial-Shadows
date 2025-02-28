using UnityEngine.InputSystem;
using UnityEngine;
using System.Collections;

public class Character1Movement : MonoBehaviour
{
    [Header("Inputs")]
    [SerializeField] private InputActionReference playerCntrls;
    [SerializeField] private InputActionAsset plyCntrlsAss;
    private InputAction jump;

    [Header("Properties")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private LayerMask ground;
    [SerializeField] private GameObject groundChecker;

    [Header("Camera")]
    [SerializeField] private GameObject cam;

    private Vector3 moveDir;
    [SerializeField] private float walkSpeed;

    [Header("Jumping")]
    [Range(1f, 5f)]
    [SerializeField] private float jumpPower;
    [Range(1f, 3f)]
    [SerializeField] private float fallMulti;
    [SerializeField] private float coyoteTime;
    [SerializeField] private float jumpBuffTime;
    private float setCoyoteTime;
    private float setJumpBuffTime;



    private void Start()
    {
        jump = plyCntrlsAss.FindActionMap("Player Controls").FindAction("Jump");

        jump.Enable();
    }

    private void OnEnable()
    {
        
    }

    private void Update()
    {
        moveDir = playerCntrls.action.ReadValue<Vector3>();
        Debug.Log(setCoyoteTime);
    }

    private void FixedUpdate()
    {
        Vector3 cameraForward = cam.transform.forward;
        Vector3 cameraRight = cam.transform.right;

        // Flatten the forward and right vectors to ignore vertical components
        cameraForward.y = 0f;
        cameraRight.y = 0f;
        cameraForward.Normalize();
        cameraRight.Normalize();

        // Calculate the movement direction based on camera orientation
        Vector3 move = cameraForward * moveDir.z + cameraRight * moveDir.x;
        rb.velocity = new Vector3(move.x * walkSpeed, rb.velocity.y, move.z * walkSpeed);

        if (jump.ReadValue<float>() > 0)
        {
            setJumpBuffTime = jumpBuffTime;
            Jump();
        }
        else
        {
            setJumpBuffTime -= Time.deltaTime;
            if (setJumpBuffTime <= 0) { setJumpBuffTime = 0; }
        }

        if (!IsGrounded())
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (fallMulti - 1) * Time.deltaTime;

            setCoyoteTime -= Time.deltaTime;
        }
        else
        {
            setCoyoteTime = coyoteTime;
        }

    }

    private void Jump()
    {
        if (setJumpBuffTime > 0f && setCoyoteTime > 0f)
        {
            rb.AddForce(rb.velocity.x, jumpPower, rb.velocity.z, ForceMode.Impulse);
        }
    }

    private bool IsGrounded()
    {
        return Physics.CheckSphere(groundChecker.transform.position, .5f, ground);
    }

}
