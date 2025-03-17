using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MenuFunctions : MonoBehaviour
{
    [Header("Scripts")]
    [SerializeField] private MainMenu mainMenuScript;

    [Header("Guns")]
    [SerializeField] private GameObject[] allGuns;
    private GameObject currentGun;

    private void Start()
    {
        mainMenuScript = FindAnyObjectByType<MainMenu>();
        currentGun = allGuns[0];
    }

    private void Update()
    {

    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void CancelQuit()
    {
        mainMenuScript.ExitQuit();
    }

    public void CancelSettings()
    {
        mainMenuScript.ExitSettings();
    }

    public void ScrollMenuUp()
    {

    }

    public void ScrollMenuDown()
    {

    }
}
