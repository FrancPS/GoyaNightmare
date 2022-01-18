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
    public float mouseSensitivity = 1f;
    private bool canRotate;

    // Quick references
    private Transform playerBody;
    private float xRotation = 0f;

    void Start()
    {
        playerBody = this.transform.parent;
        AllowCameraRotation(true);
    }

    // Update is called once per frame
    void Update()
    {
        // TODO: set can rotate after finiching the LevelController.fadeIn
        if (canRotate)
        {
            // Get mouse input
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

            // Apply horizontal rotation on the player
            playerBody.Rotate(Vector3.up * mouseX);

            // Apply vertical rotation on the camera (clamped to 60 degrees)
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -60f, 60f);
            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }
    }

    public void AllowCameraRotation(bool isAllowed)
    {
        canRotate = isAllowed;
    }

    public void ActivateCursor(bool isActive)
    {
        // Active: Visible normal cursor
        // not Active: Hidden cursor, forced to be at the center of the screen.
        Cursor.visible = isActive;
        Cursor.lockState = isActive ? CursorLockMode.None : CursorLockMode.Locked;
    }
}
