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
    private float sharkDistance = 250.0f;
    private bool hasSharkSpawned = false;

    private void Start()
    {
        boundaryCollider = boundary.GetComponent<Collider>();
        if (boundary == null)
        {
            boundary = GameObject.Find("Boundary");
        }
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
            Instantiate(shark, position, transform.rotation);
            hasSharkSpawned = true;
        }
    }
}
