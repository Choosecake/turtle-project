using UnityEngine;

public class ShipBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject[] paths;
    [SerializeField] private float moveSpeed = 1.0f, rotationSpeed = 1.0f;
    private Vector3 newDirection;
    private float minDistance = 1.5f;
    private int currentPath;
    private bool isRotating;
    
    private void Start()
    {
        currentPath = 0;
    }

    private void Update()
    {
        Vector3 targetDirection = paths[currentPath].transform.position - transform.position;

        newDirection = Vector3.RotateTowards(transform.forward, targetDirection, rotationSpeed * Time.deltaTime, 0.0f);
        transform.rotation = Quaternion.LookRotation(newDirection);

        if (Vector3.Distance(transform.position, paths[currentPath].transform.position) < 3)
        {
            if (currentPath < paths.Length - 1)
            {
                currentPath++;
            }
            else
            {
                currentPath = 0;
            }
        }

        transform.position = Vector3.MoveTowards(transform.position, paths[currentPath].transform.position, moveSpeed * Time.deltaTime);
    }
}
