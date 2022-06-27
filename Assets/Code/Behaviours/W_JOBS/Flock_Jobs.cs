using Unity.Collections;
using Unity.Jobs;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Profiling;
using Random = UnityEngine.Random;

public class Flock_Jobs : MonoBehaviour
{
	[Header("Optimization Setup")] 
	[SerializeField] private OptimizationMethod optimizationMethod; 
	
	[Header("Spawn Setup")]
	[SerializeField] private FlockUnit_Jobs flockUnitPrefab;
	[SerializeField] private int flockSize;
	[SerializeField] private Vector3 spawnBounds;
	[SerializeField] private Transform flockParent;
	[SerializeField] private bool useObstacles;

	[Header("Speed Setup")]
	[Range(0,10)] [SerializeField] private float minSpeed;
	[Range(0,10)] [SerializeField] private float maxSpeed;


	[Header("Detection Distances")]
	[Range(0,10)][SerializeField] private float cohesionDistance;
	[Range(0,10)][SerializeField] private float avoidanceDistance;
	[Range(0,10)][SerializeField] private float alignmentDistance;
	[Range(0, 100)] [SerializeField] private float boundDistance;
	[Range(0, 10)] [SerializeField] private float obstacleDistance;


	[Header("Behaviour Weights")]
	[Range(0,10)][SerializeField] private float cohesionWeight;
	[Range(0,10)][SerializeField] private float avoidanceWeight;
	[Range(0,10)][SerializeField] private float alignmentWeight;
	[Range(0, 10)] [SerializeField] private float boundWeight;
	[Range(0, 100)] [SerializeField] private float obstacleWeight;

	public float CohesionDistance => cohesionDistance;
	public float AvoidanceDistance => avoidanceDistance;
	public float AlignmentDistance => alignmentDistance;
	public float BoundDistance => boundDistance;
	public float ObstacleDistance => obstacleDistance;

	public float CohesionWeight => cohesionWeight;
	public float AvoidanceWeight => avoidanceWeight;
	public float AlignmentWeight => alignmentWeight;
	public float BoundWeight => boundWeight;
	public float ObstacleWeight => obstacleWeight;

	public float MinSpeed => minSpeed;
	public float MaxSpeed => maxSpeed;
	
	public FlockUnit_Jobs[] allUnits { get; set; }

	private static readonly ProfilerMarker ProfilerWithJobs =
		new ProfilerMarker(ProfilerCategory.Scripts, "JobsTester.WithJobs");

	private void Start()
	{
		GenerateUnits();
	}

	public void SetRandomFlockValues()
	{
		boundDistance = Random.Range(5f, 30f);
		cohesionWeight = Random.Range(1f, 10f);
		cohesionWeight = Random.Range(1f, 10f);
		avoidanceWeight = Random.Range(1f, 10f);
		minSpeed = Random.Range(1f, 2f);
		maxSpeed = Random.Range(2f, 5f);
	}

	private void Update()
	{
		switch (optimizationMethod)
		{
			case OptimizationMethod.WithJobs:
				using (ProfilerWithJobs.Auto())
				{
					MultiThreadedUpdate();
				}
				break;
		}
	}

