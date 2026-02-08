using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WeatherAudioManager : Singleton<WeatherAudioManager> // Исправил опечатку в имени
{
    [Header("Audio Source")]
    public AudioSource audioSource;  // Прикрепи в инспекторе!

    [Header("Wind Clips")]
    public List<AudioClip> wind = new List<AudioClip>();

    private Coroutine windLoopCoroutine;

    void Awake()
    {
        AudioClip[] clips = Resources.LoadAll<AudioClip>("Music/Wind");
        EventManager.playWInd += ResumeWind;
        EventManager.stopWind += StopWind;
        wind.AddRange(clips);
    }


    IEnumerator WindLoop()
    {
        while (true)  
        {
            AudioClip clip = wind[Random.Range(0, wind.Count)];
            audioSource.PlayOneShot(clip, Random.Range(0.4f, 0.8f));
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            
            
            yield return new WaitForSeconds(clip.length-1);
        }
    }

    public void StopWind()
    {
        if (windLoopCoroutine != null)
        {
            StopCoroutine(windLoopCoroutine);
            windLoopCoroutine = null;
        }
    }

    // Если нужно временно приостановить и потом возобновить
    public void ResumeWind()
    {
        if (windLoopCoroutine == null && wind.Count > 0)
        {
            windLoopCoroutine = StartCoroutine(WindLoop());
        }
    }

    void OnDestroy()
    {
        StopWind();  // На всякий случай
    }
}