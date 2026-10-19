using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBobbingSystem : MonoBehaviour
{
    [SerializeField] PlayerController playerController;

    [Header("BobbingParameters")]
    [Range(1f, 10f)]
    [SerializeField] private float noiseSpeed = 4f;
    [Range(1f, 100f)]
    [SerializeField] private float smooth = 10f;
    [SerializeField] private float cameraReturnSmooth = 5f;
    private float timer;

    [Header("ProfilesForBobbing")]
    [SerializeField] private BobbingProfile walkProfile;
    [SerializeField] private BobbingProfile sprintProfile;
    [SerializeField] private BobbingProfile crouchProfile;

    private Vector3 startPos;
    
    private void Start()
    {
        startPos = transform.localPosition;
    }

    private void Update()
    {
        if (playerController.IsWalking()) ApplyBob(walkProfile);

        else if (playerController.IsSprinting()) ApplyBob(sprintProfile);

        else if (playerController.IsMoving() && playerController.IsCrouching()) ApplyBob(crouchProfile);

        else ReturnToInitialCamPos();
    }

    private void ApplyBob(BobbingProfile profile)
    {
        timer += Time.deltaTime; // instead of global clock Time.time use this
        Vector3 offset = CameraBobbingUtility.CalculatedBobbingResult(timer, profile, noiseSpeed);

        Vector3 tiltAngle = new Vector3(0f, 0f, offset.z);
        offset.z = 0f;

        Vector3 newPos = startPos +  offset; 
        transform.localPosition = Vector3.Lerp(transform.localPosition, newPos, smooth * Time.deltaTime);

        Quaternion newAngle = Quaternion.Euler(tiltAngle);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, newAngle, smooth * Time.deltaTime);
    }

    private void ReturnToInitialCamPos()
    {
        if (transform.localPosition == startPos) return;
        transform.localPosition = Vector3.Slerp(transform.localPosition, startPos, cameraReturnSmooth * Time.deltaTime);
    }
}
