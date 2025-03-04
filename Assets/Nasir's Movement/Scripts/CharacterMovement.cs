using UnityEngine.InputSystem;
using UnityEngine;
using System.Collections;
using System;

public class CharacterMovement : MonoBehaviour
{
    [Header("Inputs")]
    [SerializeField] private InputActionReference playerCntrls;
    [SerializeField] private InputActionAsset playerCntrlsAss;
    private InputAction jump;
    private InputAction run;
    private InputAction dash;

    [Header("Properties")]
    [SerializeField] private GameObject player;
    [SerializeField] private Rigidbody rb;

    [Header("Camera")]
    [SerializeField] private Camera fpsCam;
    [SerializeField] private Transform camDir;
    [Range(90, 100)]
    [SerializeField] private int fov;

    [Header("Walking & Running")]
    private Vector3 moveDir;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float wallRunForce;
    private bool isRunning;
    public bool isGrounded { get; private set; }

    [Header("Wall Running")]
    private float wallWalkSpeed;
    private float wallRunSpeed;

    [SerializeField] private float wallCheckDist;
    private RaycastHit leftHit;
    private RaycastHit rightHit;

    public bool isLeftWalled { get; private set; }
    public bool isRightWalled { get; private set; }
    public bool isWallRunning { get; private set; }

    [Header("Jumping")]
    [Range(1f, 5f)]
    [SerializeField] private float jumpPower;
    private bool isJumping = false;

    [Range(1f, 3f)]
    [SerializeField] private float fallMulti;

    [Range(.1f, .25f)]
    [SerializeField] private float coyoteTime;
    private float setCoyoteTime;

    [SerializeField] private LayerMask ground;
    [SerializeField] private GameObject groundChecker;

    [Header("Wall Jumping")]

    [SerializeField] private LayerMask wall;
    [SerializeField] private GameObject wallChecker;

    [Header("Dashing")]
    [SerializeField] private bool enableDash;
    [Range(1f, 3f)]
    [SerializeField] private float dashPower;
    private bool canDash;
    private bool isDashing = false;

    [Range(.5f, 2f)]
    [SerializeField] private float DashTime;
    private float setDashTime;

    private Vector3 dashMomentum = Vector3.zero;
    private float momentumDecay = .5f;
    private Coroutine dashCoroutine;

    private void Start()
    {
        fpsCam.fieldOfView = fov;

        wallWalkSpeed = runSpeed;
        wallRunSpeed = wallWalkSpeed + 2f;

        groundChecker = GameObject.Find("GroundChecker");
        wallChecker = GameObject.Find("WallChecker");

        jump = playerCntrlsAss.FindActionMap("Player Controls").FindAction("Jump");
        run = playerCntrlsAss.FindActionMap("Player Controls").FindAction("Run");
        dash = playerCntrlsAss.FindActionMap("Player Controls").FindAction("Dash");

        jump.Enable();
        run.Enable();
        dash.Enable();

        dash.performed += ctx => Dash();
    }

    private void Update()
    {
        moveDir = playerCntrls.action.ReadValue<Vector3>();

        if (isRunning || isWallRunning)
        {
            ChangeWalkFOV();
        }
        else
        {
            ChangeRunFOV();
        }

        //COYOTE TIME
        if (!IsGrounded())
        {
            isGrounded = false;
            setCoyoteTime -= Time.deltaTime;
            if (rb.velocity.y > 0f)
            {
                setCoyoteTime = 0f;
            }
        }
        else
        {
            isGrounded = true;
            setCoyoteTime = coyoteTime;
        }

        CheckForWall();

        if (!canDash)
        {
            setDashTime = Mathf.Max(0f, setDashTime - Time.deltaTime);
            if (setDashTime == 0f)
            {
                canDash = true;
            }
        }

    }
    
    private void FixedUpdate()
    {
        //WALKING & RUNNING
        Move();

        //JUMPING
        if (jump.ReadValue<float>() > 0f)
        {
            Jump();
        }

        //COYOTE TIME
        if (setCoyoteTime == 0f)
        {
            ApplyGravity();
        }

    }

