using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashLightController : MonoBehaviour
{
    private const string FLASH_HOLD_LAYER = "FlashHold";

    [Header("ReferencesNeeded")]
    [SerializeField] private Animator animator;
    [SerializeField] private Transform camera;

    [Header("FlashLight Objects")]
    [SerializeField] private GameObject flashLightParent;
    [SerializeField] private GameObject flashON;
    [SerializeField] private GameObject flashOFF;
    [SerializeField] private GameObject flashPrefab;
    

    [Header("Input")]
    private PlayerInputActions playerInputActions;

    [Header("Animator Layer Shift parameters")]
    private float blendDuration = 0.25f;
    private Coroutine animatorLayerBlend;

    [Header("Left Hand Tilt Parameters")]
    [SerializeField] private Transform leftHandBone;
    private float trackingMultiplier = 0.8f;

    private bool isHolding = false;
    private bool isLightOn = false;
    private int flashLayerIndex;


    private void Start()
    {
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();

        flashLayerIndex = animator.GetLayerIndex(FLASH_HOLD_LAYER);
        flashLightParent.SetActive(false);
    }

    private void Update()
    {
        if (!isHolding) return;

        // toggle ON and OFF logic
        if (playerInputActions.Player.FlashOnOff.WasPressedThisFrame())
        {
            isLightOn = !isLightOn;
            UpdateFlashLightState();
        }

        if(playerInputActions.Player.Drop.WasPerformedThisFrame())
        {
            DropFlashLight();
        }
    }

    private void LateUpdate()
    {
        if(isHolding && isLightOn)
        {
            float pitch = camera.localEulerAngles.x;
            if(pitch > 180f)
            {
                pitch -= 360f;
            }
            float subtlePitch = pitch * trackingMultiplier;
            leftHandBone.Rotate(camera.right, subtlePitch, Space.World);
        }
    }

    public void EquipFlashLight()
    {
        isHolding = true;
        isLightOn = true;

        flashLightParent.SetActive(true);
        UpdateFlashLightState();
    }

    private void DropFlashLight()
    {
        isHolding = false;
        isLightOn = false;
        flashLightParent.SetActive(false);

        animator.SetLayerWeight(flashLayerIndex, 0f);

        Vector3 dropPosition = camera.position + (camera.forward * 1f);
        Instantiate(flashPrefab, dropPosition, camera.rotation);
    }

    private void UpdateFlashLightState()
    {
        flashOFF.SetActive(!isLightOn); // if light is OFF then this is false
        flashON.SetActive(isLightOn); // if light is ON then this would be true

        flashLightParent.SetActive(isLightOn);

        float targetWeight = isLightOn? 1f : 0f;

        if(animatorLayerBlend != null)
        {
            StopCoroutine(animatorLayerBlend);
        }

        animatorLayerBlend = StartCoroutine(LayerBlendRoutine(targetWeight));
    }

    private IEnumerator LayerBlendRoutine(float targetWeight)
    {
        float currentWeight = animator.GetLayerWeight(flashLayerIndex);
        float elapsed_time = 0f;

        while(elapsed_time < blendDuration)
        {
            elapsed_time += Time.deltaTime;

            float weightPercentage = elapsed_time / blendDuration;

            float newWeight = Mathf.Lerp(currentWeight, targetWeight, weightPercentage);

            animator.SetLayerWeight(flashLayerIndex, newWeight);

            yield return null;
        }

        animator.SetLayerWeight(flashLayerIndex, targetWeight);
        animatorLayerBlend = null;
    }
}
