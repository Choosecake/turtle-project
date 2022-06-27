using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;

[BurstCompile]
public struct TransformMoveJob : IJobParallelForTransform
{
	public NativeArray<Vector3> unitCurrentVelocities;

	[NativeDisableParallelForRestriction] public NativeArray<Vector3> unitForwardDirections;
	[NativeDisableParallelForRestriction] public NativeArray<Vector3> unitPositions;
	[NativeDisableParallelForRestriction] public NativeArray<Vector3> cohesionNeighbours;
	[NativeDisableParallelForRestriction] public NativeArray<Vector3> avoidanceNeighbours;
	[NativeDisableParallelForRestriction] public NativeArray<Vector3> aligementNeighbours;
	[NativeDisableParallelForRestriction] public NativeArray<Vector3> aligementNeighboursDirecions;
	[NativeDisableParallelForRestriction] public NativeArray<float> allUnitsSpeeds;
	[NativeDisableParallelForRestriction] public NativeArray<float> neighbourSpeeds;
	[NativeDisableParallelForRestriction] public NativeArray<Vector3> ObstacleVectors;

	public Vector3 flockPosition;
	public float cohesionDistance;
	public float avoidanceDistance;
	public float aligementDistance;
	public float boundsDistance;
	// public float obstacleDistance;
	public float cohesionWeight;
	public float avoidanceWeight;
	public float aligementWeight;
	public float boundsWeight;
	// public float obstacleWeight;
	public float fovAngle;
	public float minSpeed;
	public float maxSpeed;
	public float smoothDamp;
	public float deltaTime;
	
	// public Vector3 resultingObstacleVector;
	
	public void Execute(int index, TransformAccess transform)
	{
		//Find Neighbours
		int cohesionIndex = 0;
		int avoidanceIndex = 0;
		int aligementIndex = 0;
		for (int i = 0; i < unitPositions.Length; i++)
		{
			Vector3 currentUnitPosition = unitPositions[index];
			Vector3 currentNeighbourPosition = unitPositions[i];
			Vector3 currentNeighbourDirection = unitForwardDirections[i];
			if (currentUnitPosition != currentNeighbourPosition)
			{
				float currentDistanceToNeighbourSqr =
					Vector3.SqrMagnitude(currentUnitPosition - currentNeighbourPosition);
				if (currentDistanceToNeighbourSqr < cohesionDistance * cohesionDistance)
				{
					cohesionNeighbours[cohesionIndex] = currentNeighbourPosition;
					neighbourSpeeds[cohesionIndex] = allUnitsSpeeds[i];
					cohesionIndex++;
				}

				if (currentDistanceToNeighbourSqr < avoidanceDistance * avoidanceDistance)
				{
					avoidanceNeighbours[avoidanceIndex] = currentNeighbourPosition;
					avoidanceIndex++;
				}

				if (currentDistanceToNeighbourSqr < aligementDistance * aligementDistance)
				{
					aligementNeighbours[aligementIndex] = currentNeighbourPosition;
					aligementNeighboursDirecions[aligementIndex] = currentNeighbourDirection;
					aligementIndex++;
				}
			}
		}

		//Calculate speed
		float speed = 0f;
		if (cohesionNeighbours.Length != 0)
		{
			for (int i = 0; i < cohesionNeighbours.Length; i++)
			{
				speed += neighbourSpeeds[i];
			}

			speed /= cohesionNeighbours.Length;

		}

		speed = Mathf.Clamp(speed, minSpeed, maxSpeed);

		//Calculate cohesion
		Vector3 cohesionVector = Vector3.zero;
		if (cohesionNeighbours.Length != 0)
		{
			int cohesionNeighbourdInFOV = 0;
			for (int i = 0; i <= cohesionIndex; i++)
			{
				if (IsInFov(unitForwardDirections[index], unitPositions[index], cohesionNeighbours[i], fovAngle) &&
				    cohesionNeighbours[i] != Vector3.zero)
				{
					cohesionNeighbourdInFOV++;
					cohesionVector += cohesionNeighbours[i];
				}
			}

			cohesionVector /= cohesionNeighbourdInFOV;
			cohesionVector -= unitPositions[index];
			cohesionVector = cohesionVector.normalized * cohesionWeight;
		}

		//Calculate avoidance
		Vector3 avoidanceVector = Vector3.zero;
		if (avoidanceNeighbours.Length != 0)
		{
			int avoidanceNeighbourdInFOV = 0;
			for (int i = 0; i <= avoidanceIndex; i++)
			{
				if (IsInFov(unitForwardDirections[index], unitPositions[index], avoidanceNeighbours[i], fovAngle) &&
				    avoidanceNeighbours[i] != Vector3.zero)
				{
					avoidanceNeighbourdInFOV++;
					avoidanceVector += (unitPositions[index] - avoidanceNeighbours[i]);
				}
			}

			avoidanceVector /= avoidanceNeighbourdInFOV;
			avoidanceVector = avoidanceVector.normalized * avoidanceWeight;
		}

		//Calculate aligement
		Vector3 aligementVector = Vector3.zero;
		if (aligementNeighbours.Length != 0)
		{
			int aligementNeighbourdInFOV = 0;
			for (int i = 0; i <= aligementIndex; i++)
			{
				if (IsInFov(unitForwardDirections[index], unitPositions[index], aligementNeighbours[i], fovAngle) &&
				    aligementNeighbours[i] != Vector3.zero)
				{
					aligementNeighbourdInFOV++;
					aligementVector += aligementNeighboursDirecions[i].normalized;
				}
			}

			aligementVector /= aligementNeighbourdInFOV;
			aligementVector = aligementVector.normalized * aligementWeight;
		}

		//Calculate bounds
		Vector3 offsetToCenter = flockPosition - unitPositions[index];
		bool isNearBound = offsetToCenter.magnitude >= boundsDistance * 0.9f;
		Vector3 boundsVector = isNearBound ? offsetToCenter.normalized : Vector3.zero;
		boundsVector *= boundsWeight;

		//Move Unit
		Vector3 currentVelocity = unitCurrentVelocities[index];
		var resultingObstacleVector = ObstacleVectors[index];
		Vector3 moveVector = cohesionVector + avoidanceVector + aligementVector + boundsVector + resultingObstacleVector;

		moveVector = Vector3.SmoothDamp(unitForwardDirections[index], moveVector, ref currentVelocity, smoothDamp,
			10000, deltaTime);

		moveVector = moveVector.normalized * speed;
		if (moveVector == Vector3.zero)
		{
			moveVector = unitForwardDirections[index];
		}

		unitPositions[index] = unitPositions[index] + moveVector * deltaTime;
		unitForwardDirections[index] = moveVector.normalized;
		allUnitsSpeeds[index] = speed;
		unitCurrentVelocities[index] = currentVelocity;


	}

	private bool IsInFov(Vector3 forward, Vector3 unitPosition, Vector3 targetPosition, float angle)
	{
		return Vector3.Angle(forward, targetPosition - unitPosition) <= angle;
	}
}