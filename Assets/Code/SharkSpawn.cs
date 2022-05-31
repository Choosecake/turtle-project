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
    // private Collider turtleCollider;
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
        // if (boundary == null)
        // {
        //     boundary = GameObject.Find;
        // }
        // boundaryCollider = boundary.GetComponent<Collider>();
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
        if (other.tag == "Boundary" && !hasSharkSpawned)
        {
            StartCoroutine(PlayDangerSound());
            GameObject currentShark = Instantiate(shark, position, transform.rotation);
            sharkBehaviour = currentShark.GetComponent<SharkBehaviour>();
            sharkBehaviour.SetTargetAttributes(transform);
            // sharkBehaviour.target = gameObject;
            // sharkBehaviour.TurtleCollider = gameObject.GetComponent<Collider>();
            hasSharkSpawned = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Boundary" && hasSharkSpawned)
        {
            sharkBehaviour.isHunting = false;
            hasSharkSpawned = false;
        }
    }

    private IEnumerator PlayDangerSound()
    {
        if (musicPlayer == null)
        {
            musicPlayer = GameObject.Find("MusicPlayer").GetComponent<AudioSource>();
            if (musicPlayer == null)
            {
                Debug.LogWarning("No valid audioSource has been found!");
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
        // TurtleVitalSystems turtleVitalSystems = sharkBehaviour.TargetPrey.GetComponent<TurtleVitalSystems>();
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