using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Character1Movement : MonoBehaviour
{
    [Header("Inputs")]
    [SerializeField] private PlayerControls playerCntrls;
    [SerializeField] private InputAction playerMove;

    [Header("Properties")]
    [SerializeField] Rigidbody rb;

    [Header("Calculating Movement")]
    private Vector3 moveDir;
    [SerializeField] private float walkSpeed;

    private void Awake()
    {
        playerCntrls = new PlayerControls();
    }

    private void OnEnable()
    {
        playerMove = playerCntrls.Player.Movement;
        playerMove.Enable();
    }

    private void Update()
    {
        
    }

    private void FixedUpdate()
    {
        moveDir = playerMove.ReadValue<Vector3>();
        rb.velocity = new Vector3((moveDir.x * walkSpeed), rb.velocity.y, (moveDir.z * walkSpeed));
        
    }
}
