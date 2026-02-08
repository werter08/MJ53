using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class MusicData : StaticInstance<MusicData>
{
    public List<AudioClip> onClickSounds;
    public List<AudioClip> onHoverSounds;
    public List<AudioClip> popSounds;
    public List<AudioClip> whooshSounds;

    public List<AudioClip> mainMenuMusics;
    public List<AudioClip> normalGameMusics;

    protected override void Awake()
    {
        base.Awake();
        AssembleResources();
    }

    private void AssembleResources()
    {
        onClickSounds = Resources.LoadAll<AudioClip>("Music/ClickSounds").ToList();

        onHoverSounds = Resources.LoadAll<AudioClip>("Music/HoverSounds").ToList();

        popSounds = Resources.LoadAll<AudioClip>("Music/PopSounds").ToList();

        whooshSounds = Resources.LoadAll<AudioClip>("Music/WhooshSounds").ToList();

        mainMenuMusics = Resources.LoadAll<AudioClip>("Music/MainMenuMusics").ToList();

        normalGameMusics = Resources.LoadAll<AudioClip>("Music/NormalGameMusics").ToList();
    }
}

static public class ListExtensions
{
    static public AudioClip GetRandom(this List<AudioClip> list)
    {
        return list[Random.Range(0, list.Count)];
    }
}