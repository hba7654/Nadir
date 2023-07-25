using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioClip[] soundClips;
    [SerializeField] private AudioClip[] songs;
    [SerializeField] private AudioSource musicPlayer;

    private static AudioSource audioSource;
    private static AudioClip[] soundClipsStatic;
    // Start is called before the first frame update

    private void Start()
    {
        soundClipsStatic = soundClips;
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        musicPlayer.volume = GameManager.isPaused ? VolumeManager.musicVolume * 0.25f : VolumeManager.musicVolume;
        if(!musicPlayer.isPlaying)
        {
            musicPlayer.clip = songs[Random.Range(0, songs.Length)];
            musicPlayer.Play();
        }
    }

    public static void PlaySound(string sound)
    {
        AudioClip clip = Contains(sound);
        if(clip == null)
        {
            Debug.Log("Sound no Exist");
            return;
        }

        //if(!audioSource.isPlaying)
            audioSource.PlayOneShot(clip, VolumeManager.sfxVolume);
    }

    private static AudioClip Contains(string name)
    {
        for(int i = 0; i < soundClipsStatic.Length; i++)
        {
            if (soundClipsStatic[i].name == name)
                return soundClipsStatic[i];
        }
        return null;
    }

    public void NextSong()
    {
        musicPlayer.Stop();
        musicPlayer.clip = songs[Random.Range(0, songs.Length)];
        musicPlayer.Play();
    }

}
