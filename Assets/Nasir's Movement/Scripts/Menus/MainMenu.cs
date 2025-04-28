using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
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
    private GameObject lobby;

    private GameObject selectionMenu;
    private GameObject characterMenu;
    public GameObject arsenalMenu { get; private set; }

    private GameObject settingsGO;
    private Canvas settingsMenu;

    private GameObject quitGO;
    private Canvas quitMenu;

    private GameObject constantMenuGO;
    private Canvas constantMenu;


    [SerializeField] private static List<GameObject> menus = new List<GameObject>();
    public GameObject currentMenu { get; private set; }
    private GameObject previousMenu;

    [Header("Buttons")]
    [SerializeField] private Button[] allButtons;

    private GameObject exitGO;
    private Button exitButton;
    private Image exitButtonImage;

    private GameObject backGO;
    private Button backButton;
    private Image backButtonImage;

    private GameObject settingsButtonGO;
    private Button settingsButton;
    private Image settingsButtonImage;

    public bool goToStory { get; private set; }
    public bool goToEndless { get; private set; }

    private void Awake()
    {
        selectionMenu = GameObject.Find("SelectionMenu");
        characterMenu = GameObject.Find("CharacterMenu");
        arsenalMenu = GameObject.Find("ArsenalMenu");

        lobby = GameObject.Find("Lobby");

        settingsGO = GameObject.Find("SettingsDisplay");
        settingsButtonGO = GameObject.Find("Settings");
        quitGO = GameObject.Find("QuitDisplay");
        exitGO = GameObject.Find("Exit");
        backGO = GameObject.Find("Back");
        constantMenuGO = GameObject.Find("MenuConstants");

        settingsMenu = settingsGO.GetComponentInChildren<Canvas>();
        quitMenu = quitGO.GetComponentInChildren<Canvas>();
        constantMenu = constantMenuGO.GetComponentInChildren<Canvas>();

        settingsMenu.enabled = false;
        quitMenu.enabled = false;

        exitButton = exitGO.GetComponentInChildren<Button>();
        backButton = backGO.GetComponentInChildren<Button>();
        settingsButton = settingsButtonGO.GetComponentInChildren<Button>();

        exitButtonImage = exitGO.GetComponentInChildren<Image>();
        backButtonImage = backGO.GetComponentInChildren<Image>();
        settingsButtonImage = settingsButton.GetComponentInChildren<Image>();
    }

    private void Start()
    {
        menuCam = Camera.main.transform;

        back = playerCntrlsAss.FindActionMap("Menu Controls").FindAction("GoBack");
        back.Enable();
        back.performed += ctx => GoBack();

        canRotate = false;

        allButtons = FindObjectsByType<Button>(FindObjectsSortMode.None);
        menus.Add(selectionMenu);
    }

    private void Update()
    {
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

        if (!settingsMenu.enabled && !quitMenu.enabled)
        {
            if(currentMenu == selectionMenu)
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
        else
        {
            ExitButtonOff();
            BackButtonOff();
        }

        if (currentMenu == selectionMenu)
        {
            lobby.transform.rotation = Quaternion.Slerp(lobby.transform.rotation, Quaternion.Euler(0f, -5f, 0f), (rotationSpeed * 2) * Time.deltaTime);
        }
        else if (currentMenu == characterMenu)
        {
            lobby.transform.rotation = Quaternion.Slerp(lobby.transform.rotation, Quaternion.Euler(0f, 5f, 0f), (rotationSpeed * 2) * Time.deltaTime);
        }
        else
        {
            lobby.transform.rotation = Quaternion.Slerp(lobby.transform.rotation, Quaternion.Euler(0f, 10f, 0f), (rotationSpeed * 2) * Time.deltaTime);
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

        if (currentMenu == characterMenu)
        {
            goToStory = false;
            goToEndless = false;
        }
    }

    public void EnterSettings()
    {
        settingsMenu.enabled = true;
        constantMenu.enabled = false;
        SettingsButtonOff();
        ExitButtonOff();
        BackButtonOff();
    }

    public void ExitSettings()
    {
        settingsMenu.enabled = false;
        constantMenu.enabled = true;
        SettingsButtonOn();
        ExitButtonOn();
        BackButtonOn();

    }

    public void EnterQuit()
    {
        quitMenu.enabled = true;
        constantMenu.enabled = false;
        SettingsButtonOff();
        ExitButtonOff();
        BackButtonOff();
    }

    public void ExitQuit()
    {
        quitMenu.enabled = false;
        constantMenu.enabled = true;
        SettingsButtonOn();
        ExitButtonOn();
        BackButtonOn();
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
        }

        float camRotationTime = 1.2f;
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
        exitButtonImage.enabled = true;
    }

    private void ExitButtonOff()
    {
        exitButton.interactable = false;
        exitButtonImage.enabled = false;
    }

    private void SettingsButtonOn()
    {
        settingsButton.interactable = true;
        settingsButtonImage.enabled = true;
    }

    private void SettingsButtonOff()
    {
        settingsButton.interactable = false;
        settingsButtonImage.enabled = false;
    }

    private void BackButtonOn()
    {
        backButton.interactable = true;
        backButtonImage.enabled = true;
    }

    private void BackButtonOff()
    {
        backButton.interactable = false;
        backButtonImage.enabled = false;
    }
}
