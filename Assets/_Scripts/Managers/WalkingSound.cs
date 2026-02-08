using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


public class WalkingSound : Singleton<WalkingSound> // Исправил опечатку в имени
{
    [Header("Audio Source")]
    public AudioSource audioSource;  // Прикрепи в инспекторе!

    public float walkingSpeed = 2f;
    private List<AudioClip> walking = new List<AudioClip>();
    private List<AudioClip> swimming = new List<AudioClip>();

    private Coroutine loopCoroutine;
    private WalkingSoundType walkingSoundType;

    void Start()
    {
        AudioClip[] clips = Resources.LoadAll<AudioClip>("Music/walking");
        walking.AddRange(clips);
        
        AudioClip[] clips2 = Resources.LoadAll<AudioClip>("Music/swimming");
        swimming.AddRange(clips2);
        
        if (walking.Count > 0) { loopCoroutine = StartCoroutine(WalkingLoop()); }

        EventManager.onChangeWalkingPlace += StartLoop;
    }

    IEnumerator WalkingLoop()
    {
        while (true)  
        {
            AudioClip clip = walking[Random.Range(0, walking.Count)];
            audioSource.PlayOneShot(clip, Random.Range(0.4f, 0.8f));
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            yield return new WaitForSeconds(clip.length);
        }
    }
    IEnumerator SwimmingLoop()
    {
        while (true)  
        {
            AudioClip clip = swimming[Random.Range(0, swimming.Count)];
            audioSource.PlayOneShot(clip, Random.Range(0.4f, 0.8f));
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            yield return new WaitForSeconds(clip.length);
        }
    }

    public void Stop()
    {
        if (loopCoroutine != null)
        {
            StopCoroutine(loopCoroutine);
            loopCoroutine = null;
        }

        audioSource.Stop();
    }

    public void StartLoop(WalkingSoundType  newWalkingSoundType)
    {
        Debug.Log("new walking loop");

        walkingSoundType = newWalkingSoundType;
        if (walkingSoundType == WalkingSoundType.walking)
        {
            if ( walking.Count > 0)
            {

                loopCoroutine = StartCoroutine(WalkingLoop());
            }
        }
        else if (walkingSoundType == WalkingSoundType.swimming)
        {
            Debug.Log("Swimming");
            if (swimming.Count > 0) {

                loopCoroutine = StartCoroutine(SwimmingLoop());
            }
        } else if (walkingSoundType == WalkingSoundType.none)
        {
            Stop();
        }
    }

    void OnDestroy()
    {
        Stop();  // На всякий случай
    }

    private void Update()
    {
        if (loopCoroutine == null)
        { if (ThirdPersonController.Instance.isMoving()) { StartLoop(walkingSoundType) ;} } 
        else if (!ThirdPersonController.Instance.isMoving()) {Stop();}
    }
}

[System.Serializable]
public enum WalkingSoundType
{
    walking,
    swimming,
    none
}