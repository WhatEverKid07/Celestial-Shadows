using UnityEngine.InputSystem;
using UnityEngine;
using System.Collections;
using System;
using System.Runtime.CompilerServices;

public class CharacterMovement : MonoBehaviour
{
    [Header("Inputs")]
    [SerializeField] private InputActionReference playerCntrls;
    [SerializeField] private InputActionAsset playerCntrlsAss;
    private InputAction jump;
    private InputAction run;
    private InputAction dash;

    [Header("Properties")]
    [SerializeField] private Transform orientation;
    [SerializeField] private Rigidbody rb;

    [Header("Camera")]
    [SerializeField] private Transform camDir;
    [Range(60, 100)]
    [SerializeField] private int fov;
    [SerializeField] private float offsetDistance;
    public bool flipCam { get; private set; }

    [Header("Walking & Running")]
    private Vector3 moveDir;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    public bool isRunning { get; private set; }
    public bool isGrounded { get; private set; }

    [Header("Wall Running")]
    [SerializeField] private float wallRunForce;
    [SerializeField] private float wallRunForceMulti;
    [SerializeField] private float wallRunTime;
    private float setWallRunTime;

    public bool facingForward { get; private set; }

    [SerializeField] private float wallCheckDist;
    private RaycastHit leftHit;
    private RaycastHit rightHit;

    public bool isLeftWalled { get; private set; }
    public bool isRightWalled { get; private set; }
    public bool isWallRunning { get; private set; }

    [Header("Jumping")]
    [Range(5f, 10f)]
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
    private bool canWallJump;
    private Coroutine jumpCoroutine;
    public bool isWallJumping { get; private set; }

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
        groundChecker = GameObject.Find("GroundChecker");

        jump = playerCntrlsAss.FindActionMap("Player Controls").FindAction("Jump");
        run = playerCntrlsAss.FindActionMap("Player Controls").FindAction("Run");
        dash = playerCntrlsAss.FindActionMap("Player Controls").FindAction("Dash");

        jump.Enable();
        run.Enable();
        dash.Enable();

        dash.performed += ctx => Dash();
        jump.performed += ctx => WallJump();
    }

    private void Update()
    {
        moveDir = playerCntrls.action.ReadValue<Vector3>();

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

        if ((isRightWalled || isLeftWalled) && !IsGrounded() && GroundedDistanceForWall())
        {
            isWallRunning = true;
        }
        else
        {
            isWallRunning = false;
        }

        if (!canDash)
        {
            setDashTime = Mathf.Max(0f, setDashTime - Time.deltaTime);
            if (setDashTime == 0f)
            {
                canDash = true;
            }
        }

        if (isWallRunning)
        {
            canWallJump = true;
            if (setWallRunTime > 0f)
            {
                setWallRunTime -= Time.deltaTime;
            }
        }
        else
        {
            canWallJump = false;
            if (setWallRunTime < wallRunTime)
            {
                setWallRunTime += .5f;
                if (setWallRunTime > wallRunTime)
                {
                    setWallRunTime = wallRunTime;
                }
            }
        }

        //Debug.Log();
    }
    
    private void FixedUpdate()
    {
        //WALKING & RUNNING
        Move();

        //JUMPING
        if (jump.ReadValue<float>() > 0f && !canWallJump)
        {
            Jump();
        }

        //COYOTE TIME
        if (setCoyoteTime <= 0f)
        {
            ApplyGravity();
        }

        if (isWallRunning && setWallRunTime > 0f && !isWallJumping)
        {
            WallRunningMove();
        }
        else
        {
            StopWallRunning();
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
    private void WallRunningMove()
    {
        //CAM DIRECTIONS
        Vector3 cameraForward = camDir.forward;
        Vector3 cameraRight = camDir.right;

        cameraForward.y = 0f;
        cameraRight.y = 0f;
        cameraForward.Normalize();
        cameraRight.Normalize();

        Vector3 move = cameraForward * moveDir.z + cameraRight * moveDir.x;

        rb.useGravity = false;
        rb.velocity = new Vector3(move.x * rb.velocity.x, 0, move.z * rb.velocity.z);

        Vector3 wallNormal = isLeftWalled ? leftHit.normal : rightHit.normal;

        Vector3 wallBackward = Vector3.Cross(wallNormal, transform.up);

        if ((transform.forward - wallBackward).magnitude  > (transform.forward - -wallBackward).magnitude)
        {
            wallBackward = -wallBackward;
            flipCam = true;
        }
        else
        {
            flipCam = false;
        }

        rb.AddForce(wallBackward * wallRunForce, ForceMode.Force);

        if (!(isLeftWalled && moveDir.x > 0f) || !(isRightWalled && -moveDir.x > 0))
        {
            rb.AddForce(-wallNormal * 100, ForceMode.Force);
        }

        rb.velocity = new Vector3(move.x * rb.velocity.x, rb.velocity.y, move.z * rb.velocity.z);

    }

    private void StopWallRunning()
    {
        Move();
        rb.useGravity = true;
    }

    //JUMP FUNCTION
    private void Jump()
    {
        Vector3 jumpForce = Vector3.up * jumpPower;

        if (setCoyoteTime > 0f && !isJumping)
        {
            isJumping = true;
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            rb.AddForce(jumpForce, ForceMode.VelocityChange);

        }
        else
        {
            isJumping = false;
        }
    }

    private void WallJump()
    {
        if (canWallJump && !isWallJumping)
        {
            isWallJumping = true;

            jumpCoroutine = StartCoroutine(PerformWallJump());
            canWallJump = false;
        }
    }

    private IEnumerator PerformWallJump()
    {
        Vector3 wallNormal = isLeftWalled ? leftHit.normal : rightHit.normal;

        Vector3 up = transform.TransformDirection(Vector3.up);
        Vector3 jumpDir = (wallNormal + up).normalized;

        rb.AddForce(jumpDir * (jumpPower * 3), ForceMode.VelocityChange);

        yield return new WaitForSeconds(1f);

        isWallJumping = false;
    }

    private void ApplyGravity()
    {
        if (rb.velocity.y < 0f)
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (fallMulti - 1) * Time.deltaTime;
        }
        else
        {
            rb.velocity += Vector3.down * Physics.gravity.y * (1 - fallMulti) * Time.deltaTime;
        }
    }

    //DASH FUNCTION
    private void Dash()
    {
        if (enableDash)
        {
            if (canDash && !isDashing && !isWallRunning)
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

    private bool GroundedDistanceForWall()
    {
        return !Physics.Raycast(transform.position, Vector3.down, 1f, ground);
    }

    public void CheckForWall()
    {
        Vector3 lft = transform.TransformDirection(Vector3.left);
        Vector3 rght = transform.TransformDirection(Vector3.right);

        isLeftWalled = Physics.Raycast(transform.position, lft, out leftHit, wallCheckDist);
        isRightWalled = Physics.Raycast(transform.position, rght, out rightHit, wallCheckDist);

        Debug.DrawLine(transform.position, transform.position + orientation.right * wallCheckDist, Color.green);
        Debug.DrawLine(transform.position, transform.position + -orientation.right * wallCheckDist, Color.red);

        if (isLeftWalled)
        {
            Debug.DrawRay(leftHit.point, leftHit.normal * 5, Color.green);
        }

        if (isRightWalled)
        {
            Debug.DrawRay(rightHit.point, rightHit.normal * 5, Color.red);
        }
    }
}
