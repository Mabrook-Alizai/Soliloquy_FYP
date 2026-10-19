using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] private Transform camera;
    [SerializeField] private LayerMask flashGrabbableMask;
    private PlayerInputActions playerInputActions;

    private void Start()
    {
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
    }

    private void Update()
    {
        float interactDistance = 2f;
        float interactRadius = 0.1f;
        if (playerInputActions.Player.Interact.triggered)
        {
            if (Physics.SphereCast(camera.position, interactRadius, camera.forward, out RaycastHit hitInfo, interactDistance, flashGrabbableMask))
            {
                if (hitInfo.collider.TryGetComponent(out FlashLightGrabbable flashLightGrabbable))
                {
                    flashLightGrabbable.OnInteract();
                }
            }
        }
    }
}
