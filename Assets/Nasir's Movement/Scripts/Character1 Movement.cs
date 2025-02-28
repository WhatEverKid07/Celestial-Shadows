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
    [SerializeField] private float jumpPower;
    private bool canJump;
    private bool holdingJump;

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

        if (jump.ReadValue<float>() > 0)
        {
            holdingJump = true;
            Jump();
        }

        jump.performed += ctx =>
        {
            Jump();
            if (jump.ReadValue<float>() < 0 && canJump)
            {
         
            }
        };

    }

    private void Jump()
    {
        if (IsGrounded())
        {
            rb.AddForce(0, jumpPower, 0, ForceMode.Impulse);
        }
    }

    private bool IsGrounded()
    {
        return Physics.CheckSphere(groundChecker.transform.position, .5f, ground);
    }

}