    //WALK & RUN FUNCTION
    private void Move()
    {
        //CAM DIRECTIONS
        Vector3 cameraForward = camDir.forward;
        Vector3 cameraRight = camDir.right;

        cameraForward.y = 0f;
        cameraRight.y = 0f;
        cameraForward.Normalize();
        cameraRight.Normalize();

        Vector3 move = cameraForward * moveDir.z + cameraRight * moveDir.x;

        if (run.ReadValue<float>() > 0)
        {
            rb.velocity = new Vector3(move.x * runSpeed, rb.velocity.y, move.z * runSpeed);
            isRunning = true;
        }
        else
        {
            rb.velocity = new Vector3(move.x * walkSpeed, rb.velocity.y, move.z * walkSpeed);
            isRunning = false;
        }
    }

    private void ChangeWalkFOV()
    {
        float runFOV = fov + 10;

        if (fpsCam.fieldOfView < runFOV)
        {
            fpsCam.fieldOfView += .1f;
        }
        else
        {
            fpsCam.fieldOfView = runFOV;
        }


    }

    private void ChangeRunFOV()
    {
        float walkFOV = fov - 10;

        if (fpsCam.fieldOfView > walkFOV)
        {
            fpsCam.fieldOfView -= .1f;
        }
        else
        {
            fpsCam.fieldOfView = walkFOV;
        }
    }

    //JUMP FUNCTION
    private void Jump()
    {
        Vector3 jumpForce = Vector3.up * jumpPower;

        if (setCoyoteTime > 0f && !isJumping)
        {
            isJumping = true;
            rb.AddForce(jumpForce, ForceMode.VelocityChange);

        }
        else
        {
            isJumping = false;
        }
    }

    private void WallJump()
    {

    }

    private void ApplyGravity()
    {
        rb.velocity += Vector3.up * Physics.gravity.y * (fallMulti - 1) * Time.deltaTime;
    }

    //DASH FUNCTION
    private void Dash()
    {
        if (enableDash)
        {
            if (canDash && !isDashing)
            {
                isDashing = true;

                dashCoroutine = StartCoroutine(PerformDash());

                canDash = false;
                setDashTime = DashTime;
            }
        }
    }

    //DASH IENUMERATOR
    private IEnumerator PerformDash()
    {
        Vector3 cameraForward = camDir.forward;
        Vector3 cameraRight = camDir.right;

        cameraForward.y = 0f;
        cameraRight.y = 0f;
        cameraForward.Normalize();
        cameraRight.Normalize();

        Vector3 dashDirection = cameraForward * moveDir.z + cameraRight * moveDir.x;

        //Default dash direction
        if (dashDirection.magnitude < .1f)
        {
            dashDirection = cameraForward;
        }

        float verticalVelocity = rb.velocity.y;

        float dashDuration = .2f;
        float elaspedTime = 0f;

        while (elaspedTime < dashDuration)
        {
            //Gradually applying dash force
            float smoothFactor = Mathf.Lerp(0f, 1f, elaspedTime / dashDuration);
            Vector3 dashForce = dashPower * smoothFactor * new Vector3(dashDirection.x, 0f, dashDirection.z);
            rb.AddForce(dashForce, ForceMode.VelocityChange);

            //Increment dash time
            elaspedTime += Time.deltaTime;

            yield return null;
        }

        dashMomentum = new Vector3(rb.velocity.x, verticalVelocity, rb.velocity.z);
        StartCoroutine(ApplyDashMomentum());

        isDashing = false;
    }

    private IEnumerator ApplyDashMomentum()
    {
        float momentumTime = .5f;
        float elaspedTime = 0f;

        while (elaspedTime < momentumTime)
        {
            rb.velocity = Vector3.Lerp(rb.velocity, dashMomentum, momentumDecay * Time.deltaTime);

            elaspedTime += Time.deltaTime;
            yield return null;
        }

        dashMomentum = Vector3.zero;
    }

    //GROUNDED BOOL
    private bool IsGrounded()
    {
        return Physics.CheckSphere(groundChecker.transform.position, .2f, ground);
    }

    public void CheckForWall()
    {
        isLeftWalled = Physics.Raycast(transform.position, camDir.right, out leftHit, wallCheckDist);
        isRightWalled = Physics.Raycast(transform.position, -camDir.right, out rightHit, wallCheckDist);
    }

    private void WallRunningMove()
    {
        Vector3 wallNormal = isLeftWalled ? leftHit.normal : rightHit.normal;
        Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);

        rb.AddForce(wallForward * wallRunForce, ForceMode.Force);
    }

}
