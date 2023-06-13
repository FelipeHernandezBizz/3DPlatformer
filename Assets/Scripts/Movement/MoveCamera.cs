using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    [Header("==========References==========")]
    public Transform orientation;
    public Transform playerObj;
    public Transform player;

    public InputManager inputMan;

    [Header("==========Attributes==========")]
    public float rotationSpeed;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        Vector3 viewDir = player.position - new Vector3(transform.position.x, player.position.y, transform.position.z);
        orientation.forward = viewDir.normalized;

        float horizontalInput = inputMan.horizontalInput;
        float verticalInput = inputMan.verticalInput;

        Vector3 inputDir = orientation.forward * verticalInput;

        if (inputDir.y != 0)
        {
            playerObj.forward = Vector3.Slerp(playerObj.forward, inputDir.normalized, Time.deltaTime * rotationSpeed);
        }
    }
}
