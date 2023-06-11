using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("==========Basic Movement==========")]
    public float currentSpeed;
    public float walkingSpeed;
    public float sprintingSpeed;
    public float maxSpeed;
    Vector3 movDirection;
    Transform cameraObj;

    [Header("==========Accelerations==========")]
    public float walkingAcceleration;
    public float runningAcceleration;
    public float breakingDeceleration;

    [Header("==========Rotation==========")]
    public float rotationSpeed;

    [Header("==========References==========")]
    InputManager inputMan;
    PlayerManager pM;
    Rigidbody rb;

    private void Awake()
    {
        inputMan = GetComponent<InputManager>();
        rb = GetComponent<Rigidbody>();
        pM = GetComponent<PlayerManager>();
        cameraObj = Camera.main.transform;
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void AllMovement()
    {
        //if (pM.isInteracting)
        //    return;

        Movement();
        Turning();
    }

    private void Movement()
    {
        if (inputMan.verticalInput != 0 || inputMan.horizontalInput != 0)
        {
            if (!inputMan.sprinting) { maxSpeed = walkingSpeed; }
            else { maxSpeed = sprintingSpeed; }
            currentSpeed = Mathf.Lerp(currentSpeed, maxSpeed, Time.deltaTime * walkingAcceleration);
        }
        else if (inputMan.verticalInput == 0 || inputMan.horizontalInput == 0)
        {
            currentSpeed = Mathf.Lerp(currentSpeed, 0, Time.deltaTime * breakingDeceleration);
        }

        movDirection = cameraObj.forward * inputMan.verticalInput;
        movDirection = movDirection + cameraObj.right * inputMan.horizontalInput;
        movDirection.Normalize();
        movDirection.y = 0;
        movDirection *= currentSpeed;

        Vector3 movVel = movDirection;
        rb.velocity = movVel;
    }

    private void Turning()
    {
        Vector3 dir = Vector3.zero;
        dir = cameraObj.forward * inputMan.verticalInput;
        dir += (cameraObj.right * inputMan.horizontalInput);
        dir.Normalize();
        dir.y = 0;

        if(dir == Vector3.zero)
        {
            dir = transform.forward;
        }

        Quaternion targetRot = Quaternion.LookRotation(dir);
        Quaternion playerRotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * rotationSpeed);

        transform.rotation = playerRotation;
    }

    public void Jump()
    {

    }
}
