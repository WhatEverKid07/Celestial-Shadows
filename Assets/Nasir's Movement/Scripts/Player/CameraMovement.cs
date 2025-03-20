
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovement : MonoBehaviour
{
    [Header("Scripts")]
    [SerializeField] private CharacterMovement characterMove;

    [Header("Player")]
    [SerializeField] private GameObject player;

    [Header("Inputs")]
    [SerializeField] private InputActionReference axisX;
    [SerializeField] private InputActionReference axisY;

    [Header("Camera")]
    [SerializeField] private Camera fpsCam;
    [SerializeField] [Range(90, 100)] private int fov;
    [SerializeField] [Range(8, 12)] private float rotationSpeed;


    private float wallCheckDist = .8f;
    private RaycastHit forwardHit;
    private RaycastHit backHit;

    private LayerMask wall;

    private bool isForwardWalled;
    private bool wasForwardWalled;

    private bool isBackWalled;
    private bool wasBackWalled;

    [Header("Sensitivity")]
    private float mouseX;
    private float mouseY;

    [SerializeField] [Range(5f, 50f)] private float sensX;
    [SerializeField] [Range(5f, 50f)] private float sensY;

    private float currentRotationX = 0f;
    private float currentRotationZ = 0f;

    private void Start()
    {
        fpsCam.fieldOfView = fov;
        characterMove = FindFirstObjectByType<CharacterMovement>();
        wall = LayerMask.GetMask("Wall");

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        mouseX = axisX.action.ReadValue<float>();
        mouseY = axisY.action.ReadValue<float>();

        CheckForWall();
        Debug.Log(wasForwardWalled);

        if (characterMove.isRightWalled && characterMove.isWallRunning && !characterMove.isWallJumping)
        {
            ChangeCameraRightWallAngle();
        }
        else if (characterMove.isLeftWalled && characterMove.isWallRunning && !characterMove.isWallJumping)
        {
            ChangeCameraLeftWallAngle();
        }
        else 
        {
            ChangeCameraWalkAngle();
        }

        if (!characterMove.isWallRunning || characterMove.isGrounded)
        {
            wasForwardWalled = false;
            wasBackWalled = false;
        }

        if (characterMove.isRunning || characterMove.isWallRunning && !characterMove.isWallJumping)
        {
            ChangeWalkFOV();
        }
        else
        {
            ChangeRunFOV();
        }
    }

    private void LateUpdate()
    {
        //transform.position = Vector3.SmoothDamp(transform.position, player.transform.position, ref velocity, 0.05f);
        transform.position = player.transform.position;
    }

    private void ChangeCameraWalkAngle()
    {
        float rotationX = mouseX * sensX / 100;
        float rotationY = mouseY * sensY / 100;

        currentRotationX -= rotationY;
        currentRotationX = Mathf.Clamp(currentRotationX, -80f, 80f);

        transform.rotation = Quaternion.Euler(currentRotationX, transform.rotation.eulerAngles.y + rotationX, 0);
        player.transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
    }

    private void ChangeCameraRightWallAngle()
    {
        float rotationX = mouseX * sensX / 100; 
        float rotationZ = mouseY * sensY / 100;

        currentRotationX -= rotationZ;
        currentRotationX = Mathf.Clamp(currentRotationX, -20f, 20f);

        currentRotationZ += rotationX;
        currentRotationZ = Mathf.Clamp(currentRotationZ, 5f, 20f);

        float currentRotationY = player.transform.rotation.eulerAngles.y;
        float targetY;

        if (Mathf.DeltaAngle(currentRotationY, -90) < Mathf.DeltaAngle(currentRotationY, 90))
        {
            if (wasForwardWalled || wasBackWalled)
            {
                targetY = 80;
            }
            else
            {
                targetY = 0;
            }
        }
        else
        {
            if (wasBackWalled || wasForwardWalled)
            {
                targetY = -80;
            }
            else
            {
                targetY = -180;
            }
        }

        if (characterMove.flipCam)
        {
            targetY = -targetY;
        }

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(currentRotationX, targetY, currentRotationZ), Time.deltaTime * rotationSpeed);
    }

    private void ChangeCameraLeftWallAngle()
    {
        float rotationX = mouseX * sensX / 100;
        float rotationZ = mouseY * sensY / 100;

        currentRotationX -= rotationZ;
        currentRotationX = Mathf.Clamp(currentRotationX, -20f, 20f);

        currentRotationZ += rotationX;
        currentRotationZ = Mathf.Clamp(currentRotationZ, -20f, -5f);

        float currentRotationY = player.transform.rotation.eulerAngles.y;
        float targetY;

        if (Mathf.DeltaAngle(currentRotationY, -90) < Mathf.DeltaAngle(currentRotationY, 90))
        {
            if (wasForwardWalled || wasBackWalled)
            {
                targetY = 80;
            }
            else
            {
                targetY = 0;
            }
        }
        else
        {
            if (wasBackWalled || wasForwardWalled)
            {
                targetY = -80;
            }
            else
            {
                targetY = -180;
            }
        }

        if (characterMove.flipCam)
        {
            targetY = -targetY;
        }

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(currentRotationX, targetY, currentRotationZ), Time.deltaTime * rotationSpeed);
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

    private void CheckForWall()
    {
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 backward = transform.TransformDirection(Vector3.back);

        isForwardWalled = Physics.Raycast(transform.position, forward, out forwardHit, 1.5f, wall);
        isBackWalled = Physics.Raycast(transform.position, backward, out backHit, 1.5f, wall);

        Debug.DrawLine(transform.position, transform.position + transform.forward * wallCheckDist, Color.cyan);
        Debug.DrawLine(transform.position, transform.position + -transform.forward * wallCheckDist, Color.black);

        if (isForwardWalled)
        {
            Debug.DrawRay(forwardHit.point, forwardHit.normal * 5, Color.cyan);
            wasForwardWalled = true;
        }

        if (isBackWalled)
        {
            Debug.DrawRay(backHit.point, backHit.normal * 5, Color.black);
            wasBackWalled = true;
        }
    }
}
