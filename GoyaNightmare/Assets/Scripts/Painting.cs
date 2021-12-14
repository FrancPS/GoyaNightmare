using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Painting : MonoBehaviour
{
    // Start is called before the first frame update

    public Material emptyMaterial;
    public Material paintingMaterial;

    void Start()
    {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        Material[] materialsArray = new Material[2];
        meshRenderer.materials.CopyTo(materialsArray, 0);
        materialsArray[1] = emptyMaterial;
        meshRenderer.materials = materialsArray;

    }

    public void ChangeMaterial()
    {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        Material[] materialsArray = new Material[2];
        meshRenderer.materials.CopyTo(materialsArray, 0);
        materialsArray[1] = paintingMaterial;
        meshRenderer.materials = materialsArray;
    }
}
