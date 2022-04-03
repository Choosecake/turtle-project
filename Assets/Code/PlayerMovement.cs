using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMovement : MonoBehaviour
{
    public Vector3 playerMovement;
    private float moveSpeed = 5.0f;

    private void Update()
    {
        // print(moveSpeed);
        float vertical = Input.GetAxisRaw("Vertical");
        float horizontal = Input.GetAxisRaw("Horizontal") * 0.4f;

        playerMovement = new Vector3(horizontal, 0f, vertical) * moveSpeed * Time.deltaTime;
        transform.Translate(playerMovement, Space.Self);

        if (vertical != 0)
        {
            transform.Rotate(Vector3.forward * -horizontal, Space.Self);
        }
    }
}