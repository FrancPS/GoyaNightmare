using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
	// Transform of the camera to shake. Grabs the gameObject's transform
	// if null.
	public Transform camTransform;

	// How long the object should shake for.
	public float shakeDuration = 10f;

	// Amplitude of the shake. A larger value shakes the camera harder.
	public float shakeAmount = 0.1f;
	public float decreaseFactor = 1.0f;

	void Awake()
	{
		if (camTransform == null)
		{
			camTransform = GetComponent(typeof(Transform)) as Transform;
		}
	}

	public void ShakeCamera()
	{
		StartCoroutine(ShakeCoroutine(shakeDuration));
	}

	IEnumerator ShakeCoroutine(float shakeDuration)
	{
		Vector3 originalPos = camTransform.localPosition;
		float currentShake = shakeDuration;

		while (currentShake > 0)
		{
			camTransform.localPosition = originalPos + Random.insideUnitSphere * shakeAmount;

			currentShake -= Time.deltaTime * decreaseFactor;

			yield return null;
		}

		camTransform.localPosition = originalPos;
	}
}