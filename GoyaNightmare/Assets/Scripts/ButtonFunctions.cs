using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonFunctions : MonoBehaviour
{
    // Scene Transitions
    public LevelController levelController;
    public CanvasGroup levelTransition;
    private Coroutine lastFadeRoutine = null;
    private int targetScene;
    private bool gameIsPaused;

    private void Start()
    {
        levelTransition.alpha = 1;
        gameIsPaused = false;

        // It is possible to overlap FadeIn and FadeOut Coroutines in an infinite loop of both.
        // This happens if the player tries to change the scene before the FadeIn has completed.
        // To avoid that we will store a ref to the last routine invoked, so we can stop it when starting another one.
        lastFadeRoutine = StartCoroutine(SceneFadeIn());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && levelController != null)
        {
            if (gameIsPaused) Resume();
            else Pause();
        }
    }

    #region Button Callbacks
    public void Play()
    {
        targetScene = SceneManager.GetActiveScene().buildIndex + 1; // Using buildIndex is faster than comparing names
        lastFadeRoutine = StartCoroutine(SceneFadeOut());
    }

    public void Pause()
    {
        Time.timeScale = 0;
        gameIsPaused = true;
        MouseLook.ToggleCameraAndCursor(false);
        levelController.OpenCanvas(LevelController.PAUSE_CANVAS_ID);
    }

    public void Resume()
    {
        Time.timeScale = 1;
        gameIsPaused = false;
        MouseLook.ToggleCameraAndCursor(true);
        levelController.CloseCanvas(LevelController.PAUSE_CANVAS_ID);
    }

    public void Restart()
    {
        targetScene = SceneManager.GetActiveScene().buildIndex;
        MouseLook.ToggleCameraAndCursor(false);
        Resume();
        lastFadeRoutine = StartCoroutine(SceneFadeOut());
    }

    public void ReturnToMenu()
    {
        targetScene = 0; // 0 = Main Menu scene
        MouseLook.ToggleCameraAndCursor(false);
        Resume();
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
        PlayerController.LockInputs(true);

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
        PlayerController.LockInputs(true);

        yield return new WaitForSeconds(.5f);
        while (levelTransition.alpha > 0)
        {
            levelTransition.alpha -= Time.deltaTime;
            yield return null;
        }

        levelTransition.gameObject.SetActive(false);
        PlayerController.LockInputs(false);
    }
    #endregion
}
