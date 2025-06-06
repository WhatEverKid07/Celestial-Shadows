using UnityEngine.InputSystem;
using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class CharacterMovement : MonoBehaviour
{
    [Header("Inputs")]
    [SerializeField] private InputActionReference playerCntrls;
    [SerializeField] private InputActionAsset playerCntrlsAss;
    private InputAction jump;
    private InputAction run;
    private InputAction dash;
    private InputAction crouch;

    private bool restricted = false;

    [Header("Properties")]
    [SerializeField] private Transform orientation;
    [SerializeField] private Rigidbody rb;

    [Header("Camera")]
    [SerializeField] private Transform camDir;
    [SerializeField] [Range(60, 100)] private int fov;
    public bool flipCam { get; private set; }

    [Header("Walking & Running")]
    [SerializeField] internal float walkSpeed;
    [SerializeField] internal float runSpeed;

    internal bool canRun = true;

    public Vector3 moveDir { get; private set; }
    public bool isRunning { get; private set; }
    public bool isGrounded { get; private set; }

    private bool isWalking = false;

    [Header("Crouching")]
    [SerializeField] private float crouchWalkSpeed;
    [SerializeField] private float crouchRunSpeed;
    [SerializeField] private float crouchYScale;
    private float startYScale;

    [Header("Slope Movement")]
    [SerializeField] private float maxSlopeAngle;
    private RaycastHit slopeHit;
    private bool exitingSlope;
    private bool reduceVelo;


    [Header("Wall Running")]
    [SerializeField] [Range(300, 500)] internal float wallRunForce;
    [SerializeField] [Range(1.2f, 1.5f)] private float wallRunForceMulti;
    [SerializeField] [Range(1, 3)] private float wallRunTime;
    private float setWallRunTime;

    private float minWallRunAngle = 55f;
    private float maxWallRunAngle = 89f;
    private bool correctWallRunAngle;

    public bool facingForward { get; private set; }

    private float wallCheckDist = .8f;
    private RaycastHit leftHit;
    private RaycastHit rightHit;

    public bool isLeftWalled { get; private set; }
    public bool isRightWalled { get; private set; }
    public bool isWallRunning { get; private set; }

    [Header("Jumping")]
    [SerializeField] private float jumpPower;
    private bool isJumping = false;

    [SerializeField] [Range(1, 3)] private float fallMulti;

    [SerializeField] [Range(.1f, .25f)] private float coyoteTime;
    private float setCoyoteTime;

    private LayerMask ground;
    private RaycastHit groundHit;
    private GameObject groundChecker;

    [Header("Wall Jumping")]
    private LayerMask wall;
    private bool canWallJump;
    private Coroutine jumpCoroutine;
    public bool isWallJumping { get; private set; }

    [Header("Ledge Climbing")]
    private float ledgeCheckDist = 1.2f;
    private LayerMask ledge;

    private Transform lastLedge;
    private Transform currentLedge;

    private RaycastHit ledgeHit;

    [Header("Ledge Grabbing")]
    private float grabLedgeSpeed = 3f;
    private float maxLedgeGrabDist = .8f;
    public bool isHoldingLedge { get; private set; }
    private Coroutine holdLedgeCoroutine;

    [Header("Dashing")]
    [SerializeField] internal bool enableDash = true;
    [SerializeField] internal float dashPower;
    private bool canDash;
    private bool isDashing = false;

    [SerializeField] [Range(.5f, 2)] internal float dashTime;
    private float setDashTime;

    private Vector3 dashMomentum = Vector3.zero;
    private float momentumDecay = .5f;
    private Coroutine dashCoroutine;

    [Header("Debugging")]
    private bool enableDebug = false;
    [SerializeField] private LayerMask ignore;

    [Header("Stamina")]
    [SerializeField] private Slider staminaBar;
    [SerializeField] private int maxStamina;
    [SerializeField] private float staminaLose;
    public float stamina;
    public bool loseStamina = false;
    //public bool regenStamina = false;
    //public bool isLosingStamina = false;

    private void Start()
    {
        groundChecker = GameObject.Find("GroundChecker");
        ground = LayerMask.GetMask("Ground");
        wall = LayerMask.GetMask("Wall");
        ledge = LayerMask.GetMask("Ledge");

        jump = playerCntrlsAss.FindActionMap("Player Controls").FindAction("Jump");
        run = playerCntrlsAss.FindActionMap("Player Controls").FindAction("Run");
        dash = playerCntrlsAss.FindActionMap("Player Controls").FindAction("Dash");
        crouch = playerCntrlsAss.FindActionMap("Player Controls").FindAction("Crouch");

        jump.Enable();
        run.Enable();
        dash.Enable();
        crouch.Enable();

        dash.performed += ctx => Dash();
        jump.performed += ctx => WallJump();

        startYScale = transform.localScale.y;

        stamina = maxStamina;
        staminaBar.maxValue = maxStamina;
        staminaBar.value = maxStamina;
    }

    private void Update()
    {
        //MOVE INPUT
        moveDir = playerCntrls.action.ReadValue<Vector3>();

        if (OnSlope())
        {
            exitingSlope = false;
        }
        else
        {
            exitingSlope = true;
        }

        CheckForWall();
        CheckForLedge();

        if (correctWallRunAngle && !IsGrounded())
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
            setCoyoteTime = coyoteTime;
            if (setWallRunTime > 0f)
            {
                if (run.ReadValue<float>() > 0f)
                {
                    setWallRunTime -= Time.deltaTime * 1.4f;
                }
                else
                {
                    setWallRunTime -= Time.deltaTime;
                }
            }
        }
        else
        {
            if (setCoyoteTime <= 0f)
            {
                canWallJump = false;
            }

            if (setWallRunTime < wallRunTime)
            {
                setWallRunTime += .5f;
                if (setWallRunTime > wallRunTime)
                {
                    setWallRunTime = wallRunTime;
                }
            }
        }

        //FORCE EXIT LEDGE
        if (isHoldingLedge && moveDir.magnitude > 0.1f)
        {
            ExitLedgeGrab();
        }

        if (isWalking && !isJumping)
        {
            AudioManager.instance.PlayerWalking(true);
        }
        else if (isRunning && !isJumping)
        {
            AudioManager.instance.PlayerRunning(true);
        }
        else
        {
            AudioManager.instance.PlayerRunning(false);
            AudioManager.instance.PlayerWalking(false);
        }
    }
    
    private void FixedUpdate()
    {
        //FORCED RESTRICTION
        if (restricted)
        {
            return;
        }

        //WALKING & RUNNING
        Move();

        //JUMPING
        if (jump.ReadValue<float>() > 0f && !canWallJump && !isHoldingLedge)
        {
            Jump();
        }

        //GRAVITY
        if (setCoyoteTime <= 0f && !isDashing && !isHoldingLedge)
        {
            ApplyGravity();
        }

        //WALL RUNNING
        if (isWallRunning && setWallRunTime > 0f && !isWallJumping && !isHoldingLedge && !IsGrounded())
        {
            WallRunningMove();
        }
        else
        {
            StopWallRunning();
        }

        bool wasGrounded = isGrounded;
        isGrounded = IsGrounded();

        if (isGrounded)
        {
            setCoyoteTime = coyoteTime;
            if (!wasGrounded && isJumping)
            {
                isJumping = false;
            }
        }
        else
        {
            if (!isWallRunning)
            {
                setCoyoteTime -= Time.fixedDeltaTime;
            }

            if (rb.velocity.y > 0f)
            {
                setCoyoteTime = 0f;
            }
        }

        if (loseStamina && stamina >= 0)
        {
            //isLosingStamina = true;
            stamina -= staminaLose;
        }
        else
        {
            //isLosingStamina = false;
        }
        if (/*regenStamina &&*/ stamina < maxStamina && !loseStamina)
        {
            stamina += 0.3f;
        }
        if (staminaBar != null) { UpdateStaminaBar(); }

        if (isRunning) { loseStamina = true; }
        else if (!isRunning) { loseStamina = false; }
    }
    private void UpdateStaminaBar()
    {
        staminaBar.value = stamina;
    }

    //WALK & RUN FUNCTION
    private void Move()
    {
        // CAM DIRECTIONS
        Vector3 cameraForward = camDir.forward;
        Vector3 cameraRight = camDir.right;

        cameraForward.y = 0f;
        cameraRight.y = 0f;
        cameraForward.Normalize();
        cameraRight.Normalize();

        Vector3 move = cameraForward * moveDir.z + cameraRight * moveDir.x;
        Debug.Log(move);
        rb.useGravity = !OnSlope();

        bool hasMovementInput = moveDir.magnitude > 0.1f;

        if (run.ReadValue<float>() > 0 && canRun && hasMovementInput && !(crouch.ReadValue<float>() > 0) && stamina >= 0)
        {
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);

            rb.velocity = new Vector3(move.x * runSpeed, rb.velocity.y, move.z * runSpeed);

            isRunning = true;
            isWalking = false;

            if (OnSlope())
            {
                if (!exitingSlope)
                {
                    rb.AddForce(5f * runSpeed * GetSlopeMoveDirection(), ForceMode.Force);
                }
                if (rb.velocity.y > 0)
                {
                    rb.AddForce(Vector3.down * 10f, ForceMode.Force);
                }
            }
        }
        else if (hasMovementInput && !(crouch.ReadValue<float>() > 0))
        {
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);

            rb.velocity = new Vector3(move.x * walkSpeed, rb.velocity.y, move.z * walkSpeed);

            isRunning = false;
            isWalking = true;

            if (OnSlope())
            {
                if (!exitingSlope)
                {
                    rb.AddForce(5f * walkSpeed * GetSlopeMoveDirection(), ForceMode.Force);
                }
                if (rb.velocity.y > 0)
                {
                    rb.AddForce(Vector3.down * 10f, ForceMode.Force);
                }
            }
        }
        else if (crouch.ReadValue<float>() > 0)
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Force);

            if (hasMovementInput)
            {
                rb.velocity = new Vector3(move.x * crouchWalkSpeed, rb.velocity.y, move.z * crouchWalkSpeed);
            }

            if (run.ReadValue<float>() > 0 && canRun && hasMovementInput)
            {
                rb.velocity = new Vector3(move.x * crouchRunSpeed, rb.velocity.y, move.z * crouchRunSpeed);
            }
        }
        else
        {
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);

            isWalking = false;
            isRunning = false;
        }
    }

    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, 2f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    private Vector3 GetSlopeMoveDirection()
    {
        //CAM DIRECTIONS
        Vector3 cameraForward = camDir.forward;
        Vector3 cameraRight = camDir.right;

        cameraForward.y = 0f;
        cameraRight.y = 0f;
        cameraForward.Normalize();
        cameraRight.Normalize();

        Vector3 move = cameraForward * moveDir.z + cameraRight * moveDir.x;
        if (move.magnitude < .1f)
        {
            move = cameraForward;
            if (Vector3.Angle(transform.forward, slopeHit.normal) > Vector3.Angle(transform.forward, -slopeHit.normal))
            {
                move = -cameraForward;
            }

            Debug.Log(slopeHit.normal);
        }

        return Vector3.ProjectOnPlane(move, slopeHit.normal).normalized;
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

        //GETS VECTOR PARALLEL TO THE WALL
        Vector3 wallNormal = isLeftWalled ? leftHit.normal : rightHit.normal;

        //GETS VECTOR PERPENDICULAR FROM THE PLAYER TO THE WALL
        Vector3 wallBackward = Vector3.Cross(wallNormal, transform.up);

        //FLIPS VECTOR ON OPPOSITE SIDE OF THE WALL
        float dotProduct= Vector3.Dot(transform.forward, wallBackward);

        if (dotProduct < 0f) 
        {
            wallBackward = -wallBackward;
            flipCam = true;
        }
        else
        {
            flipCam = false;
        }  

        //WALL WALKING & WALL RUNNING
        if (run.ReadValue<float>() > 0f)
        {
            rb.AddForce(wallRunForce * wallRunForceMulti * wallBackward, ForceMode.Force);
        }
        else
        {
            rb.AddForce(wallRunForce * wallBackward, ForceMode.Force);

        }

        //ANY INPUT AWAY FROM THE WALL, GETS OFF THE WALL
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
        isWallRunning = false;
    }

    //JUMP FUNCTION
    private void Jump()
    {
        if (setCoyoteTime > 0f && !isJumping)
        {
            isJumping = true;
            AudioManager.instance.PlayerJump();
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            rb.AddForce(Vector3.up * jumpPower, ForceMode.VelocityChange);
            exitingSlope = true;
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
        //DIAGONAL JUMP FROM WALL
        Vector3 jumpDir = (wallNormal * 1.5f) + (up * 1f);

        rb.AddForce(jumpDir * (jumpPower * 1.5f), ForceMode.VelocityChange);

        yield return new WaitForSeconds(.2f);

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
        if (!enableDash || stamina < 20f) return;

        if (canDash && !isDashing && !isWallRunning)
        {
            AudioManager.instance.PlayerDash();
            isDashing = true;

            dashCoroutine = StartCoroutine(PerformDash());

            canDash = false;
            setDashTime = dashTime;
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

        float verticalVelocity = rb.velocity.y;

        float dashDuration = .2f;
        float elaspedTime = 0f;

        while (elaspedTime < dashDuration)
        {
            //Gradually applying dash force
            float smoothFactor = Mathf.Lerp(0f, 1f, elaspedTime / dashDuration);
            Vector3 dashForce = dashPower * smoothFactor * new Vector3(dashDirection.x, 0, dashDirection.z);
            rb.AddForce(dashForce, ForceMode.VelocityChange);

            //Increment dash time
            elaspedTime += Time.deltaTime;

            yield return null;
        }

        //CONTINUE VELOCITY IN X,Z DIRECTIONS
        dashMomentum = new (rb.velocity.x, verticalVelocity, rb.velocity.z);
        StartCoroutine(ApplyDashMomentum());

        stamina -= 20f;

        isDashing = false;
    }

    private IEnumerator ApplyDashMomentum()
    {
        float momentumTime = .5f;
        float elaspedTime = 0f;

        //SLOWLY REMOVE THE VELOCITY OVERTIME
        while (elaspedTime < momentumTime)
        {
            rb.velocity = Vector3.Lerp(rb.velocity, dashMomentum, momentumDecay * Time.deltaTime);

            elaspedTime += Time.deltaTime;
            yield return null;
        }

        rb.velocity = new Vector3(0f, rb.velocity.y, 0f);
        dashMomentum = Vector3.zero;
    }

    //GROUNDED BOOL
    private bool IsGrounded()
    {
        return Physics.CheckSphere(groundChecker.transform.position, 0.2f, ground);
    }

    private void CheckForWall()
    {
        Vector3 lft = transform.TransformDirection(Vector3.left);
        Vector3 rght = transform.TransformDirection(Vector3.right);

        isLeftWalled = Physics.Raycast(transform.position, lft, out leftHit, wallCheckDist, wall);
        isRightWalled = Physics.Raycast(transform.position, rght, out rightHit, wallCheckDist, wall);

        if (isLeftWalled || isRightWalled)
        {
            Vector3 wallNormal = isLeftWalled ? leftHit.normal : rightHit.normal;
            float angle = Vector3.Angle(transform.forward, -wallNormal);

            correctWallRunAngle = angle >= minWallRunAngle && angle <= maxWallRunAngle;
        }
        else
        {
            correctWallRunAngle = false;
        }

        if (isLeftWalled)
        {
            Debug.DrawRay(leftHit.point, leftHit.normal * 5, Color.green);
        }

        if (isRightWalled)
        {
            Debug.DrawRay(rightHit.point, rightHit.normal * 5, Color.red);
        }
    }

    private void CheckForLedge()
    {
        Vector3 horizontal = new (transform.position.x, 0f, transform.position.z);
        bool foundLedge = Physics.SphereCast(transform.position, .2f, horizontal, out ledgeHit, ledgeCheckDist, ledge);

        if (!foundLedge)
        {
            return;
        }
        else
        {
            Debug.DrawRay(transform.position, ledgeHit.point, Color.blue);
        }

        Vector3 playerPos = new (transform.position.x, 0f, transform.position.z);
        Vector3 ledgePos = new (transform.position.x, 0f, ledgeHit.point.z);
        float distToLedge = Vector3.Distance(playerPos, ledgePos);

        if (distToLedge < maxLedgeGrabDist && !isHoldingLedge)
        {
            currentLedge = ledgeHit.transform;
            lastLedge = currentLedge;

            restricted = true;
            rb.useGravity = false;

            isHoldingLedge = true;

            EnterLedgeGrab();
        }
    }

    private void EnterLedgeGrab()
    {
        holdLedgeCoroutine = StartCoroutine(PerformLedgeGrab());
    }

    private IEnumerator PerformLedgeGrab()
    {
        Vector3 ledgeDir = currentLedge.position - transform.position;
        float ledgeDist = Vector3.Distance(transform.position, currentLedge.position);

        while (ledgeDist > 1f)
        {
            if (rb.velocity.magnitude < grabLedgeSpeed)
            {
                rb.AddForce(1000f * grabLedgeSpeed * Time.deltaTime * ledgeDir.normalized);
            }

            ledgeDist = Vector3.Distance(transform.position, currentLedge.position);
            yield return null;
        }

        rb.velocity = Vector3.zero;
        MoveToClosestGround();
    }

    private void MoveToClosestGround()
    {
        float searchRadius = 1f;
        Collider[] groundColliders = Physics.OverlapSphere(currentLedge.position, searchRadius, ground);

        if (groundColliders.Length > 0)
        {
            Collider closestGround = groundColliders[0];
            float minDist = Vector3.Distance(transform.position, closestGround.ClosestPoint(transform.position));

            foreach (Collider col in groundColliders)
            {
                float dist = Vector3.Distance(transform.position, col.ClosestPoint(transform.position));
                if (dist < minDist)
                {
                    minDist = dist;
                    closestGround = col;
                }
            }

            ExitLedgeGrab();
            Vector3 closestGroundPosition = closestGround.ClosestPoint(transform.position);
            Vector3 forwardOffset = transform.forward * .5f;

            transform.position = new (forwardOffset.x + closestGroundPosition.x, closestGroundPosition.y + 1f, forwardOffset.z + closestGroundPosition.z);
            rb.velocity = Vector3.zero;
        }
    }
    private void ExitLedgeGrab()
    {
        restricted = false;
        isHoldingLedge = false;

        StopAllCoroutines();
        Invoke(nameof(EnableGravity), .5f);
        Invoke(nameof(ResetLastLedge), .5f);
    }

    private void ResetLastLedge()
    {
        lastLedge = null;
    }

    private void EnableGravity()
    {
        rb.useGravity = true;
    }

    private IEnumerator RegainStamina()
    {
        yield return new WaitForSeconds(2);
        if (/*isLosingStamina ||*/ loseStamina) { /*regenStamina = false;*/ yield break;}
        if (stamina == maxStamina) { /*regenStamina = false;*/ yield break;}
        //regenStamina = true;
    }
}