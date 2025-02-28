using UnityEngine.InputSystem;
using UnityEngine;
using System.Collections;

public class Character1Movement : MonoBehaviour
{
    [Header("Inputs")]
    [SerializeField] private InputActionReference playerCntrls;
    private InputAction jump;

    [Header("Properties")]
    [SerializeField] Rigidbody rb;

    [Header("Camera")]
    [SerializeField] private GameObject cam;

    private Vector3 moveDir;
    [SerializeField] private float walkSpeed;

    [Header("Jumping")]
    private LayerMask ground;
    [SerializeField] private float jumpPower;
    private float jumpPowerOver;
    private bool canJump;

    private void Start()
    {
        jump.Enable();
        jump.performed += ctx => StartCoroutine(Jump());

        jumpPowerOver = jumpPower * Time.deltaTime;
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


        rb.velocity = new Vector3(move.x * walkSpeed, rb.velocity.y, move.z * walkSpeed);
        
    }

    private IEnumerator Jump()
    {
        if (canJump)
        {
            if (IsGrounded())
            {
                rb.velocity = new Vector3(rb.velocity.x, (rb.velocity.y * jumpPowerOver), rb.velocity.z);
                canJump = false;
            }
        }
        yield return new WaitUntil(() => canJump);
       
    }

    private bool IsGrounded()
    {
        return Physics.CheckSphere(transform.position, .5f, ground);
    }

}
