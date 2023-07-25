using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeManager : MonoBehaviour
{
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;

    public static float sfxVolume = 0.5f;
    public static float musicVolume = 0.5f;
    // Start is called before the first frame update
    void Start()
    {
        sfxVolumeSlider.value = sfxVolume;
        musicVolumeSlider.value = musicVolume;
    }

    // Update is called once per frame
    void Update()
    {
        sfxVolume = sfxVolumeSlider.value;
        musicVolume = musicVolumeSlider.value;
    }
}
