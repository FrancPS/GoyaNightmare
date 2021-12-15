using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LevelController : MonoBehaviour
{
    // Variables
    public static LevelController levelController;
    public static uint objectsCollected = 0;
    public static uint currentLevel = 1;
    public static bool canFinish = false;

    [Header("Obstacles Prefab")]
    public GameObject obstaclesParent;
    public NavMeshSurface[] surfaces;

    [Header("Level Properties")]
    public static float fadeInDuration = 5;
    public static float fadeOutDuration = 5;

    static Camera camera;
    Material cameraMaterial = null;
    float currentFadeInDuration = 5;

    // Functions
    private void Awake()
    {
        levelController = this;
    }

    void Start()
    {
        GameObject cameraGO = GameObject.Find("Main Camera");
        camera = cameraGO.GetComponent<Camera>();
        cameraMaterial = cameraGO.GetComponent<PostProcessEffect>().material;
        cameraMaterial.SetFloat("_DarknessFactor", 1);

        currentFadeInDuration = fadeInDuration;

        levelController.ModifyLevel(currentLevel);
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
            if (camera)
            {
                CameraShake cameraShake = camera.GetComponent<CameraShake>();
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
        AudioController.ChangeLevelMusic(5);
        Debug.Log("Game Finished!!!!!!!!");
    }

    public static void Death()
    {
        Debug.Log("You're death!!!!!!!!");
    }
}

