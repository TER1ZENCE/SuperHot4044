using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    PlayerControls playerControl;
    PlayerLocomotion playerLocomotion;
    AnimatorManager animatorManager;

    public Vector2 movementInput;
    public float moveAmount;
    public float verticalInput;
    public float horizontalInput;

    public bool shift_Input;

    public void Awake()
    {
        animatorManager = GetComponent<AnimatorManager>();
        playerLocomotion = GetComponent<PlayerLocomotion>();
    }

    private void OnEnable()
    {
        if (playerControl == null)
        {
            playerControl = new PlayerControls();

            playerControl.PlayerMovement.Movement.performed += i => movementInput = i.ReadValue<Vector2>();

            playerControl.PlayerActions.Shift.performed += i => shift_Input = true;
            playerControl.PlayerActions.Shift.canceled += i => shift_Input = false;
        }

        playerControl.Enable();
    }

    private void OnDisabele()
    {
        playerControl.Disable();
    }

    public void HandleAllInputs()
    {
        HandleMovementInput();
        HandleSprintingInput();
        //HandleJumpinput
        //HandleActionInput
    }

    private void HandleMovementInput()
    {
        verticalInput = movementInput.y;
        horizontalInput = movementInput.x;
        moveAmount = Mathf.Clamp01(Mathf.Abs(horizontalInput) + Mathf.Abs(verticalInput));
        animatorManager.UpdateAnimatorValues(0, moveAmount, playerLocomotion.isSprinting);
    }

    private void HandleSprintingInput()
    {
        if (shift_Input && moveAmount >0.5f)
        {
            playerLocomotion.isSprinting = true;
        }
        else
        {
            playerLocomotion.isSprinting = false;
        }
    }
}