using UnityEngine;

/// <summary>
/// Insanely basic audio system which supports 3D sound.
/// Ensure you change the 'Sounds' audio source to use 3D spatial blend if you intend to use 3D sounds.
/// </summary>
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AudioSystem : StaticInstance<AudioSystem> {
    [SerializeField] private AudioSource _musicSourceA;
    [SerializeField] private AudioSource _musicSourceB;
    [SerializeField] private AudioSource _soundsSource;
    public AudioClip collectItem;
    public AudioClip grab;
    public AudioClip shotgunSound;  // Assign or leave empty to load from Resources/Music/dennish18-shotgun-146188
    public AudioClip fonkMusic;
    public AudioClip BreatheSound;
    public AudioClip heartBeatSound;
    private AudioSource _currentMusicSource;
    private AudioSource _previousMusicSource;

    
    private void Awake() {
        // Base Awake from StaticInstance handles singleton
        base.Awake();

        _currentMusicSource = _musicSourceA;
        _previousMusicSource = _musicSourceB;
        _previousMusicSource.volume = 0f; // Start muted
    }

[Header("Audio Clips (add these if missing)")]
    public AudioClip gaspSound;     // Desperate air gasp
    public AudioClip bubbleSound;   // Underwater bubbles
    public AudioClip finalGasp;     // Epic final breath
    public Image deathOverlay;
    
    [Header("Visuals (assign in inspector)")]
    public CanvasGroup oxygenUI;    // Your oxygen bar (if any)

    [Header("Dying Settings")]
    public float totalDyingTime = 15f;
    public AnimationCurve panicCurve = AnimationCurve.EaseInOut(0, 0, 1, 1); // Ramp up

    // Enhanced dying sequence
    public IEnumerator StartAlmostToDie()
    {
        // Fade in tense music
        StartCoroutine(FadeAudioSourceMusic(0f, 1f, 2f)); // Custom fade (see below)
        SwimmingCameraWobble.Instance.SetSwimming(true);

        yield return StartDyingSequence(totalDyingTime);

        // Final death throes
        StartCoroutine(FinalDeathShakeAndGasp());

        yield return new WaitForSeconds(1.5f);

        SwimmingCameraWobble.Instance.SetSwimming(false);
        
        Color c= deathOverlay.color;
        c.a = 0;
        deathOverlay.color = c;
        
        GameManager.Instance.ChangeState(GameState.reburn);
    }

    IEnumerator StartDyingSequence(float duration)
    {
        float elapsed = 0f;
        float heartBeatInterval = 0.8f; // Starts slow
        float nextHeartBeat = 0f;
        float nextBreathe = 0f;
        float nextGasp = 2f;
        float nextBubble = 1.5f;

        while (elapsed < duration)
        {
            float progress = elapsed / duration;
            float panicIntensity = panicCurve.Evaluate(progress); // 0->1 smooth ramp

            // Heartbeat: faster + louder + higher pitch
            if (Time.time >= nextHeartBeat)
            {
                float heartPitch = Mathf.Lerp(0.7f, 2.2f, panicIntensity);
                float heartVol = 0.6f + panicIntensity * 0.8f;
                PlaySound(heartBeatSound, heartVol, heartPitch);

                // Staggered double-beat for panic
                if (panicIntensity > 0.4f)
                {
                    yield return new WaitForSeconds(0.15f);
                    PlaySound(heartBeatSound, heartVol * 0.7f, heartPitch * 1.1f);
                }

                // Faster beats over time
                heartBeatInterval = Mathf.Lerp(0.8f, 0.12f, panicIntensity);
                nextHeartBeat = Time.time + heartBeatInterval;
            }

            // Ragged breathing: less frequent but desperate
            if (Time.time >= nextBreathe)
            {
                float breathePitch = Mathf.Lerp(0.9f, 1.8f, panicIntensity);
                PlaySound(BreatheSound, 0.9f + panicIntensity * 0.4f, breathePitch);
                nextBreathe = Time.time + Mathf.Lerp(2.5f, 0.4f, panicIntensity);
            }

            // Random gasps & bubbles for chaos
            if (Time.time >= nextGasp && Random.value < panicIntensity * 0.4f)
            {
                PlaySound(gaspSound, 1.2f, Random.Range(1.2f, 2f));
                nextGasp = Time.time + Random.Range(1.5f, 4f);
            }
            if (Time.time >= nextBubble && Random.value < 0.6f)
            {
                PlaySound(bubbleSound, Random.Range(0.3f, 0.8f), Random.Range(0.8f, 1.4f));
                nextBubble = Time.time + Random.Range(0.8f, 2.5f);
            }

            // Camera: ramp wobble to insane levels
            SwimmingCameraWobble.Instance.intensity = Mathf.Lerp(0.05f, 0.45f, panicIntensity);
            SwimmingCameraWobble.Instance.frequency = Mathf.Lerp(1.5f, 4.5f, panicIntensity); // Faster shake

            // Visual panic: overlay darkens + pulses, oxygen UI flashes red
            if (deathOverlay) 
            {
                Color overlayColor = deathOverlay.color;
                overlayColor.a = panicIntensity * 0.7f + Mathf.Sin(elapsed * 8f) * 0.1f * panicIntensity;
                deathOverlay.color = overlayColor;
            }
            if (oxygenUI)
            {
                oxygenUI.alpha = 1f + Mathf.Sin(elapsed * 12f) * 0.5f * panicIntensity;
            }

            yield return null; // Frame-by-frame for smooth timing
            elapsed += Time.deltaTime;
        }
    }

    IEnumerator FinalDeathShakeAndGasp()
    {
        // Massive final shake
        SwimmingCameraWobble.Instance.intensity *= 3f;
        for (int i = 0; i < 8; i++)
        {
            PlaySound(finalGasp ?? gaspSound, 2f, Random.Range(0.5f, 1.2f));
            yield return new WaitForSeconds(0.2f);
        }
        SwimmingCameraWobble.Instance.intensity = 0.03f; // Reset low
    }

    // Bonus: Music fade in/out (add AudioSource musicSource;)
    IEnumerator FadeAudioSourceMusic(float fromVol, float toVol, float duration)
    {
        float elapsed = 0f;
        _currentMusicSource.volume = fromVol;
        while (elapsed < duration)
        {
            _currentMusicSource.volume = Mathf.Lerp(fromVol, toVol, elapsed / duration);
            yield return null;
            elapsed += Time.deltaTime;
        }
        _currentMusicSource.volume = toVol;
    }

    // Your existing PlaySound (enhanced with random pitch variation for organic feel)
    public void PlaySound(AudioClip clip, float vol = 1f, float pitch = 1f)
    {
        _soundsSource.pitch = pitch * (0.95f + Random.Range(-0.05f, 0.05f)); // Tiny variation
        _soundsSource.PlayOneShot(clip, vol);
    }
    
    public void PlayMusic(AudioClip clip, float fadeDuration = 1f) {
        if (clip == null) return;

        // Stop any ongoing fade coroutine to avoid conflicts
        StopAllCoroutines();

        // Swap sources
        (_currentMusicSource, _previousMusicSource) = (_previousMusicSource, _currentMusicSource);

        // Setup new clip
        _currentMusicSource.clip = clip;
        _currentMusicSource.volume = 0f;
        _currentMusicSource.loop = true; // Assuming music loops
        _currentMusicSource.Play();

        // Crossfade
        StartCoroutine(CrossfadeCoroutine(fadeDuration));
    }

    public void PlayFonk(float fadeDuration = 1f)
    {
        PlayMusic(fonkMusic);
    }

    private IEnumerator CrossfadeCoroutine(float duration) {
        float timer = 0f;
        float startPrevVolume = _previousMusicSource.volume;
        float targetPrevVolume = 0f;
        float targetCurrVolume = 1f; // Or your desired max music volume

        while (timer < duration) {
            timer += Time.unscaledDeltaTime; // Use unscaled if you want fade during pause/time scale changes
            float t = timer / duration;

            _previousMusicSource.volume = Mathf.Lerp(startPrevVolume, targetPrevVolume, t);
            _currentMusicSource.volume = Mathf.Lerp(0f, targetCurrVolume, t);

            yield return null;
        }

        _previousMusicSource.volume = 0f;
        _previousMusicSource.Stop();
        _previousMusicSource.clip = null; // Optional cleanup
    }

    // Your existing sound methods (unchanged)
    public void PlaySound(AudioClip clip, Vector3 pos, float vol = 1, float pitch = 1) {
        _soundsSource.pitch = pitch;
        _soundsSource.transform.position = pos;
        PlaySound(clip, vol);
    }



  
    public void PlayGrabSound(float vol = 1f, float pitch = 1)
    {
        
        pitch = Random.Range(0.8f, 1.2f); 
        _soundsSource.pitch = pitch;
        _soundsSource.PlayOneShot(grab, randomVolume(vol));
        _soundsSource.pitch = 1;
    }
    
    public void PlaySuccessSound(float vol = 1f, float pitch = 1)
    {
        
        pitch = Random.Range(0.8f, 1.2f); 
        _soundsSource.pitch = pitch;
        _soundsSource.PlayOneShot(collectItem, randomVolume(vol));
        _soundsSource.pitch = 1;
    }

    public void PlayShotgunSound(float vol = 1f)
    {
        AudioClip clip = shotgunSound != null ? shotgunSound : Resources.Load<AudioClip>("Music/dennish18-shotgun-146188");
        if (clip == null) return;
        _soundsSource.pitch = Random.Range(0.95f, 1.05f);
        _soundsSource.PlayOneShot(clip, randomVolume(vol));
        _soundsSource.pitch = 1f;
    }
    
    
    // Optional: Add StopMusic with fade out
    public void StopMusic(float fadeDuration = 1f)
    {
        StopAllCoroutines();
        StartCoroutine(FadeOutCoroutine(_currentMusicSource, fadeDuration));
    }

    private IEnumerator FadeOutCoroutine(AudioSource source, float duration) {
        float timer = 0f;
        float startVolume = source.volume;

        while (timer < duration) {
            timer += Time.unscaledDeltaTime;
            source.volume = Mathf.Lerp(startVolume, 0f, timer / duration);
            yield return null;
        }

        source.Stop();
        source.volume = startVolume; // Reset for next use
    }

    private float randomVolume(float a) => Random.Range(a - 0.2f, a + 0.2f);
    
}