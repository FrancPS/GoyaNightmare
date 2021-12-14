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

    [Header("Obstacles Prefab")]
    public GameObject obstaclesParent;
    public NavMeshSurface[] surfaces;

    AudioSource audio = null;

    // Functions
    private void Awake()
    {
        levelController = this;
    }

    void Start()
    {
        audio = GetComponent<AudioSource>();
        levelController.ModifyLevel(currentLevel);
    }

    static public void ChangeLevel()
    {
        objectsCollected++;
        if (objectsCollected % 2 == 0)
        {
            currentLevel++;
            levelController.ModifyLevel(currentLevel);
            if (levelController.audio) levelController.audio.Play();
        
        } else if (objectsCollected == 5)
        {
            levelController.FinishLevel();
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

    void FinishLevel()
    {
        Debug.Log("Game Finished!!!!!!!!");
    }
}

