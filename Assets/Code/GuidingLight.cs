using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class GuidingLight : MonoBehaviour
{
    [SerializeField] private GameObject[] waypoints;
    [SerializeField] private Transform player;
    [SerializeField] private float minDistance = 15f;
    [SerializeField] private GameObject pointLight;
    [Range(1.0f, 250.0f)] [SerializeField] private float pointLightSpeed;
    private int currentWaypoint;

    private void Start()
    {
        currentWaypoint = 0;
        pointLight.transform.position = waypoints[currentWaypoint].transform.position;
    }

    private void Update()
    {
        if (Vector3.Distance(waypoints[currentWaypoint].transform.position, player.position) < minDistance)
        {
            if (currentWaypoint < waypoints.Length-1) currentWaypoint += 1;
        }
        
        pointLight.transform.position = Vector3.Lerp(
            pointLight.transform.position,
            waypoints[currentWaypoint].transform.position,
            pointLightSpeed * Time.deltaTime);
    }
}
