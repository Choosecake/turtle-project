using UnityEngine;

[SelectionBase]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Transform turtleModel;
    private Vector3 movementDirection;
    private float movementSpeed = 5.0f;
    private float rotationSpeed = 300.0f;

    private void Update()
    {
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
}