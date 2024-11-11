using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuHeroModelBehaviour : MonoBehaviour
{
    [SerializeField]
    private Transform ModelTransform;
    [SerializeField]
    private List<AnimationClip> idle_clips;

    [SerializeField]
    private List<AnimationClip> hello_clips;

    [SerializeField]
    private AnimationClip main_breathe_clip;

    [SerializeField, Range(5.0f, 60.0f)]
    private float MaxIdleDelay = 5.0f;
    [SerializeField, Range(5.0f, 20.0f)]
    private float MinIdleDelay = 5.0f;

    [SerializeField, Range(0.0f, 20.0f)]
    private float BeforeStartAnimationDelay = 0.0f;

    private float ElapsedTime = 0.0f;
    private float CurrentIdleDelay = 0.0f;

    [SerializeField]
    private Animator animator;


    [SerializeField]
    private Avatar avatar;

    [SerializeField] private AudioSource source;
    [SerializeField] private AudioClip breath;
    [SerializeField] private List<AudioClip> idles;
    [SerializeField] private List<AudioClip> helloes;
    [SerializeField] private AudioClip currentIdle;
    [SerializeField] private AudioClip currentHello;
    private bool startPlayed = false;

    private Quaternion DefaultRotation;
    private bool rotating = false;
    private AnimatorOverrideController aoc;

    internal void SetRotating(bool toggle)
    {
        rotating = toggle;
    }

    void Start()
    {
        DefaultRotation = transform.rotation;
        OverrideClips();
    }

    private void OverrideClips()
    {
        aoc = new AnimatorOverrideController(animator.runtimeAnimatorController);
        aoc["Breathe"] = main_breathe_clip;

        var rClip = UnityEngine.Random.Range(0, idle_clips.Count);
        aoc["Idle"] = idle_clips[rClip];
        if (idles.Count > 0)
            currentIdle = idles[rClip];

        var rHClip = UnityEngine.Random.Range(0, hello_clips.Count);
        aoc["Hello"] = hello_clips[rHClip];
        if (helloes.Count > 0)
        {
            currentHello = helloes[rHClip];
            source.clip = currentHello;
        }

        animator.avatar = avatar;
        animator.runtimeAnimatorController = aoc;
    }

    void ResetTimer()
    {
        ElapsedTime = 0.0f;
        CurrentIdleDelay = UnityEngine.Random.Range(MinIdleDelay, MaxIdleDelay);
    }


    void SetNewIdle()
    {
        ResetTimer();
        var rClip = UnityEngine.Random.Range(0, idle_clips.Count);
        aoc["Idle"] = idle_clips[rClip];
        if (idles.Count > 0)
        {
            currentIdle = idles[rClip];
            source.clip = currentIdle;
        }
    }

    void Update()
    {
        if (!rotating)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, DefaultRotation, 0.1f);
        }

        ElapsedTime += Time.deltaTime;
        if (startPlayed)
        {
            if (ElapsedTime > CurrentIdleDelay)
            {
                animator.SetTrigger("Idle");
            }
        }
        else
        {
            if (ElapsedTime > BeforeStartAnimationDelay)
            {
                animator.SetTrigger("Hello");
            }
        }
    }
    public void PlayHelloIdle()
    {
        animator.ResetTrigger("Hello");
        animator.ResetTrigger("Idle");
        animator.SetTrigger("Hello");
        if (currentHello)
            source.clip = currentHello;
        // PlayCustomClip(currentHello);
    }
    public void PlayIdle(float duration)
    {
        HelloPlayed();
        ResetTimer();
        CurrentIdleDelay = duration;
        animator.ResetTrigger("Hello");
        animator.ResetTrigger("Idle");
        animator.SetTrigger("Idle");
        if (currentHello)
            source.clip = currentHello;
    }

    /// <summary>
    /// Нужно вызвать в инвенте в начале каждой idle анимации
    /// </summary>
    public void ResetTriggers()
    {
        animator.ResetTrigger("Hello");
        animator.ResetTrigger("Idle");
        SetNewIdle();
    }

    internal void Rotate(float offset)
    {
        if (rotating)
        {
            ModelTransform.Rotate(Vector3.up, offset);
        }
    }

    /// <summary>
    /// Must be Called From any Hello animation
    /// </summary>
    public void HelloPlayed()
    {
        startPlayed = true;
    }

    internal void Enable(bool toggle)
    {
        gameObject.SetActive(toggle);
    }
    public void PlayCustomClip()
    {
        if (source)
            source.Play();
    }
}
