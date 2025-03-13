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
    private InputAction goBack;

    [Header("Camera")]
    [SerializeField] private Transform menuCam;
    private float rotationSpeed = 5f;
    private bool canRotate;

    private Coroutine rotateCoroutine;

    [Header("Menus")]
    private GameObject selectionMenu;
    private GameObject characterMenu;
    private GameObject arsenalMenu;
    private GameObject settingsMenu;

    [SerializeField] private static List<GameObject> menues = new List<GameObject>();
    private GameObject currentMenu;

    [Header("Buttons")]
    [SerializeField] private Button[] allButtons; 

    [Header("Modes")]
    private bool goToStory;
    private bool goToEndless;

    private void Start()
    {
        menuCam = Camera.main.transform;

        goBack = playerCntrlsAss.FindActionMap("Menu Controls").FindAction("GoBack");
        goBack.performed += ctx => GoBack();

        selectionMenu = GameObject.Find("SelectionMenu");
        characterMenu = GameObject.Find("CharacterMenu");
        arsenalMenu = GameObject.Find("ArsenalMenu");
        settingsMenu = GameObject.Find("SettingsMenu");

        allButtons = FindObjectsByType<Button>(FindObjectsSortMode.None);

        canRotate = false;
        menues.Add(selectionMenu);
    }

    private void Update()
    {
        if (menues.Count > 0)
        {
            for (int i = 0; i < menues.Count; i++)
            {
                currentMenu = menues[i];
            }
        
        }

        if (canRotate)
        {
            foreach (Button button in allButtons)
            {
                button.interactable = false;
            }

           rotateCoroutine = StartCoroutine(RotateMenu());
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
        canRotate = true;
        menues.Remove(currentMenu);
    }

    public void EnterSettings()
    {
        settingsMenu.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Exit");
    }

    private IEnumerator RotateMenu()
    {
        float rotationTime = 1f;
        float elaspedTime = 0f;

        while (elaspedTime < rotationTime)
        {
            menuCam.rotation = Quaternion.Slerp(menuCam.rotation, currentMenu.transform.rotation, rotationSpeed * Time.deltaTime);
            elaspedTime += Time.deltaTime;

            yield return null;
        }

        foreach (Button button in allButtons)
        {
            button.interactable = true;
        }

    }
}
