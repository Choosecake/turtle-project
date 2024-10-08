using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace  Behaviours
{
    public class Boid : MonoBehaviour
    {
        [SerializeField] private float FOVAngle;
        [SerializeField] private float smoothDamp;
        [SerializeField] private LayerMask obstacleMask;
        [SerializeField] private Vector3[] directionsToCheckWhenAvoidingObstacles;

        private List<FlockUnit> cohesionNeighbours = new();
        private List<FlockUnit> avoidanceNeighbours = new();
        private List<FlockUnit> alignmentNeighbours = new();
        private Flock assignedFlock;
        private Vector3 currentVelocity;
        private Vector3 currentObstacleAvoidanceVector;
        private float speed;
        private bool isFacingObstacle;
        
        public Transform Transform { get; set; }

        private void Awake()
        {
            Transform = transform;
        }

        public FlockUnit AssignFlock(Flock flock)
        {
            assignedFlock = flock;
            Transform.forward = flock.transform.forward;
            return this;
        }

        public FlockUnit InitializeSpeed(float speed)
        {
            this.speed = speed;
            return this;
        }

        public void MoveUnit()
        {
            FindNeighbours();
            CalculateSpeed();
            
            var cohesionVector = CalculateCohesionVector() * assignedFlock.CohesionWeight;
            var alignmentVector = CalculateAlignmentVector() * assignedFlock.AlignmentWeight;
            var avoidanceVector = CalculateAvoidanceVector() * assignedFlock.AvoidanceWeight;
            var boundsVector = CalculateBoundsVector() * assignedFlock.BoundWeight;
            var obstacleVector = CalculateObstacleVector() * assignedFlock.ObstacleWeight;

            var moveVector = cohesionVector + alignmentVector + avoidanceVector + boundsVector + obstacleVector;
            moveVector = Vector3.SmoothDamp(Transform.forward, moveVector, ref currentVelocity, smoothDamp);
            moveVector = moveVector.normalized * speed;
            if (moveVector == Vector3.zero)
                moveVector = transform.forward;
            
            Transform.forward = moveVector;
            transform.position += moveVector * Time.deltaTime;
        }

        private void FindNeighbours()
        {
            cohesionNeighbours.Clear();
            avoidanceNeighbours.Clear();
            alignmentNeighbours.Clear();
            var allUnits = assignedFlock.allUnits;
            for (int i = 0; i < allUnits.Length; i++)
            {
                var currentUnit = allUnits[i];
                if (currentUnit != this)
                {
                    float currentNeighbourDistanceSqr =
                        Vector3.SqrMagnitude(currentUnit.Transform.position - Transform.position);
                    if (currentNeighbourDistanceSqr <= assignedFlock.CohesionDistance * assignedFlock.CohesionDistance)
                    {
                        cohesionNeighbours.Add(currentUnit);
                    }
                    if (currentNeighbourDistanceSqr <= assignedFlock.AvoidanceDistance * assignedFlock.AvoidanceDistance)
                    {
                        avoidanceNeighbours.Add(currentUnit);
                    }
                    if (currentNeighbourDistanceSqr <= assignedFlock.AlignmentDistance * assignedFlock.AlignmentDistance)
                    {
                        alignmentNeighbours.Add(currentUnit);
                    }
                }
            }
        }

        private void CalculateSpeed()
        {
            if (!cohesionNeighbours.Any())
            {
                return;
            }
            speed = 0;
            for (int i = 0; i < cohesionNeighbours.Count(); i++)
            {
                speed += cohesionNeighbours[i].speed;
            }

            speed /= cohesionNeighbours.Count();
            speed = Mathf.Clamp(speed, assignedFlock.MinSpeed, assignedFlock.MaxSpeed);
        }

        private Vector3 CalculateCohesionVector()
        {
            var cohesionVector = Vector3.zero;
            if (cohesionNeighbours.Count == 0)
                return cohesionVector;
            int neighboursInFOV = 0;
            foreach (var unit in cohesionNeighbours.Where(t => IsInFOV(t.Transform.position)))
            {
                neighboursInFOV++;
                cohesionVector += unit.Transform.position;
            }
            cohesionVector /= neighboursInFOV;
            cohesionVector -= Transform.position;
            cohesionVector = cohesionVector.normalized;
            return cohesionVector;
        }

        private Vector3 CalculateAlignmentVector()
        {
            var alignmentVector = Transform.forward;
            if (alignmentNeighbours.Count == 0)
                return alignmentVector;
            int neighboursInFOV = 0;
            for (int i = 0; i < alignmentNeighbours.Count; i++)
            {
                if (IsInFOV(alignmentNeighbours[i].Transform.position))
                {
                    neighboursInFOV++;
                    alignmentVector += alignmentNeighbours[i].Transform.forward;
                }
            }
            alignmentVector /= neighboursInFOV;
            alignmentVector = alignmentVector.normalized;
            return alignmentVector;
        }

        private Vector3 CalculateAvoidanceVector()
        {
            var avoidanceVector = Vector3.zero;
            if (avoidanceNeighbours.Count == 0)
                return avoidanceVector;
            int neighboursInFOV = 0;
            for (int i = 0; i < avoidanceNeighbours.Count; i++)
            {
                if (IsInFOV(avoidanceNeighbours[i].Transform.position))
                {
                    neighboursInFOV++;
                    avoidanceVector += (Transform.position - avoidanceNeighbours[i].Transform.position);
                }
            }

            avoidanceVector /= neighboursInFOV;
            avoidanceVector = avoidanceVector.normalized;
            return avoidanceVector;
        }

        public Vector3 CalculateBoundsVector()
        {
            var offsetToCenter = assignedFlock.transform.position - Transform.position;
            bool isNearCenter = (offsetToCenter.magnitude >= assignedFlock.BoundDistance * 0.9f);
            return isNearCenter ? offsetToCenter.normalized : Vector3.zero;
        }

        public Vector3 CalculateObstacleVector()
        {
            var obstacleVector = Vector3.zero;
            RaycastHit hit;
            isFacingObstacle = Physics.Raycast(Transform.position, Transform.forward, out hit,
                assignedFlock.ObstacleDistance,
                obstacleMask);
            if (isFacingObstacle)
            {
                obstacleVector = FindBestDirectionToAvoidObstacle(isFacingObstacle);
            }
            else
            {
                currentObstacleAvoidanceVector = Vector3.zero;
            }
            return obstacleVector;
        }

        public Vector3 FindBestDirectionToAvoidObstacle(bool isFacingObstacle)
        {
            if (currentObstacleAvoidanceVector != Vector3.zero)
            {
                if (!isFacingObstacle)
                {
                    return currentObstacleAvoidanceVector;
                }
            }
            
            float maxDistance = int.MinValue;
            var selectedDirection = Vector3.zero;
            directionsToCheckWhenAvoidingObstacles.Shuffle();
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

        private bool IsInFOV(Vector3 position)
        {
            return Vector3.Angle(Transform.forward, position - Transform.position) <= FOVAngle;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Array.ForEach(directionsToCheckWhenAvoidingObstacles, d => Gizmos.DrawRay(transform.position, d));
            Gizmos.color = isFacingObstacle ? Color.red : Color.green;
            Gizmos.DrawRay(transform.position, transform.forward * assignedFlock.ObstacleDistance);
        }
    }
}