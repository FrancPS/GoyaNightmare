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

    Vector3 destination;
    NavMeshAgent agent;
    float saturnoMaxSpeed;

    // Teleport
    bool seenByPlayer;
    bool leftPlayerSight;
    float playerFarAwayTimer;

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
        if (!lantern.enabled)
        {
            agent.isStopped = true;
        }
        else
        {
            UpdateSpeed();
            TeleportSaturno();
            // Update position
            agent.isStopped = false;
            destination = player.GetComponent<NavMeshAgent>().steeringTarget;
            agent.SetDestination(destination);
        }
    }

    void UpdateSpeed()
    {
        // Is player seeing Saturno?
        // Modify speed accordingly
        RaycastHit hit;
        Vector3 playerToSaturno = transform.position - player.transform.position;
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

    bool TeleportSaturno()
    {
        Vector3 playerToSaturno = transform.position - player.transform.position;
        if (playerToSaturno.magnitude > teleportTriggeringDistance)
        {
            // Player escapes from Saturno
            if (leftPlayerSight)
            {
                int index = (int)Random.Range(0.0f, spawnPoints.Length);
                agent.Warp(spawnPoints[index].position);
                leftPlayerSight = false;

                return true;
            }

            playerFarAwayTimer += Time.deltaTime;
            if (playerFarAwayTimer >= teleportTriggeringTime)
            {
                playerFarAwayTimer = 0;
                int index = (int)Random.Range(0.0f, spawnPoints.Length);
                agent.Warp(spawnPoints[index].position);
                leftPlayerSight = false;
                return true;
            }
        }
        return false;

    }

}
