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

    Vector3 movement;
    NavMeshAgent agent;
    bool moving;
    bool sprinting;

    float stepsBaseVolume;
    float stepsBasePitch;
    AudioSource[] breathList;

    // Start is called before the first frame update
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        stepsBaseVolume = stepsAudio.volume;
        stepsBasePitch = stepsAudio.pitch;
        breathList = breathAudios.GetComponents<AudioSource>();
    }

    void Start()
    {
        agent.updateRotation = false;
        StartCoroutine(Breathing());
    }

    // Update is called once per frame
    void Update()
    {
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

}
