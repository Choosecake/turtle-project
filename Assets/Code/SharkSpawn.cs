using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Code;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class SharkSpawn : MonoBehaviour
{
    [SerializeField] private GameObject boundary;
    [SerializeField] private GameObject shark;
    [SerializeField] private TurtleVitalSystems turtleVitalSystems;
    private Collider boundaryCollider;
    private Vector3 position;
    private SharkBehaviour sharkBehaviour;
    private float sharkDistance = 250.0f;
    private bool hasSharkSpawned = false;
    [Header("Sounds")] 
    [SerializeField] private AudioClip initialDangerSound;
    [SerializeField] private AudioClip middleDangerSound;
    [SerializeField] private AudioClip endDangerSound;
    [SerializeField] private AudioSource musicPlayer;

    private void Start()
    {
        boundary = GameplayManager.Instance.SharkBoundaryGameObject;
        if (boundary != null)
        {
            boundaryCollider = boundary.GetComponent<Collider>();
        }
        else
        {
            this.enabled = false;
        }
    }

    private void Update()
    {
        Vector3 direction = transform.position - boundary.transform.position;
        float angle = Vector3.Angle(direction, boundary.transform.forward);

        if (transform.position.x < 0)
        {
            position = Quaternion.Euler(0, -angle, 0) * Vector3.forward * sharkDistance;
        }
        else
        {
            position = Quaternion.Euler(0, angle, 0) * Vector3.forward * sharkDistance;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Boundary") && !hasSharkSpawned)
        {
            StartCoroutine(PlayDangerSound());
            GameObject currentShark = Instantiate(shark, position, transform.rotation);
            sharkBehaviour = currentShark.GetComponent<SharkBehaviour>();
            sharkBehaviour.SetTargetAttributes(transform);
            hasSharkSpawned = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Boundary") && hasSharkSpawned)
        {
            sharkBehaviour.isHunting = false;
            hasSharkSpawned = false;
        }
    }

    private IEnumerator PlayDangerSound()
    {
        if (musicPlayer == null)
        {
            var musicPlayerGO = GameplayManager.Instance.MusicPlayerGameObject;
            if (musicPlayerGO == null)
            {
                Debug.LogWarning("No valid audioSource  Game Object  has been found!");
                yield break;
            }
            else if (!musicPlayerGO.TryGetComponent(out musicPlayer))
            {               
                Debug.LogWarning("No valid audioSource  Component  has been found!");
                yield break;
            }
        }
        
        
        
        AudioClip originalTrack = musicPlayer.clip;
        musicPlayer.loop = false;
        musicPlayer.clip = initialDangerSound;
        musicPlayer.Play();
        yield return new WaitWhile(() => musicPlayer.isPlaying);
        musicPlayer.loop = true;
        musicPlayer.clip = middleDangerSound;
        musicPlayer.Play();
        yield return new WaitWhile(() => sharkBehaviour.isHunting && !turtleVitalSystems.IsDead);
        if (turtleVitalSystems.IsDead)
        {
            musicPlayer.loop = false;
            yield break;
        }
        musicPlayer.loop = false;
        musicPlayer.clip = endDangerSound;
        musicPlayer.Play();
        yield return new WaitWhile(() => musicPlayer.isPlaying);
        musicPlayer.loop = true;
        musicPlayer.clip = originalTrack;
        musicPlayer.Play();
        
    }
}