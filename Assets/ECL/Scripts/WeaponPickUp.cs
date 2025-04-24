using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickUp : MonoBehaviour
{
    public GunManagement gm;
    private float rotationSpeed = 30f;
    private float bobbingAmplitude = 0.1f;
    private float bobbingFrequency = 1f;
    private float originalYPosition;


    public GunManagement.SelectedWeapon selectedWeapon;

    // Must be public to work
    /*public string selectedWeapon;
    public static readonly string[] options = { "AssaultRifle", "Shotgun", "Pistol", "Knife", "Grenade" };*/

    private void Start()
    {
        originalYPosition = transform.position.y;
        //gm = GetComponent<GunManagement>();
    }

    private void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

        float newY = originalYPosition + (Mathf.Sin(Time.time * bobbingFrequency) * bobbingAmplitude);
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (gm != null)
            {
                gm.AddPickedUpWeapon(selectedWeapon);
                Destroy(gameObject);
            }
            else
            {
                Debug.LogWarning("GunManagement not found on player.");
            }
        }
    }
}