using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class GuidingLight : MonoBehaviour
{ 
    [NotNull][SerializeField] private GameObject[] waypoints;
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
            if (waypoints[currentWaypoint+1].Equals(null))
            {
                Debug.LogWarning("The next array slot has no reference!\n" +
                               "Please, assign a Waypoint to it or reduce the array's length :)");
                return;
            }
            if ((currentWaypoint < waypoints.Length-1)) currentWaypoint += 1;
            
        }
        
        pointLight.transform.position = Vector3.Lerp(
            pointLight.transform.position,
            waypoints[currentWaypoint].transform.position,
            pointLightSpeed * Time.deltaTime);
    }
}
