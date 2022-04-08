using System;
using Code;
using UnityEngine;

[RequireComponent(typeof(Nutrition))]
public class FeedingSystem : MonoBehaviour
{
    [Header("Detection Sphere")]
    [SerializeField] private float nucleusDistance;
    [SerializeField] private float sphereRadius;

    [Header("Food Detection Details")] 
    [SerializeField] private LayerMask foodLayer;
    
    private Nutrition _nutrition;
    private Collider[] _detectedFood;
    private Vector3 _spherePosition;

    private void Awake()
    {
        _nutrition = GetComponent<Nutrition>();
        _detectedFood = new Collider[1];
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Debug.Log("transform.forward: " + transform.forward);
        }
    }

    // private void CheckForFood()
    // {
    //     _spherePosition = transform.position
    // }
    
    
}