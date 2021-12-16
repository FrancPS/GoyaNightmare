using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    public static AudioController audioController;

    [Header("Game Objects for Audio")]
    public GameObject musicGameObject = null;
    public GameObject sfxGameObject = null;


    public static AudioSource[] audios = null;
    public static AudioSource[] sfx = null;

    static uint currentLevel = 1;

    enum Music
    {
        START,
        LVL2,
        LVL3,
        LVL4,
        FINAL_MUSIC,
        DEATH_MUSIC
    }

    enum SFX
    {
        AMBIENT,
        CHANGE_LVL
    }

    private void Awake()
    {
        audioController = this;
    }

    void Start()
    {
        audios = musicGameObject.GetComponents<AudioSource>();
        sfx = sfxGameObject.GetComponents<AudioSource>();

        audios[(int)Music.START].Play();
        sfx[(int)SFX.AMBIENT].Play();
    }

    public static void ChangeLevelMusic(uint level)
    {
        if (level < 4)
        {
            sfx[(int)SFX.CHANGE_LVL].Play();
        }

        if (level < 5)
        {
            audioController.StartCoroutine(AudioTransition(audios[level - 2], audios[level - 1], 1.0f));
        }
        else if (level == 5)
        {
            audios[(int)Music.LVL4].Stop();
            sfx[(int)SFX.AMBIENT].Stop();
            audios[(int)Music.FINAL_MUSIC].Play();
        }
        else
        {
            audios[currentLevel - 1].Stop();
            audios[(int)Music.DEATH_MUSIC].Play();
        }

        currentLevel = level;
    }

    public static IEnumerator AudioTransition(AudioSource audioSource, AudioSource nextAudioSource, float fadeTime)
    {
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / fadeTime;

            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = startVolume;
        nextAudioSource.Play();
    }
}

