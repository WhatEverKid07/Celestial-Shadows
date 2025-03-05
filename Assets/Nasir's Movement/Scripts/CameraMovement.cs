using System;
using Unity.VisualScripting;
using UnityEditor;
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
    [Range(90, 100)]
    [SerializeField] private int fov;
    [SerializeField] private float rotationSpeed;

    [Header("Sensitivity")]
    private float mouseX;
    private float mouseY;

    [Range(5f, 50f)]
    [SerializeField] private float sensX;
    [Range(5f, 50f)]
    [SerializeField] private float sensY;

    private float currentRotationX = 0f;
    private float currentRotationZ = 0f;

    private void Start()
    {
        fpsCam.fieldOfView = fov;
        characterMove = FindFirstObjectByType<CharacterMovement>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        transform.position = new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z);

        mouseX = axisX.action.ReadValue<float>();
        mouseY = axisY.action.ReadValue<float>();

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


        if (characterMove.isRunning || characterMove.isWallRunning)
        {
            ChangeWalkFOV();
        }
        else
        {
            ChangeRunFOV();
        }
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
        currentRotationZ = Mathf.Clamp(currentRotationZ, 5f, 15f);

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(currentRotationX, 0, currentRotationZ), Time.deltaTime * rotationSpeed);
    }

    private void ChangeCameraLeftWallAngle()
    {
        float rotationX = mouseX * sensX / 100;
        float rotationZ = mouseY * sensY / 100;

        currentRotationX -= rotationZ;
        currentRotationX = Mathf.Clamp(currentRotationX, -20f, 20f);

        currentRotationZ += rotationX;
        currentRotationZ = Mathf.Clamp(currentRotationZ, -15f, -5f);

        transform.rotation = Quaternion.Slerp(transform.rotation,Quaternion.Euler(currentRotationX, -180, currentRotationZ), Time.deltaTime * rotationSpeed);
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
}
