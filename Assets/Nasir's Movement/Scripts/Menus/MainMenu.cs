using System.Collections;
using System.Collections.Generic;
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
    private bool rotateBack;
    private bool isRotating = false;

    private Coroutine rotateCoroutine;
    private Coroutine rotateBackCoroutine;

    [Header("Menus")]
    private GameObject lobby;

    private GameObject selectionMenu;
    private GameObject characterMenu;
    public GameObject arsenalMenu { get; private set; }
    private Canvas weaponMenu;
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

    private Button storyModeButton;
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
        weaponMenu = arsenalMenu.GetComponentInChildren<Canvas>();

        settingsMenu.enabled = false;
        quitMenu.enabled = false;

        storyModeButton = selectionMenu.GetComponentInChildren<Button>();
        exitButton = exitGO.GetComponentInChildren<Button>();
        backButton = backGO.GetComponentInChildren<Button>();
        settingsButton = settingsButtonGO.GetComponentInChildren<Button>();

        exitButtonImage = exitGO.GetComponentInChildren<Image>();
        backButtonImage = backGO.GetComponentInChildren<Image>();
        settingsButtonImage = settingsButton.GetComponentInChildren<Image>();

        menus.Clear();
    }

    private void Start()
    {
        menuCam = Camera.main.transform;

        back = playerCntrlsAss.FindActionMap("Menu Controls").FindAction("GoBack");
        back.Enable();
        back.performed += ctx => GoBack();

        canRotate = false;

        allButtons = FindObjectsByType<Button>(FindObjectsSortMode.None);
        storyModeButton.interactable = false;
        menus.Add(selectionMenu);

        if (GameObject.Find("WowAR"))
        {
            Destroy(GameObject.Find("WowAR"));
        }

        if (GameObject.Find("WowPistol"))
        {
            Destroy(GameObject.Find("WowPistol"));
        }

        if (GameObject.Find("WowShotgun"))
        {
            Destroy(GameObject.Find("WowShotgun"));
        }

        if (GameObject.Find("[Debug Updater]"))
        {
            Destroy(GameObject.Find("[Debug Updater]"));
        }

        if (characterMenu == null)
        {
            Debug.LogError("CharacterMenu is null — make sure it exists in the scene!");
            return;
        }

        Time.timeScale = 1.0f;
    }

    private void Update()
    {
        if (menus.Count > 0)
        {
            for (int i = 0; i < menus.Count; i++)
            {
                currentMenu = menus[menus.Count - 1];
                previousMenu = menus.Count > 1 ? menus[menus.Count - 2] : null;
            }
        }

        Debug.Log("Current menu: " + currentMenu);
        Debug.Log("Previous menu: " + previousMenu);

        if (canRotate && !isRotating && !rotateBack)
        {
            foreach (Button button in allButtons)
            {
                if (button != exitButton && button != backButton && button != storyModeButton)
                {
                    button.interactable = false;
                }
            }

            isRotating = true;
            rotateCoroutine = StartCoroutine(RotateMenuCamera());

            canRotate = false;
        }

        if (rotateBack && !isRotating && !canRotate)
        {
            foreach (Button button in allButtons)
            {
                if (button != exitButton && button != backButton && button != storyModeButton)
                {
                    button.interactable = false;
                }
            }

            isRotating = true;
            rotateBackCoroutine = StartCoroutine(RotateMenuCameraBack());

            rotateBack = false;
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

            if (currentMenu == arsenalMenu)
            {
                weaponMenu.enabled = true;
            }
            else
            {
                weaponMenu.enabled = false;
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
        if (!canRotate && !rotateBack && !isRotating && currentMenu != selectionMenu)
        {
            rotateBack = true;
            //menus.Remove(currentMenu);
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
        weaponMenu.enabled = false;
        SettingsButtonOff();
        ExitButtonOff();
        BackButtonOff();
    }

    public void ExitSettings()
    {
        settingsMenu.enabled = false;
        constantMenu.enabled = true;
        weaponMenu.enabled = true;
        SettingsButtonOn();
        ExitButtonOn();
        BackButtonOn();

    }

    public void EnterQuit()
    {
        quitMenu.enabled = true;
        constantMenu.enabled = false;
        weaponMenu.enabled = false;
        SettingsButtonOff();
        ExitButtonOff();
        BackButtonOff();
    }

    public void ExitQuit()
    {
        quitMenu.enabled = false;
        constantMenu.enabled = true;
        weaponMenu.enabled = true;
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
            currentMenu.transform.rotation = Quaternion.Slerp(currentMenu.transform.rotation, Quaternion.Euler(0f, currentMenu.transform.rotation.eulerAngles.y - 10f, 0f), (rotationSpeed / 2) * Time.deltaTime);
            if (previousMenu != null)
            {
                previousMenu.transform.rotation = Quaternion.Slerp(previousMenu.transform.rotation, Quaternion.Euler(0f, previousMenu.transform.rotation.eulerAngles.y + 10f, 0f), (rotationSpeed / 2) * Time.deltaTime);

                elaspedTime += Time.deltaTime;
            }
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
            if (button != exitButton && button != backButton && button != storyModeButton)
            {
                button.interactable = true;
            }
        }

        isRotating = false;

    }

    private IEnumerator RotateMenuCameraBack()
    {
        float rotationTime = .1f;
        float elaspedTime = 0f;

        while (elaspedTime < rotationTime)
        {
            if (currentMenu != null)
            {
                currentMenu.transform.rotation = Quaternion.Lerp(currentMenu.transform.rotation, Quaternion.Euler(0f, currentMenu.transform.rotation.eulerAngles.y + 10f, 0f), (rotationSpeed / 2) * Time.deltaTime);
            }
           
            if (previousMenu != null)
            {
                previousMenu.transform.rotation = Quaternion.Lerp(previousMenu.transform.rotation, Quaternion.Euler(0f, previousMenu.transform.rotation.eulerAngles.y - 10f, 0f), (rotationSpeed / 2) * Time.deltaTime);

                elaspedTime += Time.deltaTime;
            }
        }

        float camRotationTime = 1.2f;
        float camElaspedTime = 0f;

        if (elaspedTime > rotationTime)
        {
            while (camElaspedTime < camRotationTime)
            {
                menuCam.rotation = Quaternion.Lerp(menuCam.rotation, previousMenu.transform.rotation, rotationSpeed * Time.deltaTime);
                camElaspedTime += Time.deltaTime;

                yield return null;
            }
        }

        foreach (Button button in allButtons)
        {
            if (button != exitButton && button != backButton && button != storyModeButton)
            {
                button.interactable = true;
            }
        }

        isRotating = false;

        if (menus.Count > 1)
        {
            menus.RemoveAt(menus.Count - 1);
        }
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
