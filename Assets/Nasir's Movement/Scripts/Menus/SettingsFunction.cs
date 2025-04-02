using UnityEngine;
using UnityEngine.UI;

public class SettingsFunction : MonoBehaviour
{
    [Header("Scripts")]
    [SerializeField] private MainMenu mainMenuScript;

    [Header("Settings")]
    private GameObject generalMenu;
    private Image generalMenuBG;

    private GameObject inGameMenu;
    private Image inGameMenuBG;

    private GameObject graphicsMenu;
    private Image graphicsMenuBG;

    private GameObject creditsMenu;
    private Image creditsBG;

    /*
    private void Start()
    {
        mainMenuScript = FindAnyObjectByType<MainMenu>();

        generalMenuBG = GameObject.Find("GeneralBG");
        inGameMenuBG = GameObject.Find("In-GameBG");
        graphicsMenuBG = GameObject.Find("GraphicsBG");
        creditsBG = GameObject.Find("CreditsBG");

        generalMenuBG.SetActive(true);
        inGameMenuBG.SetActive(false);
        graphicsMenuBG.SetActive(false);
        creditsBG.SetActive(false);
    }

    public void GeneralMenu()
    {
        generalMenuBG.SetActive(true);
        inGameMenuBG.SetActive(false);
        graphicsMenuBG.SetActive(false);
        creditsBG.SetActive(false);
    }

    public void InGameMenu()
    {
        inGameMenuBG.SetActive(true);
        generalMenuBG.SetActive(false);
        graphicsMenuBG.SetActive(false);
        creditsBG.SetActive(false);
    }

    public void GraphicsMenu()
    {
        graphicsMenuBG.SetActive(true);
        generalMenuBG.SetActive(false);
        inGameMenuBG.SetActive(false);
        creditsBG.SetActive(false);
    }

    public void CreditsMenu()
    {
        creditsBG.SetActive(true);
        generalMenuBG.SetActive(false);
        graphicsMenuBG.SetActive(false);
        creditsBG.SetActive(false);
    }
    */
}
