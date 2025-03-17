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
    private GameObject previousGun;
    [SerializeField] private Transform currentOrientation;

    private bool canSwitch;
    private bool canSwitchUp;
    private bool canSwitchDown;
    private bool isSwitching = false;

    private Coroutine switchCoroutine;
    [SerializeField] private float switchSpeed;

    private void Start()
    {
        mainMenuScript = FindAnyObjectByType<MainMenu>();
    }

    private void Update()
    {
        if ((canSwitchUp || canSwitchDown) && !isSwitching)
        {
            canSwitch = true;
        }

        if (canSwitch && !isSwitching)
        {
            isSwitching = true;
            switchCoroutine = StartCoroutine(SwitchGun());

            canSwitch = false;
        }

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
        int currentIndex = System.Array.IndexOf(allGuns, currentGun);
        if (currentIndex < allGuns.Length - 1) 
        {
            currentGun = allGuns[currentIndex + 1];
        }

        canSwitchUp = true;
    }

    public void ScrollMenuDown()
    {
        int currentIndex = System.Array.IndexOf(allGuns, currentGun);
        if (currentIndex > 0) 
        {
            currentGun = allGuns[currentIndex - 1];
        }

        canSwitchDown = true;
    }

    private IEnumerator SwitchGun()
    {
        float rotationTime = 1f;
        float elapsedTime = 0f;

        while (elapsedTime < rotationTime)
        {

            Vector3 moveLeft = new(currentOrientation.position.x + 15f, currentOrientation.position.y, currentOrientation.position.z);
            currentGun.transform.position = Vector3.Lerp(currentGun.transform.position, currentOrientation.position, switchSpeed * Time.deltaTime);

            elapsedTime += Time.deltaTime;


            yield return null;
        }
    }
}
