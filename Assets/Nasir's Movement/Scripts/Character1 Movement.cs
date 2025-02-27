using UnityEngine.InputSystem;
using UnityEngine;

public class Character1Movement : MonoBehaviour
{
    [Header("Inputs")]
    [SerializeField] private InputActionReference playerCntrls;

    [Header("Properties")]
    [SerializeField] Rigidbody rb;
    [SerializeField] private float walkSpeed;

    [Header("Camera")]
    [SerializeField] private GameObject cam;

    private Vector3 moveDir;

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
}
