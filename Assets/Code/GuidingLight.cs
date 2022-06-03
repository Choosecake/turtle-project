using System;
using System.Collections;
using System.Collections.Generic;
using Code;
using JetBrains.Annotations;
using UI;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class GuidingLight : MonoBehaviour, GameEnder
{ 
    [NotNull][SerializeField] private GameObject[] waypoints;
    [Header("External Refs")]
    [SerializeField] private Transform player;
    [SerializeField] private TransitionScreen victoryTransitionScreen;
    [Header("Point Light")]
    [SerializeField] private GameObject pointLight;
    [Range(1.0f, 250.0f)] [SerializeField] private float pointLightSpeed;
    [SerializeField] private float minDistance = 15f;

    private int currentWaypoint;
    private bool hasFinishedPath;

    public Action OnPathFinished;

    private void Awake()
    {
        if (victoryTransitionScreen == null)
        {
            victoryTransitionScreen = GameplayManager.Instance.VictoryTransitionScreen;
        }
        victoryTransitionScreen.GameEnder = gameObject;

        if (player == null)
        {
            player = GameplayManager.Instance.PlayerGameObject.transform;
        }
    }

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
