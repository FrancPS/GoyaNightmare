using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/*
 * This script is responsible for controlling the player and all its actions
 * Movement => Horizontal and vertical axis control the player movement, that is set as a NavMesh agent.
 */

public class PlayerController : MonoBehaviour
{
    static bool isInputLocked;


    public float speedMultiplier;
    public float sprintStaminaCost;
    public bool inSafeZone = false;
    public GameObject lanternTutorialText;
    public GameObject sprintTutorialText;

    [Header("Audios")]
    public AudioSource stepsAudio;
    public AudioSource stepsAudioRun;
    public GameObject breathAudios;
    public AudioSource facingSaturnoAudio;
    public float facingSaturnoAudioTimer = 20f;
    public AudioSource distanceSaturnoAudio;


    [Header("Death parameters")]
    public GameObject saturno;
    public float deathTimer = 10f;
    public float minDistanceContactDanger = 30;
    public float minDistanceAreaDanger = 20;
    public float areaDangerTimerFrac = 0.3f;
    public float recoverOnSafeZone = 4f;
    public float recoverOutsideSazeZone = 2f;
    public float victoryRotationTimer;

    [Header("Distortion parameters")]
    public float darknessExpFactor = 3f;

    [Header("Read-only")]
    public float currentDeathTimer = 0f;
    public float playerToSaturnoDistance = 0f;
    public bool seenByPlayer = false;
    public float remaningFacingSaturnoAudioTimer = 0f;


    Vector3 movement;
    NavMeshAgent agent;
    bool firstTimeExit;
    bool moving;
    bool sprinting;
    bool victorySequence;
    IEnumerator breathingCoroutine;

    float stepsBaseVolume;
    float stepsBasePitch;
    AudioSource[] breathList;
    float sprintTutorialTimer = 0f;

    // Camera parameters
    GameObject mainCamera;
    Material cameraMaterial;
    float currentVictoryRotationTime;
    Vector3 victoryDirection;

    // Death parameters
    SaturnoAI saturnoAI;

    // Start is called before the first frame update
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        stepsBaseVolume = stepsAudio.volume;
        stepsBasePitch = stepsAudio.pitch;
        breathList = breathAudios.GetComponents<AudioSource>();

        mainCamera = GameObject.FindWithTag("MainCamera");
        cameraMaterial = mainCamera.GetComponent<PostProcessEffect>().material;
        cameraMaterial.SetFloat("_DistortionFactor", 0);

