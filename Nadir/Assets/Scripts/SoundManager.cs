using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioClip[] soundClips;
    [SerializeField] private AudioClip grassLoop;
    [SerializeField] private AudioClip caveLoop;
    [SerializeField] private AudioClip menuLoop;
    [SerializeField] private AudioSource musicPlayer1;
    [SerializeField] private AudioSource musicPlayer2;

    private static AudioSource audioSource;
    private static AudioClip[] soundClipsStatic;

    private float volume;
    public static float t;

    public enum MusicLocation { Grass, Cave, Menu };
    public static MusicLocation musicLocation;

    // Start is called before the first frame update
    private void Start()
    {
        soundClipsStatic = soundClips;
        audioSource = GetComponent<AudioSource>();

        if (musicLocation == MusicLocation.Menu)
            musicPlayer2 = null;
    }

    private void Update()
    {
        if (!musicPlayer1.isPlaying)
        { 
            if(musicLocation == MusicLocation.Menu)
                musicPlayer1.clip = menuLoop;
            else
                musicPlayer1.clip = grassLoop;
            musicPlayer1.loop = true;
            musicPlayer1.Play();
        }
        if (!musicPlayer2.isPlaying)
        {
            musicPlayer2.clip = caveLoop;
            musicPlayer1.loop = true;
            musicPlayer2.Play();
        }
        if (musicLocation != MusicLocation.Cave)
        {
            volume = GameManager.isPaused ? VolumeManager.musicVolume * 0.25f : VolumeManager.musicVolume;
        }

        switch (musicLocation)
        {
            case MusicLocation.Grass:
            case MusicLocation.Menu:
                musicPlayer1.volume = Mathf.Lerp(0, volume, t);
                musicPlayer2.volume = Mathf.Lerp(volume, 0, t);
                break;

            case MusicLocation.Cave:
                musicPlayer2.volume = Mathf.Lerp(0, volume, t);
                musicPlayer1.volume = Mathf.Lerp(volume, 0, t);
                break;
        }
        t += volume * Time.deltaTime;
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

}
