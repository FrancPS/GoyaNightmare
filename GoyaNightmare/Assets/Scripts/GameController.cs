using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class GameController : MonoBehaviour
{

    public static GameController gameController;

    public AudioMixer projectMixer;
    public float gameVolume = 0;

    static public AudioMixer masterMixer { get; private set; }
    static public float masterVolume { get; private set; }

    void Awake()
    {
        if (gameController != null && gameController != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            gameController = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    void Start()
    {
        masterMixer = projectMixer;
        masterVolume = gameVolume;
        masterMixer.SetFloat("masterVolume", masterVolume);
    }

    public static void SetGeneralVolume(float volume)
    {
        masterVolume = volume;
        masterMixer.SetFloat("masterVolume", masterVolume);
    }
}
