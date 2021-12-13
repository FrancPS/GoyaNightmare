using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineMaterial : MonoBehaviour
{

    // Variables
    public GameObject cameraGO;
    public Material material;
    public Material materialOutline;
    public float distance = 10;
    public float visionAngle = 30;



    // Functions
    void Update()
    {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        if (!meshRenderer) return;

        if (IsCameraLooking())
        {
            if (materialOutline && meshRenderer.material != materialOutline)
            {
                meshRenderer.material = materialOutline;
            }

        }
        else
        {
            if (material && meshRenderer.material != material)
            {
                meshRenderer.material = material;
            }
        }
    }

    bool IsCameraLooking()
    {
        Camera camera = cameraGO.GetComponent<Camera>();
        if (!camera) return false;

        Vector3 cameraDirection = camera.transform.forward;
        Vector3 objectDirection = Vector3.Normalize(transform.position - camera.transform.position);
        float objectDistance = Vector3.Distance(transform.position, camera.transform.position);
        float angle = Vector3.Dot(cameraDirection, objectDirection);

        float temp = Mathf.Deg2Rad * visionAngle;

        if (angle > Mathf.Cos(Mathf.Deg2Rad * visionAngle)  && objectDistance < distance)
        {
            return true;
        }

        return false;
    }
}
