using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorEntranceTrigger : MonoBehaviour
{
    public static event Action OnPlayerEnteredElevator;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ethan"))
        {
            OnPlayerEnteredElevator?.Invoke();
            Debug.Log("Player Entered");
            gameObject.SetActive(false);
        }
    }
}
