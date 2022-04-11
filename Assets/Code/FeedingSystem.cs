using System;
using System.Collections;
using Code;
using UnityEngine;

[RequireComponent(typeof(Nutrition))]
public class FeedingSystem : MonoBehaviour
{
    [Header("Detection Sphere")]
    [SerializeField] private float nucleusDistance;
    [SerializeField] private float sphereRadius;

    [Header("Food Detection Details")] 
    [SerializeField] private LayerMask foodLayer;
    [SerializeField] private GameObject eatInput_UI;
    [Min(0)] [SerializeField] private float detectionInterval = 0.2f;
    
    /// <summary>
    /// TEMP!
    /// Esse valor deve ser decidio por um script presente na comida, e não na tartaruga
    /// </summary>
    [Space(11)]
    [Range(0,1)][SerializeField] float recoveryValue = 0.25f;
    
    private Nutrition _nutrition;
    private Collider[] _detectedFood;
    private Vector3 _spherePosition;

    private void Awake()
    {
        _nutrition = GetComponent<Nutrition>();
        StartCoroutine(CheckForFood());
    }

    private void OnValidate()
    {
        _detectedFood = new Collider[1];
    }

    private void Update()
    {
        if (_detectedFood[0] != null && Input.GetKeyDown(KeyCode.E))
        {
            var signal = _detectedFood[0].tag.Contains("GoodFood") ? 1 : -1;
            _nutrition.RecoverNutrition(recoveryValue * signal);
            Destroy(_detectedFood[0].gameObject);
        }
    }

    private bool IsFoodNearby()
    {
        _spherePosition = transform.position;
        return Physics.OverlapSphereNonAlloc(_spherePosition, sphereRadius, _detectedFood, foodLayer) > 0;
    }

    private IEnumerator CheckForFood()
    {
        while (true)
        {
            eatInput_UI.SetActive(IsFoodNearby());
            yield return new WaitForSeconds(detectionInterval);
        }        
    }

    #if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = IsFoodNearby() ? Color.red : Color.green;
        Gizmos.DrawWireSphere(_spherePosition, sphereRadius);
    }
    #endif
}