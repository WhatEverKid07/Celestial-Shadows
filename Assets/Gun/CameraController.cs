using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float speed = 10.0f;       // Speed of the camera movement
    public float sensitivity = 5.0f;  // Sensitivity of the mouse movement

    private float rotationY = 0.0f;   // Initial rotation around the Y axis
    private float rotationX = 0.0f;   // Initial rotation around the X axis

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // WASD movement
        float translation = Input.GetAxis("Vertical") * speed * Time.deltaTime;
        float strafe = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        transform.Translate(strafe, 0, translation);

        // Mouse movement
        rotationX += Input.GetAxis("Mouse X") * sensitivity;
        rotationY -= Input.GetAxis("Mouse Y") * sensitivity;
        rotationY = Mathf.Clamp(rotationY, -90, 90); // Prevent flipping over

        transform.localEulerAngles = new Vector3(rotationY, rotationX, 0);
    }
}