using System;
using UnityEngine;

public enum MusicType
{
    StartGame,
    Running
}
public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioClip startGameMusic;

    [SerializeField] private AudioClip runningMusic;
    [SerializeField] private AudioSource audioSource;

    private static AudioManager instance;
    public static AudioManager Instance => instance;
    
    public AudioSource AudioSource => audioSource;
    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        audioSource.loop = true;
        if (startGameMusic)
        {
            audioSource.clip = startGameMusic;
            audioSource.Play();
        }
    }
    public void PlayMusic(MusicType musicType)
    {
        switch (musicType)
        {
            case MusicType.StartGame:
                audioSource.clip = startGameMusic;
                break;
            case MusicType.Running:
                audioSource.clip = runningMusic;
                break;
            default:
                break;
        }
        audioSource.Play();
    }
}