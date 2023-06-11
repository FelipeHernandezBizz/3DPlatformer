using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    InputManager inputMan;
    PlayerMovement pM;
    public bool isInteracting;
    public bool grounded;
    public Transform orientation;
    public LayerMask whatIsGround;

    private void Awake()
    {
        inputMan = GetComponent<InputManager>();
        pM = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        inputMan.AllInputs();
        GroundCheck();
    }

    private void FixedUpdate()
    {
        pM.AllMovement();
    }

    public void GroundCheck()
    {
        if (Physics.Raycast(orientation.position, -orientation.up, 1.1f, whatIsGround))
        {
            grounded = true;
        }
        else
        {
            grounded = false;
        }
    }
}
