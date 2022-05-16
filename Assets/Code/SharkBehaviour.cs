using System;
using System.Collections;
using System.Collections.Generic;
using Code;
using Ez;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SharkBehaviour : MonoBehaviour
{
    public bool isHunting;
    
    [SerializeField] private float movementSpeed;
    private Transform targetPrey;
    private Collider preyCollider;
    private Vector3 startPosition;

    public Transform TargetPrey;

    private void Start()
    {
        startPosition = transform.position;
        isHunting = true;
        
        // nojo nojento fix this
        // turtle = GameObject.Find("PlayerTurtle");
        // turtleCollider = turtle.GetComponent<Collider>();
    }

    private void Update()
    {
        if (isHunting)
        {
            MoveShark(targetPrey.transform.position);
            transform.LookAt(targetPrey.transform);
        }
        else
        {
            StartCoroutine(SharkGoBack());
        }
    }

    // criar classe separada
    private void OnTriggerEnter(Collider other)
    {
        if (other == preyCollider)
        {
            other.gameObject.Send<TurtleVitalSystems>(_=>_.Die());
        }
    }

    public Vector3 MoveShark(Vector3 direction)
    {
        return transform.position = Vector3.Lerp(transform.position, direction, movementSpeed * Time.deltaTime);
    }

    private IEnumerator SharkGoBack()
    {
        MoveShark(startPosition);
        transform.LookAt(startPosition);
        yield return new WaitForSeconds(3f);
        Destroy(this.gameObject);
    }

    public void SetTargetAttributes(Transform target)
    {
        this.targetPrey = target;
        if (!targetPrey.TryGetComponent(out this.preyCollider))
        {
            Debug.LogWarning("No collider has been found on shark's target");
        }
    }
}
