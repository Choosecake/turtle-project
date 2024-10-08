using System;
using Code;
using UnityEngine;
using Utilities;

public class DebugManager : MonoBehaviour
{
    [SerializeField] private bool enableDebugging;

    [Header("References")] 
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private float movementMultiplier;
    [SerializeField] private Nutrition nutrition;
    [SerializeField] private Breath breath;

    private void Awake()
    {
        if (!enableDebugging)
        {
            this.enabled = false;
            return;
        }

        playerMovement.MovementSpeed *= movementMultiplier;
        playerMovement.RotationSpeed *= movementMultiplier;

        nutrition.enabled = false;
        breath.enabled = false;
    }
}