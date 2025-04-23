using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class GunManagement : MonoBehaviour
{
    [SerializeField] private List<GameObject> activeWeapons;
    //[SerializeField] private List<GameObject> nonActiveWeapons;
    [SerializeField] private InputActionAsset gunControls;

    [SerializeField] private GameObject assaultRifle;
    [SerializeField] private GameObject shotgun;
    [SerializeField] private GameObject pistol;
    [SerializeField] private GameObject knife;
    [SerializeField] private GameObject grenade;

    private int currentGunIndex = 0;
    internal bool canSwitch = true;
    private InputAction scroll;

    public enum SelectedWeapon
    {
        AssaultRifle,
        Shotgun,
        Pistol,
        Grenade,
        Knife
    }

    void Start()
    {
        scroll = gunControls.FindActionMap("Gun Controls").FindAction("Scroll");
        scroll.Enable();
        UpdateGunSelection();
    }

    void Update()
    {
        float z = scroll.ReadValue<float>();
        //Debug.Log(z); // *For Debugging*
        //Debug.Log(canSwitch);
        if (z > 0 && canSwitch)
        {
            NextGun();
        }
        if (z < 0 && canSwitch)
        {
            PreviousGun();
        }
    }

    void NextGun()
    {
        currentGunIndex = (currentGunIndex + 1) % activeWeapons.Count;
        UpdateGunSelection();
    }

    void PreviousGun()
    {
        currentGunIndex = (currentGunIndex - 1 + activeWeapons.Count) % activeWeapons.Count;
        UpdateGunSelection();
    }

    void UpdateGunSelection()
    {
        // Deactivate all guns first
        foreach (GameObject gun in activeWeapons)
        {
            gun.SetActive(false);
        }

        // Activate only the selected gun
        if (activeWeapons.Count > 0)
        {
            activeWeapons[currentGunIndex].SetActive(true);
        }
    }

    public void AddPickedUpWeapon(GunManagement.SelectedWeapon selectedWeapon)
    {
        switch (selectedWeapon)
        {
            case SelectedWeapon.AssaultRifle:
                Debug.Log("Picked up Assault Rifle!");
                if (!activeWeapons.Contains(assaultRifle))
                {
                    activeWeapons.Add(assaultRifle);
                }
                break;

            case SelectedWeapon.Shotgun:
                Debug.Log("Picked up Shotgun!");
                if (!activeWeapons.Contains(shotgun))
                {
                    activeWeapons.Add(shotgun);
                }
                break;

            case SelectedWeapon.Pistol:
                Debug.Log("Picked up pistol!");
                if (!activeWeapons.Contains(pistol))
                {
                    activeWeapons.Add(pistol);
                }
                break;

            case SelectedWeapon.Knife:
                Debug.Log("Picked up knife!");
                if (!activeWeapons.Contains(knife))
                {
                    activeWeapons.Add(knife);
                }
                break;

            case SelectedWeapon.Grenade:
                Debug.Log("Picked up grenade!");
                if (!activeWeapons.Contains(grenade))
                {
                    activeWeapons.Add(grenade);
                }
                break;

            // Add other cases...
            default:
                Debug.LogWarning("Unhandled weapon type.");
                break;
        }
    }
}