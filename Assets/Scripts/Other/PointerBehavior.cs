using System;
using Unity.Mathematics;
using UnityEngine;

public class PointerBehavior : MonoBehaviour
{
    
    [SerializeField] private AudioClip pointerHit;
    [SerializeField] private AnimationCurve speedCurve;
    private AudioSource audioSource;
    private Animator _animator;
    private float curveTime;
    private bool evaluate;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        _animator = GetComponent<Animator>();
    }
    

    /// <summary>
    /// Used as animation event handler
    /// </summary>
    public void Hithandler()
    {
        if (pointerHit && audioSource && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    private void Update()
    {
        if (evaluate)
        {
            _animator.speed = speedCurve.Evaluate(curveTime);
            curveTime += Time.deltaTime/4;
        }
    }

    public void Animate(bool val)
    {
        _animator.enabled = val;
        evaluate = val;

        if (!val)
            curveTime = 0;
    }
}