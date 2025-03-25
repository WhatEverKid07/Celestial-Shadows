using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
public class ArsenalFunction : MonoBehaviour
{
    [Header("Scripts")]
    [SerializeField] private MainMenu mainMenuScript;

    [Header("Guns")]
    [SerializeField] private GameObject[] allGuns;
    private int currentIndex;
    private GameObject currentGun;
    private GameObject previousGun;
    private GameObject nextGun;
    private GameObject selectedGun;
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
            previousGun = null;
            currentGun = allGuns[0];
        }
    }

    private void Update()
    {
        currentIndex = System.Array.IndexOf(allGuns, currentGun);

        //Debug.Log("Is switching: " + isSwitching);
        //Debug.Log("Can switch up: " + canSwitchUp);
        //Debug.Log("Can switch down: " + canSwitchDown);

        //Debug.Log("The current index is: " +  currentIndex);
        //Debug.Log("Previous gun: " + previousGun);
        //Debug.Log("Current gun: " + currentGun);
        //Debug.Log("Next gun: " + nextGun);
        //Debug.Log("Selected gun: " + selectedGun);

        Debug.Log("GoToStory: " + mainMenuScript.goToStory);

        if (isSwitching)
        {
            return;
        }

        if (canSwitchUp && !isSwitching)
        {
            canSwitchUp = false;
            CanSwitchGunUp();
        }

        if (canSwitchDown && !isSwitching)
        {
            canSwitchDown = false;
            CanSwitchGunDown();
        }

    }

    public void ScrollMenuUp()
    {
        if (currentIndex < allGuns.Length - 1) 
        {
            canSwitchUp = true;
        }
    }

    public void ScrollMenuDown()
    {
        if (currentIndex > -1)
        {
            canSwitchDown = true;
        }
    }

    public void SelectWeapon()
    {
        selectedGun = currentGun;

        if (mainMenuScript.goToStory)
        {
            SceneManager.LoadScene("MovementTest", LoadSceneMode.Single);
        }
        else
        {
            SceneManager.LoadScene("EndlessMode", LoadSceneMode.Single);
        }
    }

    private void CanSwitchGunUp()
    {
        //Debug.Log("Current index: " + currentIndex);
        //Debug.Log("Previous gun: " + previousGun);
        //Debug.Log("Current gun: " + currentGun);
        Debug.Log("Next gun: " + nextGun);

        if (currentIndex < allGuns.Length - 1)
        {
            previousGun = currentGun;
            currentGun = allGuns[currentIndex + 1];
            nextGun = currentIndex + 2 < allGuns.Length ? allGuns[currentIndex + 2] : null;


            isSwitching = true;
            switchUpCoroutine = StartCoroutine(SwitchGunUp());
        }
    }

    private void CanSwitchGunDown()
    {
        //Debug.Log("Current index: " + currentIndex);
        //Debug.Log("Previous gun: " + previousGun);
        //Debug.Log("Current gun: " + currentGun);
        Debug.Log("Next gun: " + nextGun);

        if (currentIndex > 0)
        {
            previousGun = currentGun;
            currentGun = allGuns[currentIndex - 1];
            nextGun = currentIndex - 2 >= 0 ? allGuns[currentIndex - 2] : null;

            isSwitching = true;
            switchDownCoroutine = StartCoroutine(SwitchGunDown());
        }
        else
        {
            previousGun = allGuns[0];
        }

    }
    private IEnumerator SwitchGunUp()
    {
        float rotationTime = 1f;
        float elapsedTime = 0f;

        Vector3 currentLeft = new(currentOrientation.position.x + 15f, 0, currentOrientation.position.z);
        Vector3 nextLeft = new(currentOrientation.position.x - 15f, 0, currentOrientation.position.z);


        while (elapsedTime < rotationTime)
        {
            currentGun.transform.position = Vector3.Lerp(currentGun.transform.position, currentOrientation.position, switchSpeed * Time.deltaTime);
            previousGun.transform.position = Vector3.Lerp(previousGun.transform.position, currentLeft, switchSpeed * Time.deltaTime);
            if (nextGun != null)
            {
                nextGun.transform.position = Vector3.Lerp(nextGun.transform.position, nextLeft, switchSpeed * Time.deltaTime);
            }

            elapsedTime += Time.deltaTime;


            yield return null;
        }

        isSwitching = false;
    }

    private IEnumerator SwitchGunDown()
    {
        float rotationTime = 1f;
        float elapsedTime = 0f;

        Vector3 currentRight = new(currentOrientation.position.x - 15f, 0, currentOrientation.position.z);
        Vector3 nextRight = new(currentOrientation.position.x + 15f, 0, currentOrientation.position.z);

        while (elapsedTime < rotationTime)
        {
            currentGun.transform.position = Vector3.Lerp(currentGun.transform.position, currentOrientation.position, switchSpeed * Time.deltaTime);

            if(previousGun != null)
            {
                previousGun.transform.position = Vector3.Lerp(previousGun.transform.position, currentRight, switchSpeed * Time.deltaTime);
            }

            if (nextGun != null)
            {
                nextGun.transform.position = Vector3.Lerp(nextGun.transform.position, nextRight, switchSpeed * Time.deltaTime);
            }

            elapsedTime += Time.deltaTime;


            yield return null;
        }

        isSwitching = false;
    }
}
