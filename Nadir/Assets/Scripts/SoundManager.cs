using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioClip[] soundClips;

    private static AudioSource audioSource;
    private static AudioClip[] soundClipsStatic;
    // Start is called before the first frame update

    private void Start()
    {
        soundClipsStatic = soundClips;
        audioSource = GetComponent<AudioSource>();
    }

    public static void PlaySound(string sound)
    {
        AudioClip clip = Contains(sound);
        if(clip == null)
        {
            Debug.Log("Sound no Exist");
            return;
        }

        if(!audioSource.isPlaying)
            audioSource.PlayOneShot(clip);
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
