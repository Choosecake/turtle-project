using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SharkBehaviour : MonoBehaviour
{
    private GameObject turtle;
    private Collider turtleCollider;
    [SerializeField] private float movementSpeed;

    private void Start()
    {
        // nojo nojento fix this
        turtle = GameObject.Find("PlayerTurtle");
        turtleCollider = turtle.GetComponent<Collider>();
    }

    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, turtle.transform.position, movementSpeed * Time.deltaTime);
        transform.LookAt(turtle.transform);
    }

    // criar classe separada
    private void OnTriggerEnter(Collider other)
    {
        if (other == turtleCollider)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
