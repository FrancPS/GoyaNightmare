using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LevelController : MonoBehaviour
{
    private const int MAX_NUM_COLLECTABLES = 5;
    public const int PAUSE_CANVAS_ID = 0;
    public const int VICTORY_CANVAS_ID = 1;
    public const int DEATH_CANVAS_ID = 2;

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
    Material cameraMaterial = null;
    CameraShake cameraShake;

    private void Awake()
    {
        objectsCollected = 0;
        currentLevel = 1;
        canFinish = false;

        // Initialise Camera references
        GameObject cameraGO = Camera.main.gameObject;
        cameraMaterial = cameraGO.GetComponent<PostProcessEffect>().material;
        cameraShake = cameraGO.GetComponent<CameraShake>();
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

    #region Stages
    public void GoToNextStage()
    {
        objectsCollected++;

        // Level layout is changed every second picture we collect.
        if (objectsCollected % 2 == 0)
        {
            currentLevel++;
            ModifyLevelLayout(currentLevel);
            AudioController.ChangeLevelMusic(currentLevel);
            cameraShake.ShakeCamera();
        }
        // TODO: If all collectables have been collected, trigger guidance to central room
        else if (objectsCollected == MAX_NUM_COLLECTABLES)
        {
            canFinish = true;
            AudioController.ChangeLevelMusic(4);
        }
    }

    void ModifyLevelLayout(uint currentLevel)
    {
        foreach (Transform obstacle in obstaclesParent.transform)
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

                obstacle.gameObject.SetActive(toActivate);
            }
        }
        UpdateNavMeshes();
    }

    void UpdateNavMeshes()
    {
        for (int i = 0; i < surfaces.Length; i++)
        {
            surfaces[i].BuildNavMesh();
        }
    }
    #endregion

    #region Canvas Open/Close routines
    public void OpenCanvas(int canvasID)
    {
        switch (canvasID)
        {
            case 0:
                pauseCanvas.gameObject.SetActive(true);
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
                pauseCanvas.gameObject.SetActive(false);
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
    #endregion
}