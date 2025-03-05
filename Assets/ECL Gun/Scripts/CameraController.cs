using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [Header("Camera Movement Settings")]
    [SerializeField] private float speed = 10.0f;
    [SerializeField] private float sensitivity = 5.0f;
    [SerializeField] private InputActionAsset gunControls;

    private float rotationY = 0.0f;
    private float rotationX = 0.0f;
    private InputAction shoot;

    [Header("Recoil Settings")]
    [SerializeField] private Vector3 upRecoil;
    [SerializeField] private Vector3 sideRecoil;
    [SerializeField] private float recoilCooldown = 0.1f;
    [SerializeField] private float recoilRecoverySpeed = 2f;

    private Vector3 currentRecoil = Vector3.zero;
    private float nextRecoilTime = 0f;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        shoot = gunControls.FindActionMap("Gun Controls").FindAction("Shoot");
        shoot.Enable();
    }

    void Update()
    {
        HandleMovement();
        HandleMouseLook();
        HandleRecoil();
    }

    private void HandleMovement()
    {
        float translation = Input.GetAxis("Vertical") * speed * Time.deltaTime;
        float strafe = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        transform.Translate(strafe, 0, translation);
    }

    private void HandleMouseLook()
    {
        rotationX += Input.GetAxis("Mouse X") * sensitivity;
        rotationY -= Input.GetAxis("Mouse Y") * sensitivity;
        rotationY = Mathf.Clamp(rotationY, -90, 90);
    }

    public void GunController()
    {
        if (Time.time >= nextRecoilTime)
        {
            AddRecoil();
            nextRecoilTime = Time.time + recoilCooldown;
        }
    }

    private void HandleRecoil()
    {
        currentRecoil = Vector3.Lerp(currentRecoil, Vector3.zero, recoilRecoverySpeed * Time.deltaTime);

        transform.localEulerAngles = new Vector3(rotationY, rotationX, 0) + currentRecoil;
    }

    private void AddRecoil()
    {
        float sideAmount = Random.Range(-sideRecoil.y, sideRecoil.y);
        float upAmount = Random.Range(-upRecoil.x, upRecoil.x);
        Vector3 recoil = new Vector3(upAmount, sideAmount, 0f);

        currentRecoil += recoil;
    }
}