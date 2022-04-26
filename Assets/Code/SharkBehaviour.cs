using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SharkBehaviour : MonoBehaviour
{
    public bool isHunting;
    
    [SerializeField] private float movementSpeed;
    private GameObject turtle;
    private Collider turtleCollider;
    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.position;
        isHunting = true;
        
        // nojo nojento fix this
        turtle = GameObject.Find("PlayerTurtle");
        turtleCollider = turtle.GetComponent<Collider>();
    }

    private void Update()
    {
        if (isHunting)
        {
            MoveShark(turtle.transform.position);
        }
        else
        {
            StartCoroutine(SharkGoBack());
        }
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

    public Vector3 MoveShark(Vector3 direction)
    {
        return transform.position = Vector3.Lerp(transform.position, direction, movementSpeed * Time.deltaTime);
    }

    private IEnumerator SharkGoBack()
    {
        MoveShark(startPosition);
        yield return new WaitForSeconds(3f);
        Destroy(this.gameObject);
    }
}
