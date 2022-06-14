using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class JellyfishAnimation : MonoBehaviour
{
    private Animator animator;
    
    private void Start()
    {
        animator = GetComponent<Animator>();
        int startTime = Random.Range(0, 2);
        StartCoroutine(StartAnimation(startTime));
    }

    private IEnumerator StartAnimation(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        animator.SetTrigger("Trigger"); 
    }
}
