using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UI;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class GuidingLight : MonoBehaviour, GameEnder
{ 
    [NotNull][SerializeField] private GameObject[] waypoints;
    [SerializeField] private Transform player;
    [SerializeField] private float minDistance = 15f;
    [SerializeField] private GameObject pointLight;
    [Range(1.0f, 250.0f)] [SerializeField] private float pointLightSpeed;
    private int currentWaypoint;
    private bool hasFinishedPath;

    public Action OnPathFinished;

    private void Start()
    {
        currentWaypoint = 0;
        pointLight.transform.position = waypoints[currentWaypoint].transform.position;
    }

    private void Update()
    {
        if (hasFinishedPath == true) return;
        
        if (Vector3.Distance(waypoints[currentWaypoint].transform.position, player.position) < minDistance)
        {
            if (currentWaypoint >= waypoints.Length-1)
            {
                OnCriticalPointReached?.Invoke();
                hasFinishedPath = true;
                return;
            }
            
            if (currentWaypoint < waypoints.Length - 1)
            {
                currentWaypoint += 1;
                return;
            }
            if (waypoints[currentWaypoint+1].Equals(null))
            {
                Debug.LogWarning("The next array slot has no reference!\n" +
                               "Please, assign a Waypoint to it or reduce the array's length :)");
                return;
            }
            
        }
        
        pointLight.transform.position = Vector3.Lerp(
            pointLight.transform.position,
            waypoints[currentWaypoint].transform.position,
            pointLightSpeed * Time.deltaTime);
    }

    public Action OnCriticalPointReached { get; set; }
}
