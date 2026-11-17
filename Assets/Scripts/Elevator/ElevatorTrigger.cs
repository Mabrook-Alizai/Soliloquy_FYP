using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorTrigger : MonoBehaviour, IInteractable
{
    public static event Action OnTerminalInteracted;

    public void OnInteract()
    {
        Debug.Log("Terminal Interacted");
        OnTerminalInteracted?.Invoke();
    }
}
