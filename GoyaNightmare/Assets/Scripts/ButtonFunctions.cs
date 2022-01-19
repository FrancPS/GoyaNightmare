using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonFunctions : MonoBehaviour
{
    
    public GameObject pauseCanvas;

    bool isPaused = false;
    MouseLook mouseLook;

    // Scene Transitions
    public CanvasGroup levelTransition;
    private Coroutine lastFadeRoutine = null;
    private int targetScene;

    private void Awake()
    {
        mouseLook = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<MouseLook>();
        levelTransition.alpha = 1;
    }

    private void Start()
    {
        // It is possible to overlap FadeIn and FadeOut Coroutines in an infinite loop of both.
        // This happens if the player tries to change the scene before the FadeIn has completed.
        // To avoid that we will store a ref to the last routine invoked, so we can stop it when starting another one.
        lastFadeRoutine = StartCoroutine(SceneFadeIn());
    }

    private void Update()
    {
        if (pauseCanvas && Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {       
                PauseGame(false);
            }
            else
            {
                PauseGame(true);
            }
        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void PauseGame(bool pause)
    {
        if (!pauseCanvas) return;

        isPaused = pause;
        pauseCanvas.SetActive(isPaused);

        if (isPaused) { Time.timeScale = 0; }
        else { Time.timeScale = 1; }

        mouseLook.AllowCameraRotation(!isPaused);
        mouseLook.ActivateCursor(isPaused);
    }



    #region Scene Transitions
    public void Play()
    {
        targetScene = SceneManager.GetActiveScene().buildIndex + 1; // Using buildIndex is faster than comparing names
        lastFadeRoutine = StartCoroutine(SceneFadeOut());
    }

    public void Restart()
    {
        targetScene = SceneManager.GetActiveScene().buildIndex;
        lastFadeRoutine = StartCoroutine(SceneFadeOut());
        PauseGame(false);
    }

    public void ReturnToMenu()
    {
        targetScene = 0; // 0 = Main Menu scene
        PauseGame(false);
        //mouseLook.AllowCameraRotation(false);
        //mouseLook.ActivateCursor(true);
        lastFadeRoutine = StartCoroutine(SceneFadeOut());
    }

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
