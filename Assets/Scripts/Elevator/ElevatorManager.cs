using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator elevatorDoorAnim;

    [Header("Elevator Rumble Timing")]
    [SerializeField] private float waitTime = 6f;
    [SerializeField] private float rumbleDuration = 5f;

    [Header("Perlin Rumble Parameters")]
    [SerializeField] private float rumbleIntensity = 0.5f;
    [SerializeField] private float rumbleSpeed = 30f;

    private Vector3 originalPosition;
    private bool isRumbling;
    private Coroutine elevatorMovementCoroutine;

    private void Start()
    {
        originalPosition = transform.position;
        isRumbling = false;
    }

    private void OnEnable()
    {
        ElevatorTrigger.OnTerminalInteracted += ElevatorTrigger_OnTerminalInteracted;
        ElevatorEntranceTrigger.OnPlayerEnteredElevator += ElevatorEntranceTrigger_OnPlayerEnteredElevator;
    }

    private void OnDisable()
    {
        ElevatorTrigger.OnTerminalInteracted -= ElevatorTrigger_OnTerminalInteracted;
        ElevatorEntranceTrigger.OnPlayerEnteredElevator -= ElevatorEntranceTrigger_OnPlayerEnteredElevator;
    }

    private void Update()
    {
        if (isRumbling)
        {
            float x = (Mathf.PerlinNoise(Time.time * rumbleSpeed, 0f) - 0.5f) * rumbleIntensity;
            float y = (Mathf.PerlinNoise(0f, Time.time * rumbleSpeed) - 0.5f) * rumbleIntensity;
            float z = (Mathf.PerlinNoise(Time.time * rumbleSpeed, Time.time * rumbleSpeed) - 0.5f) * rumbleIntensity;

            transform.position = originalPosition + new Vector3(x, y, z);
        }
    }

    private void ElevatorEntranceTrigger_OnPlayerEnteredElevator()
    {
        // start the coroutine for elevator
        if(elevatorMovementCoroutine != null)
        {
            return;
        }
        elevatorMovementCoroutine = StartCoroutine(ElevatorMovementRoutine());
    }

    private void ElevatorTrigger_OnTerminalInteracted()
    {
        elevatorDoorAnim.Play("ElevatorDoorEnter");
    }

    private IEnumerator ElevatorMovementRoutine()
    {
        yield return new WaitForSeconds(waitTime);

        isRumbling = true;

        yield return new WaitForSeconds(rumbleDuration);

        isRumbling = false;
        elevatorDoorAnim.Play("ElevatorDoorExit");
    }
}
