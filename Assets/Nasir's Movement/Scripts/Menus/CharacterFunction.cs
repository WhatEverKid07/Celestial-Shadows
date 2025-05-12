using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class CharacterFunction : MonoBehaviour
{
    private GameObject char1;

    private bool showCharacter;
    internal bool selected;

    private void Start()
    {
        char1 = GameObject.Find("Character1Sprite");
    }

    private void Update()
    {
        if (showCharacter)
        {
            char1.SetActive(true);
        }
        else
        {
            char1.SetActive(false);
        }
    }

    public void OnMouseEnter()
    {
        showCharacter = true;
    }

    public void OnMouseExit()
    {
        if (!selected)
        {
            showCharacter = false;
        }
    }

    public void OnMouseSelect()
    {
        selected = true;
    }
}
