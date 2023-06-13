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
        Interacting
    }

    [Header("==========Movement==========")]
    public float moveSpeed;
    public float sprintingSpeed;
    public float runningSpeed;
    public float normalSpeed;
    public float groundDrag;
    States state;

    [Header("==========Jump==========")]

    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    public float maxAirJumps;
    float jumpCoutner;
    bool readyToJump;

    [HideInInspector] public float walkSpeed;
    [HideInInspector] public float sprintSpeed;

    [Header("==========Keybinds==========")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;

    [Header("==========Ground Check==========")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    [HideInInspector] public TextMeshProUGUI text_speed;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        readyToJump = true;
        state = States.Moving;
    }

    private void Update()
    {
        GroundCheck();
        MyInput();
        SpeedControl();

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
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // when to jump
        if(Input.GetKey(jumpKey) && readyToJump && jumpCoutner < maxAirJumps)
        {
            readyToJump = false;
            Jump();
        }
        if (Input.GetKeyUp(jumpKey))
        {
            Invoke(nameof(ResetJump), jumpCooldown);
        }


        if (Input.GetKeyDown(sprintKey))
        {
            state = States.Sprinting;
        }
        else if (Input.GetKeyUp(sprintKey))
        {
            state = States.Moving;
        }
    }

    private void MovePlayer()
    {
        // calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        //Check if the player is sprinting
        if (state == States.Sprinting)
        {
            moveSpeed = sprintingSpeed;
        }
        else
        {
            moveSpeed = normalSpeed;
        }

        // on ground
        if(grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);

        // in air
        else if(!grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        // limit velocity if needed
        if(flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }

        text_speed.SetText("Speed: " + flatVel.magnitude);
    }

    private void Jump()
    {
        // reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        jumpCoutner++;
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }
    private void ResetJump()
    {
        readyToJump = true;
    }

    public void GroundCheck()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        if (grounded)
        {
            jumpCoutner = 0;
        }
    }
}