using UnityEngine;
public class BonFIreVibeAudio : StaticInstance<BonFIreVibeAudio>
{
    public AudioSource audioSource;

    public AudioClip friendlyClip;
    public AudioClip creepyClip;

    private AudioClip currentClip;

    void Start()
    {
        changeSoundTO(soundType.normal);
    }


    public void changeSoundTO(soundType type)
    {
        AudioClip targetClip = null;
        switch (type)
        {
            case soundType.normal:
            {
                targetClip = friendlyClip;
                break;
            }        
            case soundType.creepy:
            {
                targetClip = creepyClip;
                break;
            }
        }
        
        if (currentClip == targetClip) return;

        currentClip = targetClip;
        audioSource.clip = currentClip;
        audioSource.Play();
    }
}

public enum soundType
{
    normal, creepy
}