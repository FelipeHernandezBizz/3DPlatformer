using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseMovement : MonoBehaviour
{
    [Header("==========Movement==========")]
    public float currentSpeed;
    public float maxSpeed;
    Vector3 moveVect;

    float horizontalInput, verticalInput;

    [Header("==========Turning==========")]
    public float rotationMultiplier;
    public float rotationSpeed;

    public Transform body;
    public Transform orientation;
    public Transform player;

    [Header("==========Acceleration==========")]
    public float forwardAcceleration;

    [Header("==========Jump==========")]
    public float jumpForce;
    public float localGravity;
    public float jumpCooldown;
    bool readyToJump;

    [Header("==========Ground Check==========")]
    public LayerMask whatIsGround;
    public float playerHeight;
    bool grounded;

    [Header("==========References=========")]
    public Rigidbody rb;

    private void Start()
    {
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        readyToJump = true;
    }

    public void Update()
    {
        Move();
        Turn();
        Jump();
        GroundDetection();
        SpeedControl();
    }

    public void Move()
    {
        verticalInput = Input.GetAxisRaw("Vertical");
        horizontalInput = Input.GetAxisRaw("Horizontal");

        if (verticalInput != 0 || horizontalInput != 0)
        {
            currentSpeed = Mathf.Lerp(currentSpeed, maxSpeed, Time.deltaTime * forwardAcceleration);
        }
        else if (verticalInput == 0 || horizontalInput == 0)
        {
            currentSpeed = Mathf.Lerp(currentSpeed, 0, Time.deltaTime * forwardAcceleration);
        }

        moveVect = (orientation.transform.forward * verticalInput + orientation.transform.right * horizontalInput).normalized * currentSpeed;
        rb.velocity = new Vector3(moveVect.x, rb.velocity.y, moveVect.z);
    }

    public void Turn()
    {
        Vector3 viewDir = player.position - new Vector3(transform.position.x, player.position.y, transform.position.z);
        orientation.forward = viewDir;

        float horiInput = Input.GetAxisRaw("Horizontal");
        float vertInput = Input.GetAxisRaw("Vertical");
        Vector3 inputDir = orientation.forward * vertInput + orientation.right * horiInput;

        if (inputDir != Vector3.zero)
        {
            body.forward = Vector3.Slerp(body.forward, inputDir.normalized, Time.deltaTime * rotationSpeed);
        }
    }

    public void Jump()
    {
        if (Input.GetKey(KeyCode.Space) && grounded && readyToJump)
        {
            Debug.Log("Jump");
            readyToJump = false;
            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y + 1 * jumpForce * Time.deltaTime, rb.velocity.z);
            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    public void ResetJump()
    {
        readyToJump = true;
    }

    public void GroundDetection()
    {
        if (Physics.Raycast(orientation.position, -Vector3.up, 1.5f, whatIsGround))
        {
            grounded = true;
        }
        else
        {
            grounded = false;
            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y + localGravity * Time.deltaTime, rb.velocity.z);
        }
    }

    public void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if (flatVel .magnitude > currentSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * currentSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }
}
