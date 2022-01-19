using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LevelController : MonoBehaviour
{
    [Header("Level Properties")]
    public static float fadeInTimer = 5;
    public static float fadeOutTimer = 5;

    [Header("UI Canvas Groups")]
    public CanvasGroup victoryCanvas;
    public CanvasGroup deathCanvas;
    public CanvasGroup pauseCanvas;

    [Header("Navigation")]
    public GameObject obstaclesParent;
    public NavMeshSurface[] surfaces;

    // Game State Static Variables
    public static uint objectsCollected { get; private set; }
    public static uint currentLevel { get; private set; }
    public static bool canFinish { get; private set; }

    // Camera references
    Camera gameCamera;
    Material cameraMaterial = null;

    private void Awake()
    {
        objectsCollected = 0;
        currentLevel = 1;
        canFinish = false;

        // Initialise Camera references
        GameObject cameraGO = GameObject.FindWithTag("MainCamera");
        gameCamera = cameraGO.GetComponent<Camera>();
        cameraMaterial = cameraGO.GetComponent<PostProcessEffect>().material;
    }

    void Start()
    {
        cameraMaterial.SetFloat("_DarknessFactor", 0);

        ModifyLevelLayout(currentLevel);

        victoryCanvas.gameObject.SetActive(false);
        victoryCanvas.alpha = 0;
        deathCanvas.gameObject.SetActive(false);
        deathCanvas.alpha = 0;
        pauseCanvas.gameObject.SetActive(false);
        pauseCanvas.alpha = 1;
    }

    public void GoToNextStage()
    {
        objectsCollected++;
        if (objectsCollected % 2 == 0)
        {
            currentLevel++;
            ModifyLevelLayout(currentLevel);
            AudioController.ChangeLevelMusic(currentLevel);
            if (gameCamera)
            {
                CameraShake cameraShake = gameCamera.GetComponent<CameraShake>();
                if (cameraShake) cameraShake.ShakeCamera();
            }

        }
        else if (objectsCollected == 5)
        {
            canFinish = true;
            AudioController.ChangeLevelMusic(4);
        }
    }

    void ModifyLevelLayout(uint currentLevel)
    {
        int children = obstaclesParent.transform.childCount;
        for (int i = 0; i < children; ++i)
        {
            GameObject obstacle = obstaclesParent.transform.GetChild(i).gameObject;
            if (obstacle)
            {
                Obstacle obstacleScript = obstacle.GetComponent<Obstacle>();
                if (obstacleScript)
                {
                    bool toActivate = false;
                    foreach (uint obstacleLevel in obstacleScript.obstacleLevels)
                    {
                        if (obstacleLevel == currentLevel)
                        {
                            toActivate = true;
                        }
                    }

                    if (toActivate)
                    {
                        obstacle.SetActive(true);
                    }
                    else
                    {
                        obstacle.SetActive(false);
                    }
                }
            }
        }
        UpdateNavMesh();
    }

    void UpdateNavMesh()
    {
        for (int i = 0; i < surfaces.Length; i++)
        {
            surfaces[i].BuildNavMesh();
        }
    }

    #region Canvas Opening Coroutines
    public void OpenCanvas(int canvasID)
    {
        switch (canvasID)
        {
            case 0:
                StartCoroutine(CanvasFadeIn(pauseCanvas, 0.01f));
                break;
            case 1:
                StartCoroutine(CanvasFadeIn(victoryCanvas, 0.25f));
                break;
            case 2:
                StartCoroutine(CanvasFadeIn(deathCanvas, 0.25f));
                break;
            default:
                break;
        }
    }

    public void CloseCanvas(int canvasID)
    {
        switch (canvasID)
        {
            case 0:
                StartCoroutine(CanvasFadeOut(pauseCanvas, 0.01f));
                break;
            case 1:
                StartCoroutine(CanvasFadeOut(victoryCanvas, 0.025f));
                break;
            case 2:
                StartCoroutine(CanvasFadeOut(deathCanvas, 0.025f));
                break;
            default:
                break;
        }
    }

    private IEnumerator CanvasFadeIn(CanvasGroup canvas, float fadeSpeedMultiplier)
    {
        canvas.gameObject.SetActive(true);
        while (canvas.alpha < 1)
        {
            canvas.alpha += fadeSpeedMultiplier;
            yield return null;
        }
    }

    private IEnumerator CanvasFadeOut(CanvasGroup canvas, float fadeSpeedMultiplier)
    {
        canvas.gameObject.SetActive(true);
        while (canvas.alpha > 0)
        {
            canvas.alpha -= fadeSpeedMultiplier;
            yield return null;
        }
        canvas.gameObject.SetActive(false);
    }

    public void OnGamePaused(bool mustOpenPauseMenu)
    {
        pauseCanvas.gameObject.SetActive(mustOpenPauseMenu);
    }
    #endregion
}