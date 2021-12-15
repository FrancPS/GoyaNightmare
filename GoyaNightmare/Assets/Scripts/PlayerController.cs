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
    public float speedMultiplier;
    public float sprintStaminaCost;
    public bool inSafeZone = false;

    [Header("Audios")]
    public AudioSource stepsAudio;
    public AudioSource stepsAudioRun;
    public GameObject breathAudios;
    public AudioSource facingSaturnoAudio;
    public AudioSource distanceSaturnoAudio;


    [Header("Dead parameters")]
    public GameObject saturno;
    public float minDistanceDanger = 10;
    public float recoverOnSafeZone = 4f;
    public float recoverOutsideSazeZone = 2f;


    Vector3 movement;
    NavMeshAgent agent;
    bool moving;
    bool sprinting;

    float stepsBaseVolume;
    float stepsBasePitch;
    AudioSource[] breathList;
    float fadeInDuration;

    // Camera parameters
    Material cameraMaterial;

    // Death parameters
    SaturnoAI saturnoAI;
    float deathTimer = 20f;
    float currentDeathTimer = 0f;

    float facingSaturnoAudioTimer = 20f;

    // Start is called before the first frame update
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        stepsBaseVolume = stepsAudio.volume;
        stepsBasePitch = stepsAudio.pitch;
        breathList = breathAudios.GetComponents<AudioSource>();

        cameraMaterial = GameObject.Find("Main Camera").GetComponent<PostProcessEffect>().material;
        cameraMaterial.SetFloat("_DistortionFactor", 0);

        saturnoAI = saturno.GetComponent<SaturnoAI>();
        inSafeZone = true;
    }

    void Start()
    {
        agent.updateRotation = false;
        StartCoroutine(Breathing());

        fadeInDuration = LevelController.fadeInDuration;
    }

    // Update is called once per frame
    void Update()
    {
        // Block movement at Start
        if (fadeInDuration > 0)
        {
            fadeInDuration -= Time.deltaTime;
            return;
        }

        // Get Input
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        moving = horizontalInput != 0 || verticalInput != 0;
        sprinting = Input.GetButton("Sprint") && StaminaBar.instance.HasStamina();

        if (moving)
        {
            // Set input to local coordinates
            movement = transform.right * horizontalInput + transform.forward * verticalInput;

            // Apply movement to the navmesh agent
            agent.Move(movement * Time.deltaTime * (sprinting ? agent.speed * speedMultiplier : agent.speed));

            if (sprinting)
            {
                StaminaBar.instance.UseStamina(sprintStaminaCost); // Update stamina bar
                if (!stepsAudioRun.isPlaying && !stepsAudio.isPlaying) PlayStepAudio(stepsAudioRun); // Steps audio running
            }
            else
            {
                if (!stepsAudioRun.isPlaying && !stepsAudio.isPlaying) PlayStepAudio(stepsAudio);  // Steps audio walking
            }
        }

        if (UpdateDeathCondition())
        {
            LevelController.Death();
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
            if (LevelController.canFinish) LevelController.FinishLevel();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.name == "SafeRoom")
        {
            inSafeZone = false;
        }
    }

    private IEnumerator Breathing()
    {
        yield return new WaitForSeconds(1f);
        while (true)
        {
            int index = Random.Range(0, breathList.Length - 1);
            if (sprinting)
            {
                breathList[index].volume = 0.01f;
            } else
            {
                breathList[index].volume = 0.002f;
            }
            breathList[index].Play();
            yield return new WaitForSeconds(breathList[index].clip.length);
        }
    }

    private bool UpdateDeathCondition()
    {
        Debug.Log(currentDeathTimer);

        if (!saturnoAI) return false;

        if (inSafeZone)
        {
            if (currentDeathTimer > 0) currentDeathTimer -= Time.deltaTime * recoverOnSafeZone;
            if (distanceSaturnoAudio.isPlaying) distanceSaturnoAudio.Stop();

        }
        else
        {

            float playerToSaturnoDistance = saturnoAI.GetPlayerToSaturnoDistance();
            bool seenByPlayer = saturnoAI.GetSeenByPlayer();

            if (seenByPlayer && playerToSaturnoDistance < minDistanceDanger)
            {
                currentDeathTimer += Time.deltaTime;

                if (facingSaturnoAudioTimer <= 0)
                {
                    facingSaturnoAudio.Play();
                    facingSaturnoAudioTimer = 15f;
                }



            }
            else if (playerToSaturnoDistance < minDistanceDanger)
            {
                if (currentDeathTimer <= 3)
                {
                    currentDeathTimer += Time.deltaTime;
                }

                if (!distanceSaturnoAudio.isPlaying) distanceSaturnoAudio.Play();

            }
            else
            {
                if (currentDeathTimer > 0) currentDeathTimer -= Time.deltaTime * recoverOutsideSazeZone;
                if (distanceSaturnoAudio.isPlaying) distanceSaturnoAudio.Stop();
            }
        }

        // Camera Material
        cameraMaterial.SetFloat("_DarknessFactor", currentDeathTimer / deathTimer);
        cameraMaterial.SetFloat("_DistortionFactor", currentDeathTimer / deathTimer);

        // Timers
        if (facingSaturnoAudioTimer > 0) facingSaturnoAudioTimer -= Time.deltaTime;

        if (currentDeathTimer >= deathTimer)
        {
            return true;
        }

        return false;
    }
}
