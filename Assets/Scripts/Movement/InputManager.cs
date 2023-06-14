using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public IA_PlayerInput playerInput = null;
    public Vector2 movVect = Vector2.zero;
    public float horizontalInput, verticalInput;

    private void Awake()
    {
        playerInput = new IA_PlayerInput();
    }

    private void OnEnable()
    {
        playerInput.Enable();

        playerInput.PlayerMovement.Movement.performed += i => movVect = i.ReadValue<Vector2>();
    }

    private void OnDisable()
    {
        playerInput.Disable();
    }

    private void Update()
    {
        AllInput();
    }

    public void AllInput()
    {
        MovementInput();
    }

    public void MovementInput()
    {
        horizontalInput = movVect.x;
        verticalInput = movVect.y;
    }
}
