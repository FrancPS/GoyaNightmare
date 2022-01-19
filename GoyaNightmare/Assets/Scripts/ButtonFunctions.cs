using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonFunctions : MonoBehaviour
{
    // Scene Transitions
    public CanvasGroup levelTransition;
    private Coroutine lastFadeRoutine = null;
    private int targetScene;

    private void Awake()
    {
        levelTransition.alpha = 1;
    }

    private void Start()
    {
        // It is possible to overlap FadeIn and FadeOut Coroutines in an infinite loop of both.
        // This happens if the player tries to change the scene before the FadeIn has completed.
        // To avoid that we will store a ref to the last routine invoked, so we can stop it when starting another one.
        lastFadeRoutine = StartCoroutine(SceneFadeIn());
    }

    #region Button Callbacks
    public void Play()
    {
        targetScene = SceneManager.GetActiveScene().buildIndex + 1; // Using buildIndex is faster than comparing names
        lastFadeRoutine = StartCoroutine(SceneFadeOut());
    }

    public void Restart()
    {
        targetScene = SceneManager.GetActiveScene().buildIndex;
        MouseLook.ToggleCameraAndCursor(false);
        lastFadeRoutine = StartCoroutine(SceneFadeOut());
    }

    public void ReturnToMenu()
    {
        targetScene = 0; // 0 = Main Menu scene
        MouseLook.ToggleCameraAndCursor(false);
        lastFadeRoutine = StartCoroutine(SceneFadeOut());
    }

    public void ExitGame()
    {
        Application.Quit();
    }
    #endregion

    #region Scene Transitions
    IEnumerator SceneFadeOut()
    {
        if (lastFadeRoutine != null) StopCoroutine(lastFadeRoutine);

        levelTransition.gameObject.SetActive(true);

        while (levelTransition.alpha < 1)
        {
            levelTransition.alpha += Time.deltaTime;
            yield return null;
        }

        SceneManager.LoadScene(targetScene);
    }

    IEnumerator SceneFadeIn()
    {
        levelTransition.gameObject.SetActive(true);

        yield return new WaitForSeconds(.5f);
        while (levelTransition.alpha > 0)
        {
            levelTransition.alpha -= Time.deltaTime;
            yield return null;
        }

        levelTransition.gameObject.SetActive(false);
    }
    #endregion
}
