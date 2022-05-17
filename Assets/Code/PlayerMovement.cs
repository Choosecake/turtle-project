using System;
using Code;
using UnityEngine;

[SelectionBase]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Transform turtleModel;

    [Header(" TESTING ")]
    [Range(-10, 100)] [SerializeField] private float depenetrationThreshold; 
    private Vector3 movementDirection;
    private float movementSpeed = 5.0f;
    private float rotationSpeed = 300.0f;
    private Rigidbody _rb;
    private TurtleVitalSystems _vitalsSystems;

    private void Awake()
    {
        _vitalsSystems = GetComponent<TurtleVitalSystems>();
        _rb = GetComponent<Rigidbody>();
        // _rb.maxDepenetrationVelocity = depenetrationThreshold;
    }

    private void Update()
    {

        PhysicsTesting();
        
        
        if (_vitalsSystems.IsDead) return;
        
        float vertical = Input.GetAxisRaw("Vertical");
        float horizontal = Input.GetAxisRaw("Horizontal");

        movementDirection = new Vector3(horizontal, 0f, vertical);
        movementDirection.Normalize();
        
        transform.Translate(movementDirection * movementSpeed * Time.deltaTime, Space.Self);
        transform.rotation = Quaternion.Lerp(transform.rotation, target.rotation, 5.0f * Time.deltaTime);

        if (movementDirection == Vector3.zero) return;
        // character rotation
        Quaternion toRotation = Quaternion.LookRotation(movementDirection, Vector3.up) * target.rotation;
        turtleModel.rotation = Quaternion.RotateTowards(turtleModel.rotation, toRotation, rotationSpeed * Time.deltaTime);
    }

    private void PhysicsTesting()
    {
        if (Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            Debug.Log("Depentration Velocity updated to" + ++_rb.maxDepenetrationVelocity ) ;
        }
        
        if (Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            Debug.Log("Depentration Velocity updated to" + --_rb.maxDepenetrationVelocity ) ;
        }
            //TESTING
            if (Input.GetKey(KeyCode.LeftControl))
            {
                if (Input.GetKeyDown(KeyCode.C))
                {
                    _rb.maxDepenetrationVelocity = depenetrationThreshold;
                    Debug.Log("Depentration Velocity updated to" + _rb.maxDepenetrationVelocity ) ;
                }

                if (Input.GetKeyDown(KeyCode.G))
                {
                    Debug.Log(" Depenetration Velocity IS: " + _rb.maxDepenetrationVelocity);
                }
            }
        
        //TESTING
    }
    
}