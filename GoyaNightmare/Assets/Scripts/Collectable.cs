using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Collectable : MonoBehaviour
{
    // Public Variables
    [Header("General")]
    public GameObject cameraGO;

    [Header("Material")]
    public Material material;
    public Material materialOutline;
    public float visionAngle = 30;

    [Header("HUD")]
    public GameObject collectableHUD;

    [Header("Releated Paiting GO")]
    public GameObject paiting;

    // Private Variables
    uint id = 0;
    string name = "";
    bool isCollected = false;
    bool isCollectable = false;


    MeshRenderer meshRenderer = null;
    Camera camera = null;
    ParticleSystem particleSystem = null;
    AudioSource audioSource = null;

    // Constructor
    Collectable( uint _id, string _name)
    {
        id = _id;
        name = _name;
    }

    // Getters
    public uint GetId()
    {
        return id;
    }
    public string GetName()
    {
        return name;
    }
    public bool GetIsCollectable()
    {
        return isCollectable;
    }

    // Setters
    public void SetId(uint _id)
    {
        id = _id;
    }
    public void SetName(string _name)
    {
        name = _name;
    }
    public void SetIsCollectable(bool _isCollectable)
    {
        isCollectable = _isCollectable;
    }

    // Functions
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        camera = cameraGO.GetComponent<Camera>();
        particleSystem = GetComponent<ParticleSystem>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (Input.GetButtonDown("Interact") && isCollectable)
        {
            SetIsCollected();
        }
    }
    void SetIsCollected()
    {
        isCollected = true;
        isCollectable = false;
        
        if (paiting) paiting.SetActive(true);
        if (particleSystem) particleSystem.Play();
        if (audioSource) audioSource.Play();
        if (collectableHUD) collectableHUD.SetActive(false);

        meshRenderer.enabled = false;
    }

    void UpdateCollectable()
    {
        if (!meshRenderer) return;

        if (IsCameraLooking())
        {
            if (materialOutline && meshRenderer.material != materialOutline)
            {
                meshRenderer.material = materialOutline;
            }

            if (collectableHUD && collectableHUD.activeInHierarchy == false)
            {
                collectableHUD.SetActive(true);
                isCollectable = true;
            }

        }
        else
        {
            if (material && meshRenderer.material != material)
            {
                meshRenderer.material = material;
            }

            if (collectableHUD && collectableHUD.activeInHierarchy == true)
            {
                collectableHUD.SetActive(false);
                isCollectable = false;
            }
        }
    }

    bool IsCameraLooking()
    {
        if (!camera) return false;

        Vector3 cameraDirection = camera.transform.forward;
        Vector3 objectDirection = Vector3.Normalize(transform.position - camera.transform.position);
        float angle = Vector3.Dot(cameraDirection, objectDirection);

        float temp = Mathf.Deg2Rad * visionAngle;

        if (angle > Mathf.Cos(Mathf.Deg2Rad * visionAngle))
        {
            return true;
        }

        return false;
    }

    void OnTriggerStay(Collider other)
    {
        if (isCollected) return;

        if (other.gameObject.name == "Player")
        {
            UpdateCollectable();
        }

    }

    void OnTriggerExit(Collider other)
    {
        if (material && meshRenderer.material != material)
        {
            meshRenderer.material = material;
        }

        if (collectableHUD && collectableHUD.activeInHierarchy == true)
        {
            collectableHUD.SetActive(false);
            isCollectable = false;
        }
    }
}




