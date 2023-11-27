using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndScreenManager : MonoBehaviour
{
    [SerializeField] private Text kills;
    [SerializeField] private Text timeElapsed;
    [SerializeField] private AudioSource music;

    // Start is called before the first frame update
    void Start()
    {
        timeElapsed.text = string.Format("Survived for {0:F2} seconds", GameManager.timeElapsed);
        kills.text = string.Format("Killed a total of {0} zombies", GameManager.kills); ;
    }

    private void Update()
    {
        music.volume = VolumeManager.musicVolume;
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene("Main Menu");
        SoundManager.musicLocation = SoundManager.MusicLocation.Menu;
        SoundManager.lerpT = 0;
    }
}
