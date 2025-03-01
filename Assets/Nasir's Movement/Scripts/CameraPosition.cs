using UnityEngine;
using UnityEngine.InputSystem;

public class CameraPosition : MonoBehaviour
{
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

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            mouseX = axisX.action.ReadValue<float>();
            mouseY = axisY.action.ReadValue<float>();

            float rotationX = mouseX * sensX / 100;
            float rotationY = mouseY * sensY / 100;

            currentRotationX -= rotationY;
            currentRotationX = Mathf.Clamp(currentRotationX, -80f, 80f);

            transform.rotation = Quaternion.Euler(currentRotationX, transform.rotation.eulerAngles.y + rotationX, 0);

            player.transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);

        }
        transform.position = new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z);
    }
}
