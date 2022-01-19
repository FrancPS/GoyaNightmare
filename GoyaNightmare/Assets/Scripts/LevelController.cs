using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LevelController : MonoBehaviour
{
    [Header("Level Properties")]
    public static float fadeInTimer = 5;
    public static float fadeOutTimer = 5;

    [Header("Win & Lose Canvas")]
    public GameObject victoryCanvas;
    public GameObject deathCanvas;

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
    MouseLook mouseLook;

    // Functions
    private void Awake()
    {
        objectsCollected = 0;
        currentLevel = 1;
        canFinish = false;

        // Initialise Camera references
        GameObject cameraGO = GameObject.FindWithTag("MainCamera");
        gameCamera = cameraGO.GetComponent<Camera>();
        cameraMaterial = cameraGO.GetComponent<PostProcessEffect>().material;
        mouseLook = gameCamera.GetComponent<MouseLook>();
    }

    void Start()
    {
        cameraMaterial.SetFloat("_DarknessFactor", 0);

        ModifyLevelLayout(currentLevel);

        victoryCanvas.SetActive(false);
        deathCanvas.SetActive(false);
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


    // Actual Level Controller
    public void FinishLevel()
    {
        mouseLook.AllowCameraRotation(false);
        mouseLook.ActivateCursor(true);
        AudioController.ChangeLevelMusic(5);
        victoryCanvas.SetActive(true);
    }

    public void Death()
    {
        mouseLook.AllowCameraRotation(false);
        mouseLook.ActivateCursor(true);
        AudioController.ChangeLevelMusic(6);
        deathCanvas.SetActive(true);
    }
}