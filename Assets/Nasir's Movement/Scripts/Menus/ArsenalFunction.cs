using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArsenalFunction : MonoBehaviour
{
    [Header("Scripts")]
    [SerializeField] private MainMenu mainMenuScript;

    [Header("Guns")]
    [SerializeField] private GameObject[] allGuns;
    private GameObject currentGun;
    private GameObject previousGun;
    [SerializeField] private Transform currentOrientation;

    private bool canSwitchUp = false;
    private bool canSwitchDown = false;
    private bool isSwitching = false;

    private Coroutine switchUpCoroutine;
    private Coroutine switchDownCoroutine;
    [SerializeField] private float switchSpeed;

    private void Start()
    {
        mainMenuScript = FindAnyObjectByType<MainMenu>();

        if (allGuns.Length > 0)
        {
            previousGun = currentGun;
        }
    }

    private void Update()
    {
        Debug.Log("Previous gun: " + previousGun);
        Debug.Log("Current gun: " + currentGun);


        if (canSwitchUp && !isSwitching)
        {
            CanSwitchGunUp();
        }

        if (canSwitchDown && !isSwitching)
        {
            CanSwitchGunDown();
        }

    }

    public void ScrollMenuUp()
    {
        if (currentGun != allGuns[1])
        {
            canSwitchUp = true;
        }
    }

    public void ScrollMenuDown()
    {
        if (currentGun != allGuns[0])
        {
            canSwitchDown = true;
        }
    }

    private void CanSwitchGunUp()
    {
        int currentIndex = System.Array.IndexOf(allGuns, currentGun);

        Debug.Log("Current index: " + currentIndex);
        Debug.Log("Previous gun: " + previousGun);
        Debug.Log("Current gun: " + currentGun);

        if (currentIndex < allGuns.Length - 1)
        {
            currentGun = allGuns[currentIndex + 1];
            previousGun = currentIndex > 0 ? allGuns[currentIndex - 1] : currentGun;
        }

        isSwitching = true;
        switchUpCoroutine = StartCoroutine(SwitchGunUp());

        canSwitchUp = false;

    }

    private void CanSwitchGunDown()
    {
        int currentIndex = System.Array.IndexOf(allGuns, currentGun);

        Debug.Log("Current index: " + currentIndex);
        Debug.Log("Previous gun: " + previousGun);
        Debug.Log("Current gun: " + currentGun);

        if (currentIndex > 0)
        {
            currentGun = allGuns[currentIndex - 1];
            previousGun = currentIndex > 1 ? allGuns[currentIndex - 2] : allGuns[0];
        }

        isSwitching = true;
        switchDownCoroutine = StartCoroutine(SwitchGunDown());

        canSwitchDown = false;

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
