using UnityEngine;

public class ThirdPersonCameraController : MonoBehaviour
{
    [SerializeField] private Transform target;
    [Range(100.0f, 1000.0f)]
    [SerializeField] private float Sensitivity = 300.0f;
    private float _mouseX, _mouseY;

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void LateUpdate()
    {
        _mouseX += Input.GetAxis("Mouse X") * Sensitivity * Time.deltaTime;
        _mouseY -= Input.GetAxis("Mouse Y") * Sensitivity * Time.deltaTime;
        
        // locks y camera rotation to limited range
        _mouseY = Mathf.Clamp(_mouseY, -35, 60);
        
        transform.LookAt(target);
        target.rotation = Quaternion.Euler(_mouseY, _mouseX, 0);
    }
}
