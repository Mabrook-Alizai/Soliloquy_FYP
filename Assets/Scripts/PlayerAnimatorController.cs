using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorController : MonoBehaviour
{

    [Header("AnimatorParameters")]
    private const string IS_WALKING = "IsWalking";
    private const string IS_SPRINTING = "IsSprinting";
    private const string IS_CROUCHING = "IsCrouching";
    private const string IS_JUMPING = "IsJumping";
    private const string INPUT_X = "InputX";
    private const string INPUT_Y = "InputY";

    [SerializeField] PlayerController playerController;
    private PlayerInputActions playerInputActions;

    private Animator animator;

    [SerializeField] private float inputSmooth = 10f;
    private Vector2 currentMovement;

    private void Start()
    {
        animator = GetComponent<Animator>();
        playerInputActions = new PlayerInputActions();
        playerInputActions.Enable();
    }

    private void Update()
    {
        SetAnimation();

    }

    private void SetAnimation()
    {
        Vector2 targetInputMovement = playerInputActions.Player.Move.ReadValue<Vector2>();
        currentMovement = Vector2.MoveTowards(currentMovement, targetInputMovement, inputSmooth * Time.deltaTime);

        // change all other stances
        animator.SetBool(IS_WALKING, playerController.IsWalking());
        animator.SetBool(IS_SPRINTING, playerController.IsSprinting());
        animator.SetBool(IS_CROUCHING, playerController.IsCrouching());
        animator.SetBool(IS_JUMPING, playerController.IsJumping());

        animator.SetFloat(INPUT_X, currentMovement.x);
        animator.SetFloat(INPUT_Y, currentMovement.y);
        

        

    }
}
