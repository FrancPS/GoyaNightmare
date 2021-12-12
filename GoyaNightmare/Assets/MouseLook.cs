using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* This script is responsible for controlling the player camera rotation based on the mouse movement.
 * Mouse horizontal movement => Rotate the player object on its Y axis.
 * Mouse vertical movement => Rotate the camera on its X axis.
 * The camera rotation is clamped to +-60º.
 */
public class MouseLook : MonoBehaviour
{
    public float mouseSensitivity = 100f;

    private Transform playerBody;
    private float xRotation = 0f;

    void Start()
    {
        playerBody = this.transform.parent;
        Cursor.lockState = CursorLockMode.Locked; // Force the cursor to be always at the center of the screen and hide it
    }

    // Update is called once per frame
    void Update()
    {
        // Get mouse input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Apply horizontal rotation on the player
        playerBody.Rotate(Vector3.up * mouseX);

        // Apply vertical rotation clamped on the camera
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -60f, 60f);
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
}
