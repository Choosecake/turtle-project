using System;
using Behaviours;
using UnityEngine;

public class SteeringAgent : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private BehaviourEnum movementBehaviour;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float maxAccel;

    [SerializeField][ReadOnly] private Vector3 _currentVelocity = Vector3.zero;
    [SerializeField][ReadOnly] private Vector3 _steeringVelocity = Vector3.zero;

    private void Awake()
    {
        _steeringVelocity = _currentVelocity = Vector3.zero;
    }

    private void FixedUpdate()
    {
        switch (movementBehaviour)
        {
            case BehaviourEnum.Seek:
                Seek(target);
                break;
            case BehaviourEnum.Flee:
                break;
            case BehaviourEnum.Arrive:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        Accelerate(_steeringVelocity);
        Direct();
    }

    private void Seek(Transform target)
    {
        var desiredVelocity = target.position - transform.position;
        desiredVelocity = desiredVelocity.normalized * maxSpeed;
        _steeringVelocity = desiredVelocity - _currentVelocity;
        _steeringVelocity = _steeringVelocity.normalized * maxAccel;
    }
    
    private void Flee(Transform target)
    {
        var desiredVelocity = target.position - transform.position;
        desiredVelocity = desiredVelocity.normalized * maxSpeed;
        _steeringVelocity = desiredVelocity - _currentVelocity;
        _steeringVelocity = _steeringVelocity.normalized * maxAccel;
    }

    private void Accelerate(Vector3 acceleration)
    {
        _currentVelocity += acceleration * Time.fixedDeltaTime;
        if (_currentVelocity.sqrMagnitude > maxSpeed * maxSpeed)
        {
            _currentVelocity = _currentVelocity.normalized * maxSpeed;
        }

        transform.position += _currentVelocity;
    }

    private void Direct()
    {
        transform.forward = _currentVelocity.normalized;
    }
}
