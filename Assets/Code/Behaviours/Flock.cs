using System;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Behaviours
{
    public class Flock : MonoBehaviour
    {
        [Header("Spawn Setup")]
        [SerializeField] private FlockUnit flockUnityPrefab;
        [SerializeField] private int flockSize;
        [SerializeField] private Vector3 spawnBounds;
        [SerializeField] private Transform flockParent;

        [Header("Speed Setup")] 
        [Range(0,10)] [SerializeField] private float minSpeed;
        [Range(0,10)] [SerializeField] private float maxSpeed;

        [Header("Detection Base")] 
        [Range(0,10)][SerializeField] private float cohesionDistance;
        [Range(0,10)][SerializeField] private float avoidanceDistance;
        [Range(0,10)][SerializeField] private float alignmentDistance;
        [Range(0, 100)] [SerializeField] private float boundDistance;
        [Range(0, 10)] [SerializeField] private float obstacleDistance;
        [SerializeField] private Transform goal;
        [Range(0, 10)] [SerializeField] private float goalDistance;
        [Range(0, 10)] [SerializeField] private float fleeDistance;
        
        [Header("Behaviour Weights")] 
        [Range(0,10)][SerializeField] private float cohesionWeight;
        [Range(0,10)][SerializeField] private float avoidanceWeight;
        [Range(0,10)][SerializeField] private float alignmentWeight;
        [Range(0, 10)] [SerializeField] private float boundWeight;
        [Range(0, 100)] [SerializeField] private float obstacleWeight;
        [Range(0, 10)] [SerializeField] private float goalWeight;
        [Range(0, 10)] [SerializeField] private float fleeWeight;


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
        public FlockUnit[] allUnits { get; private set; }

        private void Start()
        {
            GenerateUnits();
        }

        private void Update()
        {
            foreach (var unit in allUnits)
            {
                unit.MoveUnit();
            }
        }

        private void GenerateUnits()
        {
            allUnits = new FlockUnit[flockSize];
            for (int i = 0; i < flockSize; i++)
            {
                var randomVector = Random.insideUnitSphere;
                randomVector = new Vector3(randomVector.x * spawnBounds.x, randomVector.y * spawnBounds.y,
                    randomVector.z * spawnBounds.z);
                var spawnPosition = transform.position + randomVector;
                var rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
                allUnits[i] = Instantiate(flockUnityPrefab, spawnPosition, rotation, flockParent)
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


}
