using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private Transform menuCam;
    [SerializeField] private float rotationSpeed;
    private bool canRotate;
    private bool goBack;

    [Header("Menus")]
    [SerializeField] private GameObject selectionMenu;
    [SerializeField] private GameObject characterMenu;
    [SerializeField] private GameObject arsenalMenu;
    [SerializeField] private static List<GameObject> menues = new List<GameObject>();
    private GameObject currentMenu;

    private void Start()
    {
        goBack = false;
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
            menuCam.rotation = Quaternion.Slerp(menuCam.rotation, currentMenu.transform.rotation, rotationSpeed * Time.deltaTime);
            Invoke(nameof(StopRotate), 5f);
        }
    }

    public void EnterCharacterMode()
    {
        canRotate = true;
        characterMenu.SetActive(true);
        menues.Add(characterMenu);
    }

    public void Back()
    {
        canRotate = true;
        goBack = true;
        menues.Remove(currentMenu);
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Exit");
    }

    private void StopRotate()
    {
        canRotate = false;
        goBack = false;
    }
}
