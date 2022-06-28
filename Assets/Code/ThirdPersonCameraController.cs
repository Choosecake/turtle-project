using System;
using UnityEngine;
using static Code.Menu.GameAttributeStrings;

public class ThirdPersonCameraController : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private Transform target;
    [Range(100.0f, 1000.0f)]
    // [SerializeField] private float Sensitivity = 300.0f;
    [SerializeField] private float sphereRadius;
    [SerializeField] private LayerMask collisionMask;
    private Vector3 rayDirection, cameraOffset;
    private float _mouseX, _mouseY, distance;

    private float Sensitivity => PlayerPrefs.GetFloat(SensitivityString);
    
    private void Awake()
    {
        cameraOffset = transform.localPosition;
    }

    private void Update()
    {
        _mouseX += Input.GetAxis("Mouse X") * Sensitivity * Time.deltaTime;
        _mouseY -= Input.GetAxis("Mouse Y") * Sensitivity * Time.deltaTime;
    }

    private void LateUpdate()
    {
        // locks y camera rotation to limited range
        _mouseY = Mathf.Clamp(_mouseY, -35, 60);
        
        target.rotation = Quaternion.Euler(_mouseY, _mouseX, 0);
        transform.LookAt(target);



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
