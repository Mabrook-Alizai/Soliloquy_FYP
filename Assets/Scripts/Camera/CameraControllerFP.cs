using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControllerFP : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float mouseSensitivity = 2f;

    
    private float cameraMovement;

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        CameraMovement();
    }

    private void CameraMovement()
    {
        float inputX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float inputY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // up and down movement
        cameraMovement -= inputY;
        cameraMovement = Mathf.Clamp(cameraMovement, -80f, 60f);
        transform.localEulerAngles = Vector3.right * cameraMovement;

        // left and right movement
        player.Rotate(Vector3.up, inputX);
    }
}
