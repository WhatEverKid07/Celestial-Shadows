using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
public class ArsenalFunction : MonoBehaviour
{
    [Header("Scripts")]
    [SerializeField] private MainMenu mainMenuScript;

    [Header("Camera")]
    [SerializeField] private Transform gunCam;
    private Vector3 moveOffset;

    [Header("Guns")]
    [SerializeField] private GameObject[] allGuns;
    private int currentIndex;
    private GameObject currentGun;
    private GameObject previousGun;
    private GameObject nextGun;
    private GameObject selectedGun;

    private GameObject pistol;
    private GameObject shotgun;
    private GameObject ar;

    private bool canSwitchUp = false;
    private bool canSwitchDown = false;
    private bool isSwitching = false;

    private Coroutine switchUpCoroutine;
    private Coroutine switchDownCoroutine;
    [SerializeField] private float switchSpeed;

    private void Start()
    {
        gunCam = Camera.main.transform;

        mainMenuScript = FindAnyObjectByType<MainMenu>();

        if (allGuns.Length > 0)
        {
            previousGun = null;
            currentGun = allGuns[0];
            nextGun = allGuns[1];
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

        //Debug.Log("GoToStory: " + mainMenuScript.goToStory);
        //Debug.Log("GoToEndless: " + mainMenuScript.goToEndless);

        if (isSwitching)
        {
            return;
        }

        if (canSwitchUp && !isSwitching)
        {
            canSwitchUp = false;
            CanSwitchGunUp();
        }

        if ((canSwitchDown && !isSwitching) || mainMenuScript.currentMenu != mainMenuScript.arsenalMenu)
        {
            canSwitchDown = false;
            CanSwitchGunDown();
        }

        if (mainMenuScript.currentMenu == mainMenuScript.arsenalMenu)
        {
            gunCam.transform.position = Vector3.Lerp(gunCam.transform.position, new Vector3(currentGun.transform.position.x, currentGun.transform.position.y, currentGun.transform.position.z + 150f), switchSpeed * Time.deltaTime);
        }
        else
        {
            gunCam.transform.position = Vector3.Lerp(gunCam.transform.position, Vector3.zero, switchSpeed * Time.deltaTime);
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
        if (selectedGun == allGuns[0])
        {
            pistol = new GameObject("Pistol");
            DontDestroyOnLoad(pistol);
        }
        else if (selectedGun == allGuns[1])
        {
            shotgun = new GameObject("Shotgun");
            DontDestroyOnLoad(shotgun);
        }
        else
        {
            ar = new GameObject("AR");
            DontDestroyOnLoad(ar);
        }

        if (mainMenuScript.goToStory)
        {
            SceneManager.LoadScene("Story Mode", LoadSceneMode.Single);
        }
        else
        {
            SceneManager.LoadScene("open world", LoadSceneMode.Single);
        }
    }

    private void CanSwitchGunUp()
    {
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

        while (elapsedTime < rotationTime)
        {

            gunCam.transform.position = Vector3.Lerp(gunCam.transform.position, new Vector3(currentGun.transform.position.x, currentGun.transform.position.y, currentGun.transform.position.z + 150f), switchSpeed * Time.deltaTime);
            elapsedTime += Time.deltaTime;


            yield return null;
        }

        isSwitching = false;
    }

    private IEnumerator SwitchGunDown()
    {
        if (mainMenuScript.currentMenu == mainMenuScript.arsenalMenu)
        {
            float rotationTime = 1f;
            float elapsedTime = 0f;

            while (elapsedTime < rotationTime)
            {

                gunCam.transform.position = Vector3.Lerp(gunCam.transform.position, new Vector3(currentGun.transform.position.x, currentGun.transform.position.y, currentGun.transform.position.z + 150f), switchSpeed * Time.deltaTime);
                elapsedTime += Time.deltaTime;

                yield return null;
            }
        }

        isSwitching = false;
    }
}
