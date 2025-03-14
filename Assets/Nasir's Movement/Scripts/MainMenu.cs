using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    private GameObject settingsGO;
    private Canvas settingsMenu;
    private GameObject quitMenu;

    [SerializeField] private static List<GameObject> menus = new List<GameObject>();
    private GameObject currentMenu;
    private GameObject previousMenu;

    [Header("Buttons")]
    [SerializeField] private Button[] allButtons;
    private GameObject exitGO;
    private GameObject backGO;
    private Button exitButton;
    private TextMeshProUGUI exitButtonTxt;
    private Button backButton;
    private TextMeshProUGUI backButtonTxt;

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
        settingsGO = GameObject.Find("SettingsMenu");
        quitMenu = GameObject.Find("QuitMenu");
        exitGO = GameObject.Find("Exit");
        backGO = GameObject.Find("Back");

        settingsMenu = settingsGO.GetComponentInChildren<Canvas>();

        exitButton = exitGO.GetComponentInChildren<Button>();
        backButton = backGO.GetComponentInChildren<Button>();

        exitButtonTxt = exitButton.GetComponentInChildren<TextMeshProUGUI>();
        backButtonTxt = backButton.GetComponentInChildren<TextMeshProUGUI>();

        allButtons = FindObjectsByType<Button>(FindObjectsSortMode.None);

        canRotate = false;
        menus.Add(selectionMenu);
    }

    private void Update()
    {
        Debug.Log(isRotating);

        if (menus.Count > 0)
        {
            for (int i = 0; i < menus.Count; i++)
            {
                currentMenu = menus[i];
                previousMenu = menus[menus.Count - 1];
            }
        
        }

        if (canRotate && !isRotating)
        {
            foreach (Button button in allButtons)
            {
                if (button != exitButton && button != backButton)
                {
                    button.interactable = false;
                }
            }

            isRotating = true;
            rotateCoroutine = StartCoroutine(RotateMenuCamera());

            canRotate = false;
        }

        if (currentMenu == selectionMenu)
        {
            ExitButtonOn();
            BackButtonOff();
        }
        else
        {
            ExitButtonOff();
            BackButtonOn();
        }
    }

    public void EnterStory()
    {
        goToStory = true;
        goToEndless = false;

        canRotate = true;
        characterMenu.SetActive(true);
        menus.Add(characterMenu);
    }

    public void EnterEndless()
    {
        goToStory = false;
        goToEndless = true;

        canRotate = true;
        characterMenu.SetActive(true);
        menus.Add(characterMenu);
    }

    public void EnterArsenal()
    {
        canRotate = true;
        menus.Add(arsenalMenu);
    }

    public void GoBack()
    {
        if (!canRotate && !isRotating && currentMenu != selectionMenu)
        {
            canRotate = true;
            menus.Remove(currentMenu);
        }
    }

    public void EnterSettings()
    {
        settingsMenu.enabled = true;
    }

    public void QuitGame()
    {
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
            if (button != exitButton && button != backButton)
            {
                button.interactable = true;
            }
        }


        isRotating = false;

    }

    private void ExitButtonOn()
    {
        exitButton.interactable = true;
        exitButtonTxt.enabled = true;
    }

    private void ExitButtonOff()
    {
        exitButton.interactable = false;
        exitButtonTxt.enabled = false;
    }

    private void BackButtonOn()
    {
        backButton.interactable = true;
        backButtonTxt.enabled = true;
    }

    private void BackButtonOff()
    {
        backButton.interactable = false;
        backButtonTxt.enabled = false;
    }
}
