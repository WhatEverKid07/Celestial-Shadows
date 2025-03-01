using UnityEngine.InputSystem;
using UnityEngine;
using System.Collections;

public class Character1Movement : MonoBehaviour
{
    [Header("Inputs")]
    [SerializeField] private InputActionReference playerCntrls;
    [SerializeField] private InputActionAsset plyCntrlsAss;
    private InputAction jump;
    private InputAction run;
    private InputAction dash;

    [Header("Properties")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private LayerMask ground;
    [SerializeField] private GameObject groundChecker;

    [Header("Camera")]
    [SerializeField] private GameObject cam;

    private Vector3 moveDir;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;

    [Header("Jumping")]
    [Range(1f, 5f)]
    [SerializeField] private float jumpPower;
    [Range(1f, 3f)]
    [SerializeField] private float fallMulti;
    [SerializeField] private float coyoteTime;
    private float setCoyoteTime;

    [Header("Dashing")]
    [SerializeField] private bool enableDash;
    [SerializeField] private float dashPower;
    private bool canDash;
    [SerializeField] private float maxDashTime;
    private float dashTime;



    private void Start()
    {
        jump = plyCntrlsAss.FindActionMap("Player Controls").FindAction("Jump");
        run = plyCntrlsAss.FindActionMap("Player Controls").FindAction("Run");
        dash = plyCntrlsAss.FindActionMap("Player Controls").FindAction("Dash");

        jump.Enable();
        run.Enable();
        dash.Enable();
    }

    private void OnEnable()
    {
        
    }

    private void Update()
    {
        moveDir = playerCntrls.action.ReadValue<Vector3>();
        
        if (!canDash)
        {
            dashTime = maxDashTime;
            dashTime -= Time.deltaTime;
            if (dashTime <= 0f) { dashTime = 0f; }
            if (dashTime == 0f)
            {
                canDash = true;
            }
        }
    }

    private void FixedUpdate()
    {
        //CAM DIRECTIONS
        Vector3 cameraForward = cam.transform.forward;
        Vector3 cameraRight = cam.transform.right;

        // Flatten the forward and right vectors to ignore vertical components
        cameraForward.y = 0f;
        cameraRight.y = 0f;
        cameraForward.Normalize();
        cameraRight.Normalize();

        // Calculate the movement direction based on camera orientation
        Vector3 move = cameraForward * moveDir.z + cameraRight * moveDir.x;

        //WALKING & RUNNING
        if (run.ReadValue<float>() > 0)
        {
            rb.velocity = new Vector3(move.x * runSpeed, rb.velocity.y, move.z * runSpeed);
        }
        else
        {
            rb.velocity = new Vector3(move.x * walkSpeed, rb.velocity.y, move.z * walkSpeed);
        }

        //JUMPING
        if (jump.ReadValue<float>() > 0)
        {
            Jump();
        }

        //COYOTE TIME
        if (!IsGrounded())
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (fallMulti - 1) * Time.deltaTime;
            setCoyoteTime -= Time.deltaTime;
            if (rb.velocity.y > 0f)
            {
                setCoyoteTime = 0f;
            }
        }
        else
        {
            setCoyoteTime = coyoteTime;
        }

        //DASHING
        if (enableDash)
        {
            dash.performed += ctx =>
            {
                Dash();
            };
        }
    }

    //JUMP FUNCTION
    private void Jump()
    {
        if (setCoyoteTime > 0f)
        {
            rb.AddForce(rb.velocity.x, jumpPower, rb.velocity.z, ForceMode.Impulse);
        }
    }

    //DASH FUNCTION
    private void Dash()
    {
        if (canDash)
        {
            rb.AddForce(dashPower * 10, rb.velocity.y, rb.velocity.z, ForceMode.Impulse);
            canDash = false;
        }
    }

    //GROUNDED BOOL
    private bool IsGrounded()
    {
        return Physics.CheckSphere(groundChecker.transform.position, .5f, ground);
    }

}
