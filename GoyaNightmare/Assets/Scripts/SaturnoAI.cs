using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class SaturnoAI : MonoBehaviour
{
    public GameObject player;
    public Light lantern;
    public Transform[] spawnPoints;
    public float teleportTriggeringDistance;
    public float teleportTriggeringTime;
    public Animator animator;

    Vector3 destination;
    NavMeshAgent agent;
    float saturnoMaxSpeed;

    // Teleport
    bool seenByPlayer;
    bool leftPlayerSight;
    float playerFarAwayTimer = 0.0f;
    float playerToSaturnoDistance;

    bool turnVisible;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent) saturnoMaxSpeed = agent.speed;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 playerToSaturno = transform.position - player.transform.position;
        if (turnVisible && Vector3.Dot(playerToSaturno, player.transform.forward) < 0.0f)
        {
            GetComponent<MeshRenderer>().enabled = true;
            turnVisible = false;
        }

        if (player.GetComponent<PlayerController>().inSafeZone)
        {
            agent.isStopped = false;
            agent.SetDestination(spawnPoints[0].position);
        }
        else
        {
            UpdateSpeed();
            if (!lantern.enabled)
            {
                agent.isStopped = true;
                animator.ResetTrigger("ResumeWalk");
                animator.SetTrigger("Stop");
            }
            else
            {
                ShouldTeleportSaturno();
                // Update position
                agent.isStopped = false;
                animator.ResetTrigger("Stop");
                animator.SetTrigger("ResumeWalk");
                destination = player.GetComponent<NavMeshAgent>().steeringTarget;
                agent.SetDestination(destination);
            }
        }
    }

    void UpdateSpeed()
    {
        // Is player seeing Saturno?
        // Modify speed accordingly
        RaycastHit hit;
        Vector3 playerToSaturno = transform.position - player.transform.position;
        playerToSaturnoDistance = Vector3.Distance(transform.position, player.transform.position);

        if (Physics.Raycast(player.transform.position, playerToSaturno, out hit, Mathf.Infinity))
        {
            if (Vector3.Dot(playerToSaturno, player.transform.forward) > 0.0f && hit.collider.name == "Saturno")
            {
                agent.speed = saturnoMaxSpeed / 2.0f;
                seenByPlayer = true;
                leftPlayerSight = false;
            }
            else
            {
                agent.speed = saturnoMaxSpeed;
                if (seenByPlayer) leftPlayerSight = true;
                seenByPlayer = false;
            }
        }
    }

    bool ShouldTeleportSaturno()
    {
        Vector3 playerToSaturno = transform.position - player.transform.position;
        if (playerToSaturno.magnitude > teleportTriggeringDistance)
        {
            // Player escapes from Saturno
            if (leftPlayerSight)
            {
                Teleportation();

                return true;
            }

            playerFarAwayTimer += Time.deltaTime;

            if (playerFarAwayTimer >= teleportTriggeringTime)
            {
                playerFarAwayTimer = 0.0f;
                Teleportation();

                return true;
            }
        }
        return false;

    }

    void Teleportation()
    {
        int index;
        do
        {
            index = (int)Random.Range(0.0f, spawnPoints.Length);
        } while ((spawnPoints[index].position - player.transform.position).magnitude < (teleportTriggeringDistance / 2));

        agent.Warp(spawnPoints[index].position);
        leftPlayerSight = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.name.Contains("Obstacle"))
        {
            GetComponent<MeshRenderer>().enabled = false;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.name.Contains("Obstacle"))
        {
            turnVisible = true;
        }
    }

    // Getters
    public bool GetSeenByPlayer() {
        return seenByPlayer;
    }

    public float GetPlayerToSaturnoDistance()
    {
        return playerToSaturnoDistance;
    }

}
