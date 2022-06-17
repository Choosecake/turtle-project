using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace Behaviours
{
    [BurstCompile]
    public struct MoveJob : IJobParallelFor
    {
        
        [NativeDisableParallelForRestriction] public NativeArray<Vector3> unitForwardDirections;
        [NativeDisableParallelForRestriction] public NativeArray<Vector3> unitCurrentVelocities;
        [NativeDisableParallelForRestriction] public NativeArray<Vector3> unitPositions;
        [NativeDisableParallelForRestriction] public NativeArray<Vector3> cohesionNeighbours;
        [NativeDisableParallelForRestriction] public NativeArray<Vector3> avoidanceNeighbours;
        [NativeDisableParallelForRestriction] public NativeArray<Vector3> alignmentNeighbours;
        [NativeDisableParallelForRestriction] public NativeArray<Vector3> alignmentNeighboursDirections;
        [NativeDisableParallelForRestriction] public NativeArray<float> allUnitsSpeeds;
        [NativeDisableParallelForRestriction] public NativeArray<float> neighboursSpeeds;

        public Vector3 flockPosition;
        public Vector3 playerPosition;
        
        public float cohesionDistance;
        public float avoidanceDistance;
        public float alignmentDistance;
        public float boundsDistance;
        public float obstacleDistance;

        public float cohesionWeight;
        public float avoidanceWeight;
        public float alignmentWeight;
        public float boundsWeight;
        public float obstacleWeight;

        public float fovAngle;
        public float minSpeed;
        public float maxSpeed;
        public float smoothDamp;
        public float deltaTime;

        public void Execute(int executionIndex)
        {
            //Find Neighbours
            int cohesionIndex = 0;
            int avoidanceIndex = 0;
            int alignmentIndex = 0;
            for (int i = 0; i < unitPositions.Length; i++)
            {
                Vector3 currentNeighbourPosition = unitPositions[i];
                Vector3 currentNeighbourDirection = unitForwardDirections[i];
                Vector3 currentUnitPosition = unitPositions[executionIndex];
                if (currentUnitPosition != currentNeighbourPosition)
                {
                    float currentDistanceToNeighbourSqr =
                        Vector3.SqrMagnitude(currentUnitPosition - currentNeighbourPosition);
                    if (currentDistanceToNeighbourSqr < cohesionDistance * cohesionDistance)
                    {
                        cohesionNeighbours[cohesionIndex] = currentNeighbourPosition;
                        neighboursSpeeds[cohesionIndex] = allUnitsSpeeds[i];
                        cohesionIndex++;
                    }

                    if (currentDistanceToNeighbourSqr < avoidanceDistance * alignmentDistance)
                    {
                        avoidanceNeighbours[avoidanceIndex] = currentNeighbourPosition;
                        avoidanceIndex++;
                    }

                    if (currentDistanceToNeighbourSqr < alignmentDistance * alignmentDistance)
                    {
                        alignmentNeighbours[alignmentIndex] = currentNeighbourPosition;
                        alignmentNeighboursDirections[alignmentIndex] = currentNeighbourDirection;
                        alignmentIndex++;
                    }
                }
            }
            
            //Calculate speed
            float speed = 0f;
            if (cohesionNeighbours.Length != 0)
            {
                for (int i = 0; i < cohesionNeighbours.Length; i++)
                {
                    if (neighboursSpeeds[i] != 0)
                    {
                        speed += neighboursSpeeds[i];
                    }
                }
                speed /= cohesionNeighbours.Length;
            }
            
            //Calculate cohesion 
            Vector3 cohesionVector = Vector3.zero;
            if (cohesionNeighbours.Length != 0)
            {
                int cohesionNeighboursInFOV = 0;
                for (int i = 0; i < cohesionNeighbours.Length; i++)
                {
                    if (IsInFOV(unitForwardDirections[executionIndex], unitPositions[executionIndex],
                        cohesionNeighbours[i], fovAngle) && cohesionNeighbours[i] != Vector3.zero)
                    {
                        cohesionNeighboursInFOV++;
                        cohesionVector += cohesionNeighbours[i];
                    } 
                }
                cohesionVector /= cohesionNeighboursInFOV;
                cohesionVector -= unitPositions[executionIndex];
                cohesionVector = cohesionVector.normalized * cohesionWeight;
            }

            //Calculate avoidance
            Vector3 avoidanceVector = Vector3.zero;
            if (avoidanceNeighbours.Length != 0)
            {
                int avoidanceNeighboursInFOV = 0;
                for (int i = 0; i < avoidanceNeighbours.Length; i++)
                {
                    if (IsInFOV(unitForwardDirections[executionIndex], unitPositions[executionIndex],
                        avoidanceNeighbours[i], fovAngle) && avoidanceNeighbours[i] != Vector3.zero)
                    {
                        avoidanceNeighboursInFOV++;
                        avoidanceVector += (unitPositions[executionIndex] - avoidanceNeighbours[i]);
                    }
                }

                avoidanceVector /= avoidanceNeighboursInFOV;
                avoidanceVector = avoidanceVector.normalized * avoidanceWeight;
            }
            
            //Calculate alignment
            Vector3 alignmentVector = Vector3.zero;
            if (alignmentNeighbours.Length != 0)
            {
                int alignmentNeighboursInFOV = 0;
                for (int i = 0; i < alignmentNeighbours.Length; i++)
                {
                    if (IsInFOV(unitForwardDirections[executionIndex], unitPositions[executionIndex],
                        alignmentNeighbours[i], fovAngle) && alignmentNeighbours[i] != Vector3.zero)
                    {
                        alignmentNeighboursInFOV++;
                        alignmentVector += alignmentNeighboursDirections[i].normalized;
                    }
                }

                alignmentVector /= alignmentNeighboursInFOV;
                alignmentVector = alignmentVector.normalized * alignmentWeight;
            }

            Vector3 currentVelocity = unitCurrentVelocities[executionIndex];
            Vector3 moveVector = cohesionVector + avoidanceVector + alignmentVector;
            
            moveVector = Vector3.SmoothDamp(unitForwardDirections[executionIndex], moveVector, ref currentVelocity,
                smoothDamp, 10000, deltaTime);
            moveVector = moveVector.normalized * speed;
            if (moveVector == Vector3.zero)
            {
                moveVector = unitForwardDirections[executionIndex];
            }

            unitPositions[executionIndex] += moveVector * deltaTime;
            unitForwardDirections[executionIndex] = moveVector.normalized;
            allUnitsSpeeds[executionIndex] = speed;
            unitCurrentVelocities[executionIndex] = currentVelocity;
        }
        
        public static bool IsInFOV(Vector3 forward, Vector3 unitPosition, Vector3 targetPosition, float angle)
        {
            return Vector3.Angle(forward, targetPosition - unitPosition) <= angle;
        }
    }
}