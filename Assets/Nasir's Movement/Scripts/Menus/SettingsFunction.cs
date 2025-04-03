using UnityEngine;
using UnityEngine.UI;

public class SettingsFunction : MonoBehaviour
{
    [Header("Scripts")]
    private MainMenu mainMenuScript;

    [Header("Settings")]
    private GameObject generalMenu;
    private Image generalMenuBG;

    private GameObject inGameMenu;
    private Image inGameMenuBG;

    private GameObject graphicsMenu;
    private Image graphicsMenuBG;

    private GameObject creditsMenu;
    private Image creditsBG;

    private void Start()
    {
        mainMenuScript = FindAnyObjectByType<MainMenu>();

        generalMenu = GameObject.Find("GeneralBG");
        inGameMenu = GameObject.Find("In-GameBG");
        graphicsMenu = GameObject.Find("GraphicsBG");
        creditsMenu = GameObject.Find("CreditsBG");

        generalMenuBG = generalMenu.GetComponent<Image>();
        inGameMenuBG = inGameMenu.GetComponent<Image>();
        graphicsMenuBG = graphicsMenu.GetComponent<Image>();
        creditsBG = creditsMenu.GetComponent<Image>();


        generalMenuBG.enabled = true;
        inGameMenuBG.enabled = false;
        graphicsMenuBG.enabled = false;
        creditsBG.enabled = false;
    }

    public void GeneralMenu()
    {
        generalMenuBG.enabled = true;
        inGameMenuBG.enabled = false;
        graphicsMenuBG.enabled = false;
        creditsBG.enabled = false;
    }

    public void InGameMenu()
    {
        inGameMenuBG.enabled = true;
        generalMenuBG.enabled = false;
        graphicsMenuBG.enabled = false;
        creditsBG.enabled = false;
    }

    public void GraphicsMenu()
    {
        graphicsMenuBG.enabled = true;
        generalMenuBG.enabled = false;
        inGameMenuBG.enabled = false;
        creditsBG.enabled = false;
    }

    public void CreditsMenu()
    {
        creditsBG.enabled = true;
        generalMenuBG.enabled = false;
        graphicsMenuBG.enabled = false;
        inGameMenuBG.enabled = false;
    }

}
