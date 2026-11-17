using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashLightGrabbable : MonoBehaviour, IInteractable
{
 
    
    public void OnInteract()
    {
        FlashLightController controller = FindObjectOfType<FlashLightController>();
        if(controller != null)
        {
            controller.EquipFlashLight();
            Destroy(gameObject);
        }
    }
}
