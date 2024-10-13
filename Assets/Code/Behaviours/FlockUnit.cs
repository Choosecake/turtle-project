using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace  Behaviours
{
    //BOID
    public class FlockUnit : MonoBehaviour
    {
        [Range(0,180)][SerializeField] private float FOVAngle;
        [SerializeField] private float smoothDamp;
        [SerializeField] private LayerMask obstacleMask;
        [SerializeField] private Vector3[] directionsToCheckWhenAvoidingObstacles;
        
        [ReadOnly][SerializeField] private Vector3 cohesionVector;
        [ReadOnly][SerializeField] private Vector3 alignmentVector;
        [ReadOnly][SerializeField] private Vector3 avoidanceVector;
        [ReadOnly][SerializeField] private Vector3 boundsVector;
        [ReadOnly][SerializeField] private Vector3 obstacleVector;
        [ReadOnly][SerializeField] private Vector3 fleeVector;
        [ReadOnly][SerializeField] private Vector3 preyVector;
        [ReadOnly][SerializeField] private Vector3 moveVector;

        private List<FlockUnit> cohesionNeighbours = new();
        private List<FlockUnit> avoidanceNeighbours = new();
        private List<FlockUnit> alignmentNeighbours = new();
        private Flock assignedFlock;
        private Vector3 currentVelocity;
        private Vector3 currentObstacleAvoidanceVector;
        private float speed;
        private bool isFacingObstacle;
        
        public Transform Transform { get; private set; }
        public Flock Flock => assignedFlock; //NEW

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
            
            cohesionVector = CalculateCohesionVector() * assignedFlock.CohesionWeight;
            alignmentVector = CalculateAlignmentVector() * assignedFlock.AlignmentWeight;
            avoidanceVector = CalculateAvoidanceVector() * assignedFlock.AvoidanceWeight;
            boundsVector = CalculateBoundsVector() * assignedFlock.BoundWeight;
            obstacleVector = CalculateObstacleVector() * assignedFlock.ObstacleWeight;
            fleeVector = CalculateFleeVector(); //NEW
            preyVector = CalculatePreyVector(); //NEW
            
            moveVector = cohesionVector + alignmentVector + avoidanceVector + boundsVector + obstacleVector + fleeVector + preyVector;
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
            foreach (var currentUnit in allUnits)
            {
                if (currentUnit == this)
                {
                    continue;
                }
                
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
            return cohesionVector.normalized;
        }

        private Vector3 CalculateAlignmentVector()
        {
            var alignmentVector = Transform.forward;
            if (alignmentNeighbours.Count == 0)
                return alignmentVector;
            int neighboursInFOV = 0;
            foreach (var unit in alignmentNeighbours.Where(t => IsInFOV(t.Transform.position)))
            {
                neighboursInFOV++;
                alignmentVector += unit.Transform.forward;
            }
            alignmentVector /= neighboursInFOV;
            return alignmentVector.normalized;
        }

        private Vector3 CalculateAvoidanceVector()
        {
            var avoidanceVector = Vector3.zero;
            if (avoidanceNeighbours.Count == 0)
                return avoidanceVector;
            int neighboursInFOV = 0;
            foreach (var unit in avoidanceNeighbours.Where(t => IsInFOV(t.Transform.position)))
            {
                neighboursInFOV++;
                avoidanceVector += (Transform.position - unit.Transform.position);
            }

            avoidanceVector /= neighboursInFOV;
            return avoidanceVector.normalized;
        }

        public Vector3 CalculateBoundsVector()
        {
            var offsetToCenter = assignedFlock.transform.position - Transform.position;
            bool isEvading = (offsetToCenter.magnitude >= assignedFlock.BoundDistance * 0.9f);
            return isEvading ? offsetToCenter.normalized : Vector3.zero;
        }

        //NEW
        public Vector3 CalculateFleeVector()
        {
            var totalResult = Vector3.zero;
            foreach (var factor in assignedFlock.FleeFactors)
            {
                int unitCount = 0;
                var factorResult = Vector3.zero;
                if (factor.flock.isActiveAndEnabled == false)
                    continue;
                foreach (var unit in factor.flock.allUnits)
                {
                    if (Vector3.SqrMagnitude(unit.Transform.position - Transform.position) > factor.SqrDistance)
                        continue;
                    unitCount++;
                    factorResult += (Transform.position - unit.Transform.position);
                }
                var weightedResult = (factorResult / unitCount).normalized * factor.weight;
                totalResult += weightedResult;
            }
            return totalResult;
        }
        
        //NEW
        public Vector3 CalculatePreyVector()
        {
            var totalResult = Vector3.zero;
            foreach (var factor in assignedFlock.PreyFactors)
            {
                int unitCount = 0;
                var factorResult = Vector3.zero;
                if (factor.flock.isActiveAndEnabled == false)
                    continue;
                foreach (var unit in factor.flock.allUnits)
                {
                    if (Vector3.SqrMagnitude(unit.Transform.position - Transform.position) > factor.SqrDistance)
                        continue;
                    unitCount++;
                    factorResult += (unit.Transform.position - Transform.position);
                }
                var weightedResult = (factorResult / unitCount).normalized * factor.weight;
                totalResult += weightedResult;
            }
            return totalResult;
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
        
        //It's still kinda broken
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
            foreach (var t in directionsToCheckWhenAvoidingObstacles)
            {
                var currentDirection =
                    Transform.TransformDirection(t.normalized);
                
                if (Physics.Raycast(Transform.position, currentDirection, out var hit, assignedFlock.ObstacleDistance,
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
            if (Application.isPlaying == false)
            {
                return;
            }
            Gizmos.color = isFacingObstacle ? Color.red : Color.green;
            Gizmos.DrawRay(transform.position, transform.forward * assignedFlock.ObstacleDistance);
        }
    }
}