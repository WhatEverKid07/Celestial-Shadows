using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class GunManagement : MonoBehaviour
{
    [SerializeField] private List<GameObject> weapons;
    [SerializeField] private InputActionAsset gunControls;
    
    private int currentGunIndex = 0;
    internal bool canSwitch = true;
    private InputAction scroll;

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
        currentGunIndex = (currentGunIndex + 1) % weapons.Count;
        UpdateGunSelection();
    }

    void PreviousGun()
    {
        currentGunIndex = (currentGunIndex - 1 + weapons.Count) % weapons.Count;
        UpdateGunSelection();
    }

    void UpdateGunSelection()
    {
        // Deactivate all guns first
        foreach (GameObject gun in weapons)
        {
            gun.SetActive(false);
        }

        // Activate only the selected gun
        if (weapons.Count > 0)
        {
            weapons[currentGunIndex].SetActive(true);
        }
    }
}