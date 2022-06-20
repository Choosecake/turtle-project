using System;
using UnityEngine;
using UnityEngine.Serialization;

public class FlockUnit_Jobs : MonoBehaviour
{
    [SerializeField] private float FovAngle; 
    [SerializeField] private float SmoothDamp;
    [SerializeField] private LayerMask obstacleMask;
    [SerializeField] private Vector3[] directionsToCheckWhenAvoidingObstacles;

    public Transform Transform { get; set; }
    public Vector3 CurrentVelocity { get; set; }
    public float speed { get; set; }

    public float smoothDamp => SmoothDamp;
    public float FOVAngle => FovAngle;

    private Vector3 currentObstacleAvoidanceVector;
    private Flock_Jobs assignedFlock;
    
    private void Awake()
    {
        Transform = transform;
    }

    public FlockUnit_Jobs AssignFlock(Flock_Jobs flock)
    {
        assignedFlock = flock;
        return this;
    }

    public FlockUnit_Jobs InitializeSpeed(float speed)
    {
        this.speed = speed;
        return this;
    }
    
    public Vector3 CalculateObstacleVector()
    {
        var obstacleVector = Vector3.zero;
        RaycastHit hit;
        if (Physics.Raycast(Transform.position, Transform.forward, out hit, assignedFlock.ObstacleDistance,
            obstacleMask))
        {
            obstacleVector = FindBestDirectionToAvoidObstacle();
        }
        else
        {
            currentObstacleAvoidanceVector = Vector3.zero;
        }
        return obstacleVector;
    }
    
    public Vector3 FindBestDirectionToAvoidObstacle()
    {
        if (currentObstacleAvoidanceVector != Vector3.zero)
        {
            RaycastHit hit;
            if (!Physics.Raycast(Transform.position, Transform.forward, out hit, assignedFlock.ObstacleDistance,
                obstacleMask))
            {
                return currentObstacleAvoidanceVector;
            }
        }
            
        float maxDistance = int.MinValue;
        var selectedDirection = Vector3.zero;
        for (int i = 0; i < directionsToCheckWhenAvoidingObstacles.Length; i++)
        {
            RaycastHit hit;
            var currentDirection =
                Transform.TransformDirection(directionsToCheckWhenAvoidingObstacles[i].normalized);
                
            if (Physics.Raycast(Transform.position, currentDirection, out hit, assignedFlock.ObstacleDistance,
                obstacleMask))
            {
                float currentDistance = (hit.point - Transform.position).sqrMagnitude;
                if (currentDistance > maxDistance)
                {
                    maxDistance = currentDistance;
                    selectedDirection = currentDirection;
                }
            }
            else
            {
                selectedDirection = currentDirection;
                currentObstacleAvoidanceVector = currentDirection.normalized;
                return selectedDirection.normalized;
            }
        }
        return selectedDirection.normalized;
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Array.ForEach(directionsToCheckWhenAvoidingObstacles, d => Gizmos.DrawRay(transform.position, d));
    }
}