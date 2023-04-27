using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioClip[] soundClips;
    [SerializeField] private AudioClip[] songs;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private AudioSource musicPlayer;

    private static AudioSource audioSource;
    private static Slider sfxVolumeSliderStatic;
    private static AudioClip[] soundClipsStatic;
    // Start is called before the first frame update

    private void Start()
    {
        soundClipsStatic = soundClips;
        sfxVolumeSliderStatic = sfxVolumeSlider;
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        musicPlayer.volume = GameManager.isPaused ? musicVolumeSlider.value * 0.5f : musicVolumeSlider.value;
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
            audioSource.PlayOneShot(clip, sfxVolumeSliderStatic.value);
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
