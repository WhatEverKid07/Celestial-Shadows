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
        characterMove = FindFirstObjectByType<CharacterMovement>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {

        mouseX = axisX.action.ReadValue<float>();
        mouseY = axisY.action.ReadValue<float>();

        if (characterMove.IsWalled())
        {
            ChangeCameraWallAngle();
        }
        else
        {
            ChangeCameraWalkAngle();
        }


        transform.position = new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z);
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

    private void ChangeCameraWallAngle()
    {
        float rotationX = mouseX * sensX / 100;
        float rotationZ = mouseY * sensY / 100;

        currentRotationX -= rotationZ;
        currentRotationX = Mathf.Clamp(currentRotationX, -80f, 80f);

        currentRotationZ += rotationX;
        if (characterMove.isLeftWalled)
        {
            currentRotationZ = Mathf.Clamp(currentRotationZ, -45f, -70f);
        }
        else
        {
            currentRotationZ = Mathf.Clamp(currentRotationZ, 45f, 70f);
        }

        float targetRotationY = characterMove.isLeftWalled ? -180: 0;
        float currentRotationY = targetRotationY;

        transform.rotation = Quaternion.Euler(currentRotationX, currentRotationY, currentRotationZ + rotationX);
        player.transform.rotation = Quaternion.Euler(0, currentRotationY, currentRotationZ);
    }
}
