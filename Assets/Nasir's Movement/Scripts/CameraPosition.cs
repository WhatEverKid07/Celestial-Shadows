using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;
using static UnityEditor.Timeline.TimelinePlaybackControls;

public class CameraPosition : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private GameObject player;

    [Header("Inputs")]
    [SerializeField] private InputActionReference axisX;
    [SerializeField] private InputActionReference axisY;

    private float mouseX;
    private float mouseY;

    [Range(20f, 100f)]
    [SerializeField] private float sensX;
    [Range(20f, 100f)]
    [SerializeField] private float sensY;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    private void Update()
    {
        mouseX = axisX.action.ReadValue<float>();
        mouseY = axisY.action.ReadValue<float>();

        float rotationX = mouseX * (sensX * Time.deltaTime);
        float rotationY = mouseY * (sensY * Time.deltaTime);
        rotationX = Mathf.Clamp(rotationX, -80f, 80);

        Vector2 cameraRotation = transform.rotation.eulerAngles;

        cameraRotation.x -= rotationY;
        cameraRotation.y += rotationX;

        transform.rotation = Quaternion.Euler(cameraRotation.x, cameraRotation.y, 0);
        player.transform.rotation = Quaternion.Euler(0, cameraRotation.y , 0);
        transform.position = new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z);
    }

}