        saturnoAI = saturno.GetComponent<SaturnoAI>();
    }

    void Start()
    {
        LockInputs(true);
        inSafeZone = true;
        firstTimeExit = true;
        agent.updateRotation = false;
        breathingCoroutine = Breathing();
        StartCoroutine(breathingCoroutine);
    }

    // Update is called once per frame
    void Update()
    {
        if (GameController.Instance.HasGameFinished()) return;

        if (victorySequence)
        {
            // Recovery
            distanceSaturnoAudio.volume = 0.0f;
            if (currentDeathTimer > 0) {
                currentDeathTimer -= Time.deltaTime * recoverOnSafeZone;
            } else
            {
                currentDeathTimer = 0;
            }
            float deathPercentage = currentDeathTimer / deathTimer;
            cameraMaterial.SetFloat("_DarknessFactor", Mathf.Pow(deathPercentage, darknessExpFactor));
            cameraMaterial.SetFloat("_DistortionFactor", deathPercentage);

            MouseLook.AllowCameraRotation(false);
            currentVictoryRotationTime += Time.deltaTime;
            Vector3 lookAtDirection = Vector3.Slerp(victoryDirection, new Vector3(0, 0, 1), currentVictoryRotationTime / victoryRotationTimer);
            mainCamera.transform.LookAt(mainCamera.transform.position + lookAtDirection);

            if (currentVictoryRotationTime >= victoryRotationTimer + 1)
            {
                GameController.Instance.FinishLevel();
                victorySequence = false;
            }
            return;
        }

        if (!isInputLocked)
        {
            // Get Input
            float horizontalInput = Input.GetAxisRaw("Horizontal");
            float verticalInput = Input.GetAxisRaw("Vertical");

            moving = horizontalInput != 0 || verticalInput != 0;
            sprinting = Input.GetButton("Sprint") && StaminaBar.Instance.HasStamina();

            // SprintTutorial
            if (!firstTimeExit)
            {
                if (sprinting || sprintTutorialTimer >= 10.0f)
                {
                    sprintTutorialText.SetActive(false);
                }
                else
                {
                    sprintTutorialTimer += Time.deltaTime;
                }
            }

            // Movement
            if (moving)
            {
                // Set input to local coordinates
                movement = transform.right * horizontalInput + transform.forward * verticalInput;

                // Apply movement to the navmesh agent
                agent.Move(movement * Time.deltaTime * (sprinting ? agent.speed * speedMultiplier : agent.speed));

                if (sprinting)
                {
                    StaminaBar.Instance.UseStamina(sprintStaminaCost * Time.deltaTime); // Update stamina bar
                    if (!stepsAudioRun.isPlaying && !stepsAudio.isPlaying) PlayStepAudio(stepsAudioRun); // Steps audio running
                }
                else
                {
                    if (!stepsAudioRun.isPlaying && !stepsAudio.isPlaying) PlayStepAudio(stepsAudio);  // Steps audio walking
                }
            }
        }

        if (UpdateDeathCondition())
        {
            StopCoroutine(breathingCoroutine);
            distanceSaturnoAudio.volume = 0.0f;
            GameController.Instance.Death();
        }
    }

    void PlayStepAudio(AudioSource audio)
    {
        audio.volume = stepsBaseVolume + Random.Range(-0.2f, 0.2f);
        audio.pitch = stepsBasePitch + Random.Range(-0.2f, 0.2f);
        audio.Play();

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.name == "SafeRoom")
        {
            inSafeZone = true;
            if (LevelController.canFinish)
            {
                agent.speed = agent.speed / 2.0f;
                Vector3 origin = new Vector3(0, 1, 0);
                agent.SetDestination(origin);
                victoryDirection = mainCamera.transform.forward;
                currentVictoryRotationTime = 0f;
                victorySequence = true;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.name == "SafeRoom")
        {
            inSafeZone = false;

            // Toggle Lantern and Sprint tutorials
            if (firstTimeExit)
            {
                lanternTutorialText.SetActive(false);
                sprintTutorialText.SetActive(true);
                firstTimeExit = false;
            }
        }
    }

    private IEnumerator Breathing()
    {
        while (true)
        {
            int index = Random.Range(0, breathList.Length - 1);
            if (sprinting)
            {
                breathList[index].volume = 0.01f;
            }
            else
            {
                breathList[index].volume = 0.002f;
            }
            breathList[index].Play();
            yield return new WaitForSeconds(breathList[index].clip.length);
        }
    }

    private bool UpdateDeathCondition()
    {
        if (!saturnoAI) return false;

        playerToSaturnoDistance = saturnoAI.GetPlayerToSaturnoDistance();
        seenByPlayer = saturnoAI.GetSeenByPlayer();

        if (inSafeZone)
        {
            if (currentDeathTimer > 0) currentDeathTimer -= Time.deltaTime * recoverOnSafeZone;
        }
        else
        {
            if (seenByPlayer && playerToSaturnoDistance < minDistanceContactDanger)
            {
                currentDeathTimer += Time.deltaTime;

                if (remaningFacingSaturnoAudioTimer <= 0)
                {
                    facingSaturnoAudio.Play();
                    remaningFacingSaturnoAudioTimer = facingSaturnoAudioTimer;
                }
            }
            else if (saturnoAI.IsVisible() && playerToSaturnoDistance < minDistanceAreaDanger)
            {
                if (currentDeathTimer <= areaDangerTimerFrac * deathTimer)
                {
                    currentDeathTimer += Time.deltaTime;
                }
            }
            else
            {
                if (currentDeathTimer > 0) currentDeathTimer -= Time.deltaTime * recoverOutsideSazeZone;
            }
        }


        if (currentDeathTimer < 0) currentDeathTimer = 0;
        float deathPercentage = currentDeathTimer / deathTimer;

        // Camera Material
        cameraMaterial.SetFloat("_DarknessFactor", Mathf.Pow(deathPercentage, darknessExpFactor));
        cameraMaterial.SetFloat("_DistortionFactor", deathPercentage);

        // Audios
        if (distanceSaturnoAudio.isPlaying)
        {
            distanceSaturnoAudio.volume = Mathf.Lerp(0, 0.5f, deathPercentage);
        }
        else
        {
            distanceSaturnoAudio.Play();
        }

        // Timers
        if (remaningFacingSaturnoAudioTimer > 0) remaningFacingSaturnoAudioTimer -= Time.deltaTime;

        if (currentDeathTimer >= deathTimer)
        {
            return true;
        }

        return false;
    }

    public static void LockInputs(bool lockThem)
    {
        isInputLocked = lockThem;
    }
}