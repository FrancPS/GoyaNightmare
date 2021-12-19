using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LevelController : MonoBehaviour
{
    // Variables
    public static LevelController levelController;
    public static uint objectsCollected;
    public static uint currentLevel;
    public static bool canFinish;
    public static bool playerDead;
    public static bool playerFinished;

    [Header("Level Properties")]
    public static float fadeInTimer = 5;
    public static float fadeOutTimer = 5;

    [Header("Canvas Properties")]
    public GameObject victoryCanvas;
    public GameObject deathCanvas;


    [Header("Obstacles Prefab")]
    public GameObject obstaclesParent;
    public NavMeshSurface[] surfaces;

    static Camera gameCamera;
    Material cameraMaterial = null;
    float currentFadeInDuration = 5;

    public static GameObject canvasVictory { get; private set; }
    public static GameObject canvasDeath { get; private set; }
    public static float fadeInDuration { get; private set; }
    public static float fadeOutDuration { get; private set; }

    // Functions
    private void Awake()
    {
        levelController = this;

        objectsCollected = 0;
        currentLevel = 1;
        canFinish = false;
        playerFinished = false;
        playerDead = false;

        // Setting Static variables
        canvasDeath = deathCanvas;
        canvasVictory = victoryCanvas;
        fadeInDuration = fadeInTimer;
        fadeOutDuration = fadeOutTimer;
    }

    void Start()
    {
        GameObject cameraGO = GameObject.Find("Main Camera");
        gameCamera = cameraGO.GetComponent<Camera>();
        cameraMaterial = cameraGO.GetComponent<PostProcessEffect>().material;
        cameraMaterial.SetFloat("_DarknessFactor", 1);

        currentFadeInDuration = fadeInDuration;

        levelController.ModifyLevel(currentLevel);

        victoryCanvas.SetActive(false);
        deathCanvas.SetActive(false);
        canvasVictory = victoryCanvas;
        canvasDeath = deathCanvas;

    }

    void Update()
    {
        if (currentFadeInDuration > 0)
        {
            cameraMaterial.SetFloat("_DarknessFactor", currentFadeInDuration / fadeInDuration);
            currentFadeInDuration -= Time.deltaTime;
        }
    }

    static public void ChangeLevel()
    {
        objectsCollected++;
        if (objectsCollected % 2 == 0)
        {
            currentLevel++;
            levelController.ModifyLevel(currentLevel);
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

    void ModifyLevel(uint currentLevel)
    {
        int children = levelController.obstaclesParent.transform.childCount;
        for (int i = 0; i < children; ++i)
        {
            GameObject obstacle = levelController.obstaclesParent.transform.GetChild(i).gameObject;
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
        levelController.ModifyNavMesh();
    }

    void ModifyNavMesh()
    {
        for (int i = 0; i < surfaces.Length; i++)
        {
            surfaces[i].BuildNavMesh();
        }
    }

    public static void FinishLevel()
    {
        gameCamera.GetComponent<MouseLook>().ActivateCursor(true);
        playerFinished = true;
        AudioController.ChangeLevelMusic(5);
        canvasVictory.SetActive(true);
    }

    public static void Death()
    {
        gameCamera.GetComponent<MouseLook>().ActivateMouseLook(false);
        gameCamera.GetComponent<MouseLook>().ActivateCursor(true);
        playerDead = true;
        AudioController.ChangeLevelMusic(6);
        canvasDeath.SetActive(true);
    }
}