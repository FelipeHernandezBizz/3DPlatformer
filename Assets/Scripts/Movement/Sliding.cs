using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sliding : MonoBehaviour
{
    [Header("==========Attributes==========")]
    public float maxSlideTime;
    public float slideForce;
    public float slideYScale;
    float slideTimer;

    [Header("==========References==========")]
    public Transform orientation;
    public Transform playeerObj;
    Rigidbody rb;
    PlayerMovement pm;
    InputManager iM;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerMovement>();
        iM = GetComponent<InputManager>();
    }

    private void Update()
    {
        if (iM.playerInput.PlayerMovement.Crouch.WasPressedThisFrame() && pm.state == PlayerMovement.States.Sprinting && (iM.horizontalInput != 0 || iM.verticalInput != 0))
        {
            StartSlide();
        }
        if (iM.playerInput.PlayerMovement.Crouch.WasReleasedThisFrame() && pm.isSliding && !pm.underRoof)
        {
            StopSlide();
        }

        if (pm.isSliding && !pm.underRoof && pm.state != PlayerMovement.States.Crouching && !iM.playerInput.PlayerMovement.Crouch.IsPressed())
        {
            StopSlide();
            pm.state = PlayerMovement.States.Moving;
        }
    }

    private void FixedUpdate()
    {
        if (pm.isSliding) { SlideMovement(); }
    }

    public void StartSlide()
    {
        pm.isSliding = true;
        transform.localScale = new Vector3(transform.localScale.x, slideYScale, transform.localScale.z);
        rb.AddForce(-transform.up * 5f, ForceMode.Impulse);
        Debug.Log("Sliding");
        slideTimer = maxSlideTime;
    }

    public void SlideMovement()
    {
        Vector3 inputDir = orientation.forward * iM.verticalInput + orientation.right * iM.horizontalInput;
        if (!pm.OnSlope() && rb.velocity.y > -0.1f)
        {
            rb.AddForce(inputDir.normalized * slideForce, ForceMode.Force);
            pm.SetUnderRoof();
            slideTimer -= Time.deltaTime;
            
        }
        else
        {
            rb.AddForce(pm.SlopeDirection(inputDir) * slideForce, ForceMode.Force);
        }
        if (slideTimer <= 0) { StopSlide(); }
    }

    public void StopSlide()
    {
        pm.isSliding = false;

        if (!pm.underRoof)
        {
            pm.state = PlayerMovement.States.Moving;
            transform.localScale = new Vector3(transform.localScale.x, pm.GetStartScale(), transform.localScale.z);
        }
    }
}
