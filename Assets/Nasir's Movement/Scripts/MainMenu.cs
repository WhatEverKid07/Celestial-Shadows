using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Android;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("Inputs")]
    [SerializeField] private InputActionAsset playerCntrlsAss;
    private InputAction back;

    [Header("Camera")]
    [SerializeField] private Transform menuCam;
    private float rotationSpeed = 5f;
    private bool canRotate;
    private bool isRotating = false;

    private Coroutine rotateCoroutine;

    [Header("Menus")]
    private GameObject selectionMenu;
    private GameObject characterMenu;
    private GameObject arsenalMenu;
    private GameObject settingsMenu;
    private GameObject quitMenu;

    [SerializeField] private static List<GameObject> menues = new List<GameObject>();
    private GameObject currentMenu;
    private GameObject previousMenu;

    [Header("Buttons")]
    [SerializeField] private Button[] allButtons; 

    [Header("Modes")]
    private bool goToStory;
    private bool goToEndless;

    private void Start()
    {
        menuCam = Camera.main.transform;

        back = playerCntrlsAss.FindActionMap("Menu Controls").FindAction("GoBack");
        back.Enable();
        back.performed += ctx => GoBack();

        selectionMenu = GameObject.Find("SelectionMenu");
        characterMenu = GameObject.Find("CharacterMenu");
        arsenalMenu = GameObject.Find("ArsenalMenu");
        settingsMenu = GameObject.Find("SettingsMenu");
        quitMenu = GameObject.Find("QuitMenu");

        allButtons = FindObjectsByType<Button>(FindObjectsSortMode.None);

        canRotate = false;
        menues.Add(selectionMenu);
    }

    private void Update()
    {
        Debug.Log(isRotating);
        if (menues.Count > 0)
        {
            for (int i = 0; i < menues.Count; i++)
            {
                currentMenu = menues[i];
                previousMenu = menues[menues.Count - 1];
            }
        
        }

        if (canRotate && !isRotating)
        {
            foreach (Button button in allButtons)
            {
                button.interactable = false;
            }

            isRotating = true;
            rotateCoroutine = StartCoroutine(RotateMenuCamera());

            canRotate = false;
        }
    }

    public void EnterStory()
    {
        goToStory = true;
        goToEndless = false;

        canRotate = true;
        characterMenu.SetActive(true);
        menues.Add(characterMenu);
    }

    public void EnterEndless()
    {
        goToStory = false;
        goToEndless = true;

        canRotate = true;
        characterMenu.SetActive(true);
        menues.Add(characterMenu);
    }

    public void EnterArsenal()
    {
        canRotate = true;
        menues.Add(arsenalMenu);
    }

    public void GoBack()
    {
        if (!canRotate && !isRotating && currentMenu != selectionMenu)
        {
            canRotate = true;
            menues.Remove(currentMenu);
        }
    }

    public void EnterSettings()
    {
        settingsMenu.SetActive(true);
    }

    public void QuitGame()
    {
        quitMenu.SetActive(true);
    }

    private IEnumerator RotateMenuCamera()
    {
        float rotationTime = .1f;
        float elaspedTime = 0f;

        while (elaspedTime < rotationTime)
        {
            currentMenu.transform.rotation = Quaternion.Slerp(currentMenu.transform.rotation, Quaternion.Euler(0f, currentMenu.transform.rotation.eulerAngles.y - 10f, 0f), (rotationSpeed/2) * Time.deltaTime);
            previousMenu.transform.rotation = Quaternion.Slerp(previousMenu.transform.rotation, Quaternion.Euler(0f, previousMenu.transform.rotation.eulerAngles.y + 10f, 0f), (rotationSpeed / 2) * Time.deltaTime);
            elaspedTime += Time.deltaTime;

            yield return null;
        }

        float camRotationTime = 1f;
        float camElaspedTime = 0f;

        if (elaspedTime > rotationTime)
        {
            while (camElaspedTime < camRotationTime)
            {
                menuCam.rotation = Quaternion.Slerp(menuCam.rotation, currentMenu.transform.rotation, rotationSpeed * Time.deltaTime);
                camElaspedTime += Time.deltaTime;

                yield return null;
            }
        }

        foreach (Button button in allButtons)
        {
            button.interactable = true;
        }

        isRotating = false;

    }
}
