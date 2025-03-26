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
    [SerializeField] private Transform previousOrientation;
    [SerializeField] private Transform nextOrientation;

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
            SceneManager.LoadScene("MovementTest", LoadSceneMode.Single);
        }
        else
        {
            SceneManager.LoadScene("EndlessMode", LoadSceneMode.Single);
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
        Vector3 moveOffset = new(15f, 0f, 0f);
        float rotationTime = 1f;
        float elapsedTime = 0f;

        while (elapsedTime < rotationTime)
        {
            currentGun.transform.position = Vector3.Lerp(currentGun.transform.position, currentOrientation.position, switchSpeed * Time.deltaTime);
            previousGun.transform.position = Vector3.Lerp(previousGun.transform.position, previousOrientation.position, switchSpeed * Time.deltaTime);
            if (nextGun != null)
            {
                nextGun.transform.position = Vector3.Lerp(nextGun.transform.position, nextOrientation.position, switchSpeed * Time.deltaTime);
            }

            foreach (GameObject gun in allGuns)
            {
                if (gun != nextGun && gun != currentGun && gun != previousGun)
                {
                    gun.transform.position = Vector3.Lerp(gun.transform.position, gun.transform.position + moveOffset, (switchSpeed * Time.deltaTime));
                }
            }

            elapsedTime += Time.deltaTime;


            yield return null;
        }

        isSwitching = false;
    }

    private IEnumerator SwitchGunDown()
    {
        Vector3 moveOffset = new(-15f, 0f, 0f);

        if (mainMenuScript.currentMenu == mainMenuScript.arsenalMenu)
        {
            float rotationTime = 1f;
            float elapsedTime = 0f;

            while (elapsedTime < rotationTime)
            {
                currentGun.transform.position = Vector3.Lerp(currentGun.transform.position, currentOrientation.position, switchSpeed * Time.deltaTime);

                if (previousGun != null)
                {
                    previousGun.transform.position = Vector3.Lerp(previousGun.transform.position, nextOrientation.position, switchSpeed * Time.deltaTime);
                }

                if (nextGun != null)
                {
                    nextGun.transform.position = Vector3.Lerp(nextGun.transform.position, previousOrientation.position, switchSpeed * Time.deltaTime);
                }

                foreach (GameObject gun in allGuns)
                {
                    if (gun != nextGun && gun != currentGun && gun != previousGun)
                    {
                        gun.transform.position = Vector3.Lerp(gun.transform.position, gun.transform.position + moveOffset, switchSpeed * Time.deltaTime);
                    }
                }

                elapsedTime += Time.deltaTime;


                yield return null;
            }
        }
        else
        {
            float rotationTime = .5f;
            float elapsedTime = 0f;

            while (elapsedTime < rotationTime)
            {
                currentGun.transform.position = Vector3.Lerp(currentGun.transform.position, currentOrientation.position, (switchSpeed * Time.deltaTime) * 2);

                if (previousGun != null)
                {
                    previousGun.transform.position = Vector3.Lerp(previousGun.transform.position, nextOrientation.position, (switchSpeed * Time.deltaTime) * 3);
                }

                if (nextGun != null)
                {
                    nextGun.transform.position = Vector3.Lerp(nextGun.transform.position, previousOrientation.position, (switchSpeed * Time.deltaTime) * 3);
                }

                foreach(GameObject gun in allGuns)
                {
                    if (gun != nextGun && gun != currentGun && gun != previousGun)
                    {
                        gun.transform.position = Vector3.Lerp(gun.transform.position, moveOffset, (switchSpeed * Time.deltaTime) * 3);
                    }
                }

                elapsedTime += Time.deltaTime;


                yield return null;
            }
        }

        isSwitching = false;
    }
}
