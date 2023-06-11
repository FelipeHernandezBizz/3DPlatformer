using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    PlayerControl pC;
    AnimationManager animMan;
    public Vector2 movInput;
    public float verticalInput;
    public float horizontalInput;
    public bool sprinting;
    public float movAmout;

    private void Awake()
    {
        animMan = GetComponent<AnimationManager>();
    }

    private void OnEnable()
    {
        if (pC == null)
        {
            pC = new PlayerControl();
            pC.PlayerMovement.Movement.performed += i => movInput = i.ReadValue<Vector2>();
        }

        pC.Enable();
    }

    private void OnDisable()
    {
        pC.Disable();
    }

    public void AllInputs()
    {
        MovInput();
    }

    private void MovInput()
    {
        verticalInput = movInput.y;
        horizontalInput = movInput.x;
        movAmout = Mathf.Clamp01(Mathf.Abs(horizontalInput) + Mathf.Abs(verticalInput));
        animMan.UpdateAnimValue(0, movAmout);
    }
}
