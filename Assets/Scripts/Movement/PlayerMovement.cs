using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerMovement : MonoBehaviour
{
    public enum States
    {
        Sprinting,
        Moving,
        Crouching,
        Air,
        Interacting
    }

    [Header("==========Movement==========")]
    public float moveSpeed;
    public float sprintingSpeed;
    public float runningSpeed;
    public float walkingSpeed;
    public float groundDrag;
    public States state;

    [Header("==========Jump==========")]

    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    public float maxAirJumps;
    float jumpCoutner;
    bool readyToJump;

    [Header("==========Crouch==========")]
    public float crouchSpeed;
    public float crouchYScale;
    private float startYScale;
    bool crouching;
    public bool underRoof;

    [Header("==========Slope Movement==========")]
    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    bool exitSlope;

    [Header("==========Ground Check==========")]
    public float playerHeight;
    public LayerMask whatIsGround;
    public bool grounded;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;
    InputManager inputMan;

    public TextMeshProUGUI text_speed;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        inputMan = GetComponent<InputManager>();
        rb.freezeRotation = true;
        startYScale = transform.localScale.y;

        readyToJump = true;
    }

    private void Update()
    {
        MyInput();
        SpeedControl();
        StateHandler();

        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        if (grounded)
        {
            jumpCoutner = 0;
        }

        // handle drag
        if (grounded)
            rb.drag = groundDrag;
        else
            rb.drag = 0;
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        horizontalInput = inputMan.movVect.x;
        verticalInput = inputMan.movVect.y;

        // when to jump
        if (inputMan.playerInput.PlayerMovement.Jump.WasPerformedThisFrame() && readyToJump && jumpCoutner < maxAirJumps)
        {
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }

        if (inputMan.playerInput.PlayerMovement.Crouch.WasPressedThisFrame())
        {
            crouching = true;
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }
        if (inputMan.playerInput.PlayerMovement.Crouch.WasReleasedThisFrame() && !underRoof)
        {
            crouching = false;
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
        }

        if (!inputMan.playerInput.PlayerMovement.Crouch.IsPressed() && !underRoof)
        {
            crouching = false;
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
        }
    }

    private void StateHandler()
    {
        //Crouching
        if (crouching && grounded)
        {
            underRoof = Physics.Raycast(transform.position, transform.up, playerHeight * 0.5f + 0.2f, whatIsGround);
            state = States.Crouching;
            moveSpeed = crouchSpeed;
        }

        //Sprinting
        else if (inputMan.playerInput.PlayerMovement.Sprint.IsPressed() && grounded && !underRoof)
        {
            state = States.Sprinting;
            moveSpeed = sprintingSpeed;
        }

        else if (grounded && !underRoof)
        {
            state = States.Moving;
            moveSpeed = walkingSpeed;
        }

        else
        {
            state = States.Air;
        }
    }

    private void MovePlayer()
    {
        // calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        //On Slope
        if (OnSlope() && !exitSlope)
        {
            rb.AddForce(SlopeDirection() * moveSpeed * 10, ForceMode.Force);
            if (rb.velocity.y > 0) { rb.AddForce(Vector3.down * 80, ForceMode.Force); }
        }

        // on ground
        if(grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);

        // in air
        else if(!grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);

        rb.useGravity = !OnSlope();
    }

    private void SpeedControl()
    {
        //Limited speed on slope
        if (OnSlope() && !exitSlope)
        {
            if(rb.velocity.magnitude > moveSpeed) { rb.velocity = rb.velocity.normalized * moveSpeed; }
        }

        //On ground and air
        else
        {
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            // limit velocity if needed
            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }

            text_speed.SetText("Speed: " + flatVel.magnitude);
        }
        
    }

    private void Jump()
    {
        // reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        jumpCoutner++;
        exitSlope = true;
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }
    private void ResetJump()
    {
        readyToJump = true;
        exitSlope = false;
    }

    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, -transform.up, out slopeHit, playerHeight * 0.5f + 0.3f, whatIsGround))
        {
            float angle = Vector3.Angle(transform.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }
        else { return false; }
    }

    private Vector3 SlopeDirection()
    {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }
}