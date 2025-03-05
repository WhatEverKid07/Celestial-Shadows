using System.Collections.Generic;
using UnityEngine;

public class GunManagement : MonoBehaviour
{
    public List<GameObject> guns; // List of gun GameObjects
    private int currentGunIndex = 0; // Tracks the currently selected gun

    void Start()
    {
        UpdateGunSelection();
    }

    void Update()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll > 0f)
        {
            NextGun();
        }
        else if (scroll < 0f)
        {
            PreviousGun();
        }
    }

    void NextGun()
    {
        currentGunIndex = (currentGunIndex + 1) % guns.Count;
        UpdateGunSelection();
    }

    void PreviousGun()
    {
        currentGunIndex = (currentGunIndex - 1 + guns.Count) % guns.Count;
        UpdateGunSelection();
    }

    void UpdateGunSelection()
    {
        // Deactivate all guns first
        foreach (GameObject gun in guns)
        {
            gun.SetActive(false);
        }

        // Activate only the selected gun
        if (guns.Count > 0)
        {
            guns[currentGunIndex].SetActive(true);
        }
    }
}
