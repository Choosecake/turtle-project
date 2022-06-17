using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace  Behaviours
{
    public class FlockUnit_Jobs : MonoBehaviour
    {
        [SerializeField] private float FOVangle;
        [SerializeField] private float smoothDamp;

        
        public float Speed { get; set; }
        public Vector3 CurrentVelocity { get; set; }
        public Transform Transform { get; set; }
        public float FOVAngle => FOVangle;
        public float SmoothDamp => smoothDamp;

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
            this.Speed = speed;
            return this;
        }
        
    }
}