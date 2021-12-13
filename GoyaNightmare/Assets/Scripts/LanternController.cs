using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 
 * This script is responsible for controlling the lantern.
 * Activate and deactivate light using F key and set the "lanternOn" global variable. <- TODO
 * Flickering.
 * TODO: Sprinting offset.
 */

public class LanternController : MonoBehaviour
{

    public float maxReduction;
    public float maxIncrease;
    public float rateDamping;
    public float strength;

    Light lantern;
    float storedIntensity;

    void Awake()
    {
        lantern = gameObject.GetComponent<Light>();
    }

    private void Start()
    {
        lantern.enabled = true;
        storedIntensity = lantern.intensity;

        StartCoroutine(DoFlicker());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Lantern"))
        {
            lantern.enabled = !lantern.enabled;
        }
    }

    private IEnumerator DoFlicker()
    {
        while (true)
        {
            lantern.intensity = Mathf.Lerp(lantern.intensity, Random.Range(storedIntensity - maxReduction, storedIntensity + maxIncrease), strength * Time.deltaTime);
            yield return new WaitForSeconds(rateDamping);
        }
    }
}
