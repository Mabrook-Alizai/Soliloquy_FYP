using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System;


public class PlayerController : MonoBehaviour
{
    [Header("Input Actions And Character Controller")]
    private PlayerInputActions playerInputActions;
    private Vector2 movement2D;
    private CharacterController controller;

    [Header("Player Movement States")]
    private PlayerStates playerState;
    private enum PlayerStates
    {
        walking,
        sprinting,
        crouching,
        air,
    }

    [Header("Boolean Parameters")]
    
    

    [Header("Movement Parameters")]
    [SerializeField] private float walkSpeed = 3.5f;
    [SerializeField] private float sprintSpeed = 5f;

    [Header("Jumping Parameters")]
    [SerializeField] private float gravity = -9.8f;
    [SerializeField] private float jumpheight = 1.2f;
    private float verticalVelocity = 0f;

    [Header("Crouching Parameters")]
    [SerializeField] private float crouchSpeed = 2.5f;
    [SerializeField] private float crouchHeight = 1f;
    private float normalHeight =2f;


    [Header("Stamina Parameters")]
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float staminaUseMultiplier = 5f;
    [SerializeField] private float timeBeforeStaminaRegenerates = 3f;
    [SerializeField] private float staminaIncrease = 2f;
    [SerializeField] private float staminaTimeIncrement = 0.1f;
    private float currentStamina;
    private Coroutine regeneratingStamina;
    public static Action<float> OnStaminaChange;
    

    private void Awake()
    {
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
        currentStamina = maxStamina;
    }

    private void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        GroundMovement();
        StaminaHandler();
        
    }

    private void GroundMovement()
    {
        // Get Player movement Vector2
        movement2D = playerInputActions.Player.Move.ReadValue<Vector2>();
        movement2D = movement2D.normalized;
        

        Vector3 moveDir = transform.right * movement2D.x + transform.forward * movement2D.y;

        Jumphandler();
        CrouchHandler();

        // check if sprinting
        float currentSpeed = PlayerStateAndSprintHandler();

        Vector3 finalMovement = moveDir * currentSpeed;
        finalMovement.y = verticalVelocity;

        controller.Move(finalMovement * Time.deltaTime);
    }

    private void Jumphandler()
    {
        if (controller.isGrounded && verticalVelocity <= 0)
        {
            verticalVelocity = -2f;
        }

        if (playerInputActions.Player.Jump.triggered && controller.isGrounded)
        {
            verticalVelocity = Mathf.Sqrt(-2f * gravity * jumpheight); // first law of motion
        }

        verticalVelocity += gravity * Time.deltaTime; // v = gt
    }

    private float PlayerStateAndSprintHandler()
    {
        float currentSpeed;

        // check first if player is grounded and crouching
        if (playerInputActions.Player.Crouch.IsPressed() && (controller.isGrounded || playerState != PlayerStates.sprinting) && !playerInputActions.Player.Sprint.IsPressed())
        {
            playerState = PlayerStates.crouching;
            currentSpeed = crouchSpeed;
        }

        // is grounded and is sprinting
        else if (controller.isGrounded && playerInputActions.Player.Sprint.IsPressed() && currentStamina > 0 && playerState != PlayerStates.crouching)
        {
            if(IsMoving() && movement2D.y >= 0)
            {
                playerState = PlayerStates.sprinting;
                currentSpeed = sprintSpeed;
            }
            else
            {
                playerState = PlayerStates.walking;
                currentSpeed = walkSpeed;
            }

        }

        // is grounded but not running nor sprinting
        else if (controller.isGrounded)
        {
            playerState = PlayerStates.walking;
            currentSpeed = walkSpeed;
        }

        // neither case, player is in air
        else
        {
            playerState = PlayerStates.air;
            currentSpeed = walkSpeed;
        }

        return currentSpeed;
    }

    private void CrouchHandler()
    {
        if (playerState == PlayerStates.crouching)
        {
            controller.height = crouchHeight;
        }
        else
        {
            controller.height = normalHeight;
        }
    }

    private void StaminaHandler()
    {
        if(playerState == PlayerStates.sprinting && IsMoving())
        {
            if(regeneratingStamina != null)
            {
                StopCoroutine(regeneratingStamina);
                regeneratingStamina  = null;
            }

            currentStamina -= staminaUseMultiplier * Time.deltaTime;

            if (currentStamina < 0) currentStamina = 0;

            OnStaminaChange?.Invoke(currentStamina);

            if (currentStamina <= 0) playerState = PlayerStates.walking;
        }
        if(playerState != PlayerStates.sprinting && currentStamina < maxStamina && regeneratingStamina == null) // only start this if player is not sprinting, there is stamina to refill and currently there is no stamina generating
        {
            regeneratingStamina = StartCoroutine(RegenerateStamina());
        }

    }

    public bool IsWalking()
    {
        if (IsMoving() && !IsSprinting() && !IsCrouching()) return true;
        else return false;
    }

    public bool IsSprinting()
    {
        if (IsMoving() && playerInputActions.Player.Sprint.IsPressed() && currentStamina > 0 && movement2D.y >= 0) return true; // if moving and pressing shift
        else return false;
    }

    public bool IsCrouching()
    {
        if (playerInputActions.Player.Crouch.IsPressed()) return true;
        else return false;
    }

    public bool IsJumping()
    {
        if (playerInputActions.Player.Jump.triggered ) return true;
        else return false;
    }

    public bool IsMoving()
    {
        if (movement2D != Vector2.zero) return true;
        else return false;
    }

    


    private IEnumerator RegenerateStamina()
    {
        yield return new WaitForSeconds(timeBeforeStaminaRegenerates);
        WaitForSeconds timeToWait = new WaitForSeconds(staminaTimeIncrement);

        while(currentStamina < maxStamina)
        {
            //if (currentStamina > 0) playerState = PlayerStates.sprinting;
            currentStamina += staminaIncrease;

            if (currentStamina > maxStamina) currentStamina = maxStamina;

            OnStaminaChange?.Invoke(currentStamina);

            yield return timeToWait;
        }
        regeneratingStamina = null;
    }
}
