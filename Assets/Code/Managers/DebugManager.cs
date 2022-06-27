using System;
using UnityEngine;
using Utilities;

public class DebugManager : MonoBehaviour
{
    [SerializeField] private bool enableDebugging;

    [Header("References")] 
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private float movementMultiplier;

    private void Awake()
    {
        if (!enableDebugging)
        {
            this.enabled = false;
            return;
        }

        playerMovement.MovementSpeed *= movementMultiplier;
        playerMovement.RotationSpeed *= movementMultiplier;
    }
}