using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Camera Movement Settings")]
    [SerializeField] private float speed = 10.0f;
    [SerializeField] private float sensitivity = 5.0f;

    private float rotationY = 0.0f;
    private float rotationX = 0.0f;

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

    private void HandleRecoil()
    {
        if (Input.GetKey(KeyCode.Mouse0) && Time.time >= nextRecoilTime)
        {
            AddRecoil();
            nextRecoilTime = Time.time + recoilCooldown;
        }
        currentRecoil = Vector3.Lerp(currentRecoil, Vector3.zero, recoilRecoverySpeed * Time.deltaTime);

        transform.localEulerAngles = new Vector3(rotationY, rotationX, 0) + currentRecoil;
    }

    private void AddRecoil()
    {
        float sideAmount = Random.Range(-sideRecoil.y, sideRecoil.y);
        Vector3 recoil = new Vector3(upRecoil.x, sideAmount, 0f);

        currentRecoil += recoil;
    }
}