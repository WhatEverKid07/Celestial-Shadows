using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHeadBob : MonoBehaviour
{
    [Header("Scripts")]
    [SerializeField] private CharacterMovement characterMove;

    [Header("Player")]
    [SerializeField] private GameObject player;

    [Header("Head Bobbing")]
    [SerializeField] private Transform cameraHolder;
    [SerializeField] private Transform playerCamera;

    [SerializeField] [Range(1, 10)] internal float bobSpeed;
    [SerializeField] [Range(.05f, .0005f)] internal float bobForce;
    [SerializeField] [Range(1.2f, 1.7f)] private float bobMulti;

    internal float originalBobForce;
    internal float originalBobSpeed;

    private Vector3 originalCameraLocalPos;
    private float bobTimer = 0f;
    private Vector3 velocity = Vector3.zero;

    private void Start()
    {
        originalBobForce = bobForce;
        originalBobSpeed = bobSpeed;
        originalCameraLocalPos = playerCamera.localPosition; 
        characterMove = FindFirstObjectByType<CharacterMovement>();
    }

    private void Update()
    {
        cameraHolder.position = Vector3.SmoothDamp(cameraHolder.position, player.transform.position, ref velocity, 0.05f);

        if (characterMove.isGrounded && !characterMove.isWallRunning)
        {
            bobTimer += Time.deltaTime * bobSpeed;
            if (characterMove.isRunning)
            {
                float bobOffsetY = (Mathf.Sin(bobTimer) * bobForce * bobMulti) * 5;
                float bobOffsetX = (Mathf.Sin(bobTimer) * bobForce * bobMulti) * 2;
                playerCamera.localPosition = new Vector3(originalCameraLocalPos.x + bobOffsetX, originalCameraLocalPos.y + bobOffsetY, originalCameraLocalPos.z);
            }
            else
            {
                float bobOffsetY = Mathf.Sin(bobTimer) * bobForce;
                playerCamera.localPosition = new Vector3(originalCameraLocalPos.x, originalCameraLocalPos.y + bobOffsetY, originalCameraLocalPos.z);
            }
        }
        else
        {
            bobTimer = 0f;
            playerCamera.localPosition = Vector3.Slerp(playerCamera.localPosition, originalCameraLocalPos, Time.deltaTime * 5f);
        }

    }
}
