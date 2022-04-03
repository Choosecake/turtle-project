using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCameraController : MonoBehaviour
{
    [SerializeField] private Transform target, player;
    private PlayerMovement _playerMovement;
    private const float RotationSpeed = 100.0f;
    private const float RotationSmoothness = 3.0f;
    private float _mouseX, _mouseY;

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        _playerMovement = GetComponentInParent<PlayerMovement>();
    }

    private void LateUpdate()
    {
        _mouseX += Input.GetAxis("Mouse X") * RotationSpeed * Time.deltaTime;
        _mouseY -= Input.GetAxis("Mouse Y") * RotationSpeed * Time.deltaTime;
        
        // locks y camera rotation to limited range
        _mouseY = Mathf.Clamp(_mouseY, -35, 60);
        
        transform.LookAt(target);
        target.rotation = Quaternion.Euler(_mouseY, _mouseX, 0);

        // smooth player rotation towards where the camera is pointing while player is walking
        if (_playerMovement.playerMovement == Vector3.zero) return;
        player.rotation = Quaternion.Lerp(player.rotation, target.rotation, RotationSmoothness * Time.deltaTime);
    }
}
