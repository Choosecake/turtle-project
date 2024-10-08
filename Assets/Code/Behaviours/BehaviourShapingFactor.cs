using System;
using System.Collections.Generic;
using Behaviours;
using UnityEngine;

[Serializable]
public class BehaviourShapingFactor //NEW
{
    // public LayerMask mask;
    public Flock flock;
    [Range(0,100)] public float distance;
    [Range(0,10)] public float weight;

    public float SqrDistance => distance * distance;
}