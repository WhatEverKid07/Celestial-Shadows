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

    private bool canSwitchUp;
    private bool canSwitchDown;
    private bool isSwitching = false;

    private Coroutine switchUpCoroutine;
    private Coroutine switchDownCoroutine;
    [SerializeField] private float switchSpeed;

    private void Start()
    {
        mainMenuScript = FindAnyObjectByType<MainMenu>();

        if (allGuns.Length > 0)
        {
            currentGun = allGuns[0]; 
        }
    }

    private void Update()
    {

        if (canSwitchUp && !isSwitching)
        {
            isSwitching = true;
            switchUpCoroutine = StartCoroutine(SwitchGunUp());

            canSwitchUp = false;
        }

        if (canSwitchDown && !isSwitching)
        {
            isSwitching = true;
            switchDownCoroutine = StartCoroutine(SwitchGunDown());

            canSwitchDown = false;
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
        currentIndex = currentIndex < 0 ? 0 : currentIndex;

        Debug.Log("Current index: " + currentIndex);
        Debug.Log("Previous gun" + previousGun);
        Debug.Log("Current gun: " + currentGun);

        if (currentIndex < allGuns.Length) 
        {
            currentGun = allGuns[currentIndex + 1];

            if (currentIndex != 0)
            {
                previousGun = allGuns[currentIndex - 1];
            }
            else
            {
                previousGun = allGuns[currentIndex];
            }

            canSwitchUp = true;
        }
    }

    public void ScrollMenuDown()
    {
        int currentIndex = System.Array.IndexOf(allGuns, currentGun);
        currentIndex = currentIndex < 0 ? 0 : currentIndex;

        Debug.Log("Current index: " + currentIndex);
        Debug.Log("Previous gun" + previousGun);
        Debug.Log("Current gun: " + currentGun);

        if (currentIndex > 0) 
        {
            currentGun = allGuns[currentIndex - 1];
            previousGun = allGuns[currentIndex - 2];
            canSwitchDown = true;
        }
    }

    private IEnumerator SwitchGunUp()
    {
        float rotationTime = 1f;
        float elapsedTime = 0f;

        while (elapsedTime < rotationTime)
        {

            Vector3 moveLeft = new(currentOrientation.position.x + 15f, currentOrientation.position.y, currentOrientation.position.z);
            currentGun.transform.position = Vector3.Lerp(currentGun.transform.position, currentOrientation.position, switchSpeed * Time.deltaTime);
            previousGun.transform.position = Vector3.Lerp(previousGun.transform.position, moveLeft, switchSpeed * Time.deltaTime);

            elapsedTime += Time.deltaTime;


            yield return null;
        }

        isSwitching = false;
    }

    private IEnumerator SwitchGunDown()
    {
        float rotationTime = 1f;
        float elapsedTime = 0f;

        while (elapsedTime < rotationTime)
        {

            Vector3 moveRight = new(currentOrientation.position.x - 15f, currentOrientation.position.y, currentOrientation.position.z);
            previousGun.transform.position = Vector3.Lerp(previousGun.transform.position, currentOrientation.position, switchSpeed * Time.deltaTime);
            currentGun.transform.position = Vector3.Lerp(currentGun.transform.position, moveRight, switchSpeed * Time.deltaTime);

            elapsedTime += Time.deltaTime;


            yield return null;
        }

        isSwitching = false;
    }
}
