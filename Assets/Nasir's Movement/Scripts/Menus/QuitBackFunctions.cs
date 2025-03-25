using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class QuitBackFunctions : MonoBehaviour
{
    [Header("Scripts")]
    [SerializeField] private MainMenu mainMenuScript;

    private void Start()
    {
        mainMenuScript = FindAnyObjectByType<MainMenu>();
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
}
