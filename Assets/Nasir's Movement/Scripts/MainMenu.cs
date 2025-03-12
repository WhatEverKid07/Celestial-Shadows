using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private Transform menuCam;
    [SerializeField] private float rotationSpeed;
    private bool canRotate;

    [Header("Menus")]
    [SerializeField] private GameObject selectionMenu;
    [SerializeField] private GameObject characterMenu;
    [SerializeField] private GameObject arsenalMenu;
    private Transform currentMenu;

    private void Update()
    {
        if (canRotate)
        {
            menuCam.rotation = Quaternion.Slerp(menuCam.rotation, currentMenu.transform.rotation, rotationSpeed * Time.deltaTime);
            Invoke(nameof(StopRotate), rotationSpeed * Time.deltaTime);
            if (currentMenu == characterMenu)
            {
                menuCam.rotation = Quaternion.Slerp(menuCam.rotation, currentMenu.rotation, rotationSpeed * Time.deltaTime);
                Invoke(nameof(StopRotate), rotationSpeed * Time.deltaTime);
                selectionMenu.SetActive(false);
            }
        }
    }

    public void EnterCharacterMode()
    {
        canRotate = true;
        characterMenu.SetActive(true);
        currentMenu = characterMenu.transform;
    }

    public void Back()
    {
        canRotate = true;
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Exit");
    }

    private void StopRotate()
    {
        canRotate = false;
    }
}
