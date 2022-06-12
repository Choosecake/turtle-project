using System;
using System.Collections;
using System.Collections.Generic;
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

        [Header("Speed Setup")] 
        [Range(0,10)] [SerializeField] private float minSpeed;
        [Range(0,10)] [SerializeField] private float maxSpeed;

        [Header("Detection Base")] 
        [Range(0,10)][SerializeField] private float cohesionDistance;
        [Range(0,10)][SerializeField] private float avoidanceDistance;
        [Range(0,10)][SerializeField] private float alignmentDistance;
        
        [Header("Behaviour Weights")] 
        [Range(0,10)][SerializeField] private float cohesionWeight;
        [Range(0,10)][SerializeField] private float avoidanceWeight;
        [Range(0,10)][SerializeField] private float alignmentWeight;
        
        

        public float CohesionDistance => cohesionDistance;
        public float AvoidanceDistance => avoidanceDistance;
        public float AlignmentDistance => alignmentDistance;

        public float CohesionWeight => cohesionWeight;
        public float AvoidanceWeight => avoidanceWeight;
        public float AlignmentWeight => alignmentWeight;

        public float MinSpeed => minSpeed;
        public float MaxSpeed => maxSpeed;
        public FlockUnit[] allUnits { get; set; }

        private void Start()
        {
            GenerateUnits();
        }

        private void Update()
        {
            for (int i = 0; i < allUnits.Length; i++)
            {
                allUnits[i].MoveUnit();
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
                allUnits[i] = Instantiate(flockUnityPrefab, spawnPosition, rotation)
                    .AssignFlock(this).
                    InitializeSpeed(Random.Range(minSpeed, maxSpeed));
            }
        }
    }


}
