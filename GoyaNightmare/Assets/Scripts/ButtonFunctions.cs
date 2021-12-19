using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonFunctions : MonoBehaviour
{

    public Animator animator;
    public GameObject pauseCanvas;

    bool isPaused = false;


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
    public void Play()
    {
        animator.SetTrigger("FadeOut");
    }

    void OnFadeComplete()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // Using buildIndex is faster than comparing names
        PauseGame(false);
    }

    public void PauseGame(bool pause)
    {
        isPaused = pause;
        if (pauseCanvas) pauseCanvas.SetActive(isPaused);

        if (isPaused) { Time.timeScale = 0; }
        else { Time.timeScale = 1; }

        GameObject.Find("Main Camera").GetComponent<MouseLook>().ActivateCursor(isPaused);
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1); // Only applicable because we only have 1 game scene
        PauseGame(false);
        GameObject.Find("Main Camera").GetComponent<MouseLook>().ActivateCursor(true);
    }
}
