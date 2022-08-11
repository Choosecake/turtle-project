using UnityEngine;

public class ThirdPersonCameraController : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private Transform target;
    [Range(100.0f, 1000.0f)]
    [SerializeField] private float Sensitivity = 300.0f;
    [SerializeField] private float sphereRadius;
    [SerializeField] private LayerMask collisionMask;
    private Vector3 rayDirection, cameraOffset;
    private float _mouseX, _mouseY, distance;

    private void Awake()
    {
	Cursor.visible = true;
	//Cursor.lockState
        if (PlayerPrefs.HasKey("Sensitivity"))
        {
            Sensitivity = PlayerPrefs.GetFloat("Sensitivity");
        }
        
        cameraOffset = transform.localPosition;
    }

    private void LateUpdate()
    {
        _mouseX += Input.GetAxis("Mouse X") * Sensitivity * Time.deltaTime;
        _mouseY -= Input.GetAxis("Mouse Y") * Sensitivity * Time.deltaTime;
        
        // locks y camera rotation to limited range
        _mouseY = Mathf.Clamp(_mouseY, -35, 60);
        
        transform.LookAt(target);
        target.rotation = Quaternion.Euler(_mouseY, _mouseX, 0);

        // camera collision
        rayDirection = (gameObject.transform.position - player.transform.position).normalized;
        Ray ray = new Ray(player.transform.position, rayDirection);
        RaycastHit hit;
        LayerMask layer = LayerMask.NameToLayer("CameraBound");
        if (Physics.SphereCast(ray, sphereRadius, out hit, 7.5f, collisionMask))
        {
            distance = Mathf.Clamp(hit.distance, -5.0f, 7.5f);
        }
        else
        {
            distance = 7.5f;
        }
        
        transform.localPosition = Vector3.Lerp(transform.localPosition, transform.localPosition.normalized * (distance - 0.5f), Time.deltaTime * 100);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(gameObject.transform.position, sphereRadius);
    }
}