	private void MultiThreadedUpdate()
	{
		NativeArray<Vector3> unitForwardDirections = new NativeArray<Vector3>(allUnits.Length, Allocator.TempJob);
		NativeArray<Vector3> unitCurrentVelocities = new NativeArray<Vector3>(allUnits.Length, Allocator.TempJob);
		NativeArray<Vector3> unitPositions = new NativeArray<Vector3>(allUnits.Length, Allocator.TempJob);
		NativeArray<Vector3> cohesionNeighbours = new NativeArray<Vector3>(allUnits.Length, Allocator.TempJob);
		NativeArray<Vector3> avoidanceNeighbours = new NativeArray<Vector3>(allUnits.Length, Allocator.TempJob);
		NativeArray<Vector3> aligementNeighbours = new NativeArray<Vector3>(allUnits.Length, Allocator.TempJob);
		NativeArray<Vector3> aligementNeighboursDirecions = new NativeArray<Vector3>(allUnits.Length, Allocator.TempJob);
		NativeArray<Vector3> obstacleVectors = new NativeArray<Vector3>(allUnits.Length, Allocator.TempJob);
		NativeArray<float> allUnitsSpeeds = new NativeArray<float>(allUnits.Length, Allocator.TempJob);
		NativeArray<float> neighbourSpeeds = new NativeArray<float>(allUnits.Length, Allocator.TempJob);

		for (int i = 0; i < allUnits.Length; i++)
		{
			unitForwardDirections[i] = allUnits[i].Transform.forward;
			unitCurrentVelocities[i] = allUnits[i].CurrentVelocity;
			unitPositions[i] = allUnits[i].Transform.position;
			cohesionNeighbours[i] = Vector3.zero;
			avoidanceNeighbours[i] = Vector3.zero;
			aligementNeighbours[i] = Vector3.zero;
			aligementNeighboursDirecions[i] = Vector3.zero;
			allUnitsSpeeds[i] = allUnits[i].speed;
			neighbourSpeeds[i] = 0f;

			obstacleVectors[i] =
				useObstacles
					? allUnits[i].CalculateObstacleVector() * obstacleWeight
					: Vector3.zero;
		}

		MoveJob moveJob = new MoveJob
		{
			unitForwardDirections = unitForwardDirections,
			unitCurrentVelocities = unitCurrentVelocities,
			unitPositions = unitPositions,
			cohesionNeighbours = cohesionNeighbours,
			avoidanceNeighbours = avoidanceNeighbours,
			aligementNeighbours = aligementNeighbours,
			aligementNeighboursDirecions = aligementNeighboursDirecions,
			allUnitsSpeeds = allUnitsSpeeds,
			neighbourSpeeds = neighbourSpeeds,
			cohesionDistance = cohesionDistance,
			avoidanceDistance = avoidanceDistance,
			aligementDistance = alignmentDistance,
			boundsDistance = boundDistance,
			// obstacleDistance = obstacleDistance,
			cohesionWeight = cohesionWeight,
			avoidanceWeight = avoidanceWeight,
			aligementWeight = alignmentWeight,
			boundsWeight = boundWeight,
			// obstacleWeight = obstacleWeight,
			fovAngle = flockUnitPrefab.FOVAngle,
			minSpeed = minSpeed,
			maxSpeed = maxSpeed,
			smoothDamp = flockUnitPrefab.smoothDamp,
			flockPosition = transform.position,
			deltaTime = Time.deltaTime,
			ObstacleVectors = obstacleVectors,
		};

		JobHandle handle = moveJob.Schedule(allUnits.Length, 4);
		handle.Complete();
		for (int i = 0; i < allUnits.Length; i++)
		{
			allUnits[i].Transform.forward = unitForwardDirections[i];
			allUnits[i].Transform.position = unitPositions[i];
			allUnits[i].CurrentVelocity = unitCurrentVelocities[i];
			allUnits[i].speed = allUnitsSpeeds[i];
		}

		unitForwardDirections.Dispose();
		unitCurrentVelocities.Dispose();
		unitPositions.Dispose();
		cohesionNeighbours.Dispose();
		avoidanceNeighbours.Dispose();
		aligementNeighbours.Dispose();
		aligementNeighboursDirecions.Dispose();
		allUnitsSpeeds.Dispose();
		neighbourSpeeds.Dispose();
		obstacleVectors.Dispose();
	}

	private void GenerateUnits()
	{
		allUnits = new FlockUnit_Jobs[flockSize];
		for (int i = 0; i < flockSize; i++)
		{
			var randomVector = Random.insideUnitSphere;
			randomVector = new Vector3(randomVector.x * spawnBounds.x, randomVector.y * spawnBounds.y, randomVector.z * spawnBounds.z);
			var spawnPosition = transform.position + randomVector;
			var rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
			allUnits[i] = Instantiate(flockUnitPrefab, spawnPosition, rotation, flockParent)
				.AssignFlock(this)
				.InitializeSpeed(Random.Range(minSpeed, maxSpeed));
		}
	}
	
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, boundDistance);
		Gizmos.color = Color.green;
		Gizmos.DrawWireCube(transform.position, spawnBounds);
	}
}
