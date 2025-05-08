using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeathManager : MonoBehaviour
{
    [SerializeField] private Target playerHealth;
    [SerializeField] private Canvas playerDeathScreen;
    private bool isDead;
    void Start()
    {
        Time.timeScale = 1;
        isDead = false;
        playerDeathScreen.enabled = false;
    }
    void Update()
    {
        if (playerHealth.health <= 0 && !isDead)
        {
            ScreenEnable();
        }
    }
    public void ScreenEnable()
    {
        isDead = true;
        playerDeathScreen.enabled = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0;
    }
}