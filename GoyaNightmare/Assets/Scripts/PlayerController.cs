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

    Vector3 movement;
    NavMeshAgent agent;
    bool sprinting;

    // Start is called before the first frame update
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        agent.updateRotation = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Get Input
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");
        sprinting = Input.GetButton("Sprint") && StaminaBar.instance.HasStamina();

        // Set input to local coordinates
        movement = transform.right * horizontalInput + transform.forward * verticalInput;

        // Apply movement to the navmesh agent
        agent.Move(movement * Time.deltaTime * (sprinting ? agent.speed * speedMultiplier : agent.speed));

        if (sprinting)
        {
            StaminaBar.instance.UseStamina(sprintStaminaCost); // Update stamina bar
            // TODO: Run audio
        }
        else
        {
            // TODO: Walk audio
        }
    }
}
