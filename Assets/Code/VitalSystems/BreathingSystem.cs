using System;
using System.Collections;
using Code;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Breath))]
public class BreathingSystem : MonoBehaviour
{
    [Header("Detection Sphere")]
    [SerializeField] private float nucleusDistance;
    [SerializeField] private float sphereRadius;

    [Header("Food Detection Details")] 
    [SerializeField] private LayerMask surfaceLayer;
    [FormerlySerializedAs("breathInput_UI")] [SerializeField] private GameObject breathInstruction_UI;
    [Min(0)] [SerializeField] private float detectionInterval = 0.2f;
    
    /// <summary>
    /// TEMP!
    /// Esse valor deve ser decidio por um script presente na comida, e não na tartaruga
    /// </summary>
    [Space(11)]
    [Range(0,1)][SerializeField] float recoveryValue = 0.25f;
    
    private Breath _breath;
    private Collider[] _detectedSurface;
    private Vector3 _spherePosition;
    
    public Breath Breath => _breath;


    private void Awake()
    {
        _breath = GetComponent<Breath>();
        StartCoroutine(CheckForFood());
    }

    private void OnValidate()
    {
        _detectedSurface = new Collider[1];
    }

    private void Update()
    {
        // if (_detectedSurface[0] != null && Input.GetKeyDown(KeyCode.E))
        if (_detectedSurface.Length > 0 && Input.GetKeyDown(KeyCode.E))

        {
            //Talvez precise trocar esa verificação DE Tag pra LayerMask que é mais precisa e tem menos chance de dar merda
            var signal = _detectedSurface[0].tag.Contains("PollutedAir") ? -1 : 1;
            _breath.RecoverBreath(recoveryValue * signal);
        }
    }

    private bool isSurfaceNearby()
    {
        _spherePosition = transform.position;
        _detectedSurface = Physics.OverlapSphere(_spherePosition, sphereRadius, surfaceLayer);
        return _detectedSurface.Length > 0;
        // return Physics.OverlapSphereNonAlloc(_spherePosition, sphereRadius, _detectedSurface, surfaceLayer) > 0;
    }

    private IEnumerator CheckForFood()
    {
        while (true)
        {
            breathInstruction_UI.SetActive(isSurfaceNearby());
            yield return new WaitForSeconds(detectionInterval);
        }        
    }

    #if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = isSurfaceNearby() ? Color.red : Color.green;
        Gizmos.DrawWireSphere(_spherePosition, sphereRadius);
    }
    #endif
}