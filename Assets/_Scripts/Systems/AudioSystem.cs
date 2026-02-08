using UnityEngine;

/// <summary>
/// Insanely basic audio system which supports 3D sound.
/// Ensure you change the 'Sounds' audio source to use 3D spatial blend if you intend to use 3D sounds.
/// </summary>
using System.Collections;
using UnityEngine;

public class AudioSystem : StaticInstance<AudioSystem> {
    [SerializeField] private AudioSource _musicSourceA;
    [SerializeField] private AudioSource _musicSourceB;
    [SerializeField] private AudioSource _soundsSource;
    public AudioClip collectItem;
    public AudioClip grab;
    private AudioSource _currentMusicSource;
    private AudioSource _previousMusicSource;

    private void Awake() {
        // Base Awake from StaticInstance handles singleton
        base.Awake();

        _currentMusicSource = _musicSourceA;
        _previousMusicSource = _musicSourceB;
        _previousMusicSource.volume = 0f; // Start muted
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

    public void PlaySound(AudioClip clip, float vol = 1, float pitch = 1) {
        _soundsSource.pitch = pitch;
        _soundsSource.PlayOneShot(clip, vol);
    }

    public void PlayOnClickSound(float vol = 1, float pitch = 1)
    {
        if (pitch <= 1.01 && pitch >= 0.99)
        {
            pitch = Random.Range(0.8f, 1.2f); 
        }
        _soundsSource.pitch = pitch;
        _soundsSource.PlayOneShot(MusicData.Instance.onClickSounds.GetRandom(), vol);
        _soundsSource.pitch = 1;
    }



    public void PlayPopSound(float vol = 1, float pitch = 1)
    {
        if (pitch <= 1.01 && pitch >= 0.99)
        {
            pitch = Random.Range(0.8f, 1.2f); 
        }
        _soundsSource.pitch = pitch;
        _soundsSource.PlayOneShot(MusicData.Instance.popSounds.GetRandom(), randomVolume(vol));
        _soundsSource.pitch = 1;
    }

  
    public void PlayGrabSound(float vol = 0.3f, float pitch = 1)
    {
        
        pitch = Random.Range(0.8f, 1.2f); 
        _soundsSource.pitch = pitch;
        _soundsSource.PlayOneShot(grab, randomVolume(vol));
        _soundsSource.pitch = 1;
    }
    
    public void PlaySuccessSound(float vol = 0.3f, float pitch = 1)
    {
        
        pitch = Random.Range(0.8f, 1.2f); 
        _soundsSource.pitch = pitch;
        _soundsSource.PlayOneShot(collectItem, randomVolume(vol));
        _soundsSource.pitch = 1;
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