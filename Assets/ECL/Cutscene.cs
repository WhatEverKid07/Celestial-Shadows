using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class Cutscene : MonoBehaviour
{
    [SerializeField] private VideoPlayer cutscene;
    void Start()
    {
        StartCoroutine(CutsceneEnd());
    }

    private IEnumerator CutsceneEnd()
    {
        yield return new WaitForSeconds((float)cutscene.clip.length);
        SceneManager.LoadScene("MainMenu");
    }
}
