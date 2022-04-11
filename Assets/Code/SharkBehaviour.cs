using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharkBehaviour : MonoBehaviour
{
    [SerializeField] private Transform turtle;
    [SerializeField] private float movementSpeed;

    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, turtle.position, movementSpeed * Time.deltaTime);
        transform.LookAt(turtle);
    }
}
