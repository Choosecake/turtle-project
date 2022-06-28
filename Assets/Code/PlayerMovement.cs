using System;
using Code;
using UnityEngine;
using UnityEngine.Serialization;

[SelectionBase]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Transform turtleModel;
    [Min(0)][SerializeField] private float maxSpeed = 7.5f;
    [Min(0)][SerializeField] private float accelerationRate = 7.5f;
    [Range(0, 01f)] [SerializeField] private float relativeDrag = 0.1f;
    [Min(0)][SerializeField] private float rotationSpeed = 300.0f;
    
    private Vector3 movementDirection;

    private Rigidbody _rb;
    private TurtleVitalSystems _vitalsSystems;
    
    public float MaxSpeed
    {
        get => maxSpeed;
        set => maxSpeed = value;
    }

    public float RotationSpeed
    {
        get => rotationSpeed;
        set => rotationSpeed = value;
    }

    private void Awake()
    {
        _vitalsSystems = GetComponent<TurtleVitalSystems>();
        _rb = GetComponent<Rigidbody>();
        _rb.maxDepenetrationVelocity = 1;
        _rb.drag = accelerationRate * relativeDrag;
    }

    private void Update()
    {
        if (_vitalsSystems.IsDead) return;
        
        float vertical = Input.GetAxisRaw("Vertical");
        float horizontal = Input.GetAxisRaw("Horizontal");

        movementDirection = new Vector3(horizontal, 0f, vertical);
        movementDirection.Normalize();
        
        transform.rotation = Quaternion.Lerp(transform.rotation, target.rotation, 5.0f * Time.deltaTime);

        turtleModel.transform.Rotate(0, 0, -horizontal * 300 * Time.deltaTime, Space.Self);

        if (movementDirection == Vector3.zero) return;
        // character rotation
        Quaternion toRotation = Quaternion.LookRotation(movementDirection, Vector3.up) * target.rotation;
        turtleModel.rotation = Quaternion.RotateTowards(turtleModel.rotation, toRotation, rotationSpeed * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        // _rb.velocity = transform.TransformDirection(movementDirection) * maxSpeed;
        var resultingForce = transform.TransformDirection(movementDirection) * accelerationRate;
        _rb.AddForce(resultingForce) ;
        if (_rb.velocity.sqrMagnitude > maxSpeed * maxSpeed)
        {
            _rb.velocity = _rb.velocity.normalized * maxSpeed;
        }
    }
    
}