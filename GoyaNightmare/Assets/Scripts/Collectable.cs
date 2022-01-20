using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    // Public Variables
    [Header("Material")]
    public Material material;
    public Material materialOutline;
    public float visionAngle = 30;

    [Header("HUD")]
    public GameObject collectableHUD;

    [Header("Releated Painting GO")]
    public GameObject painting;

    // Private Variables
    bool playerIsCloseToMe = false;

    // Components references
    Camera gameCamera = null;
    MeshRenderer meshRenderer = null;
    ParticleSystem collectedParticles = null;
    AudioSource audioSource = null;
    Collider collectableCollider = null;

    // Functions
    void Awake()
    {
        gameCamera = Camera.main;
        meshRenderer = GetComponent<MeshRenderer>();
        collectedParticles = GetComponent<ParticleSystem>();
        audioSource = GetComponent<AudioSource>();
        collectableCollider = GetComponent<Collider>();
    }

    void Update()
    {
        if (playerIsCloseToMe)
        {
            if (IsCameraLooking())
            {
                ShowHintAndOutline();
                if (Input.GetButtonDown("Interact")) PickUpCollectable();
            }
            else HideHintAndOutline();
        }
    }

    void PickUpCollectable()
    {
        // Play collect effects
        collectedParticles.Play();
        audioSource.Play();

        // Hide this Collectable
        collectableHUD.SetActive(false);
        meshRenderer.enabled = false;
        collectableCollider.enabled = false;

        // Show up the corresponding Painting
        Painting paintingScript = painting.GetComponent<Painting>();
        if (paintingScript) paintingScript.ChangeMaterial();

        GameController.Instance.GoToNextStage();
    }

    bool IsCameraLooking()
    {
        // Compare the angle of this object Forward() and main Camera Forward() vectors.
        // If the angle is within a certain value, we can consider the player is looking at the painting.

        Vector3 cameraDirection = gameCamera.transform.forward;
        Vector3 objectDirection = Vector3.Normalize(transform.position - gameCamera.transform.position);
        float angle = Vector3.Dot(cameraDirection, objectDirection);

        if (angle > Mathf.Cos(Mathf.Deg2Rad * visionAngle)) return true;
        else return false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Player")
        {
            playerIsCloseToMe = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "Player")
        {
            HideHintAndOutline();
            playerIsCloseToMe = false;
        }
    }

    private void ShowHintAndOutline()
    {
        meshRenderer.material = materialOutline;
        collectableHUD.SetActive(true);
    }

    private void HideHintAndOutline()
    {
        meshRenderer.material = material;
        collectableHUD.SetActive(false);
    }
}




