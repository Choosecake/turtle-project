using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class SharkSpawn : MonoBehaviour
{
    [SerializeField] private GameObject boundary;
    [SerializeField] private GameObject shark;
    private Collider turtleCollider;
    private Collider boundaryCollider;
    private Vector3 position;
    private SharkBehaviour sharkBehaviour;
    private float sharkDistance = 250.0f;
    private bool hasSharkSpawned = false;

    private void Start()
    {
        boundaryCollider = boundary.GetComponent<Collider>();
    }

    private void Update()
    {
        Vector3 direction = transform.position - boundary.transform.position;
        float angle = Vector3.Angle(direction, boundary.transform.forward);

        if (transform.position.x < 0)
        {
            position = Quaternion.Euler(0, -angle, 0) * Vector3.forward * sharkDistance;
        }
        else
        {
            position = Quaternion.Euler(0, angle, 0) * Vector3.forward * sharkDistance;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other == boundaryCollider && !hasSharkSpawned)
        {
            GameObject currentShark = Instantiate(shark, position, transform.rotation);
            sharkBehaviour = currentShark.GetComponent<SharkBehaviour>();
            hasSharkSpawned = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other == boundaryCollider && hasSharkSpawned)
        {
            sharkBehaviour.isHunting = false;
            hasSharkSpawned = false;
        }
    }
}