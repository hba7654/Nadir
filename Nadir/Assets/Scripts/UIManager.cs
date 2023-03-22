using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Main Game");
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }

    public void InstructionsScene()
    {
        SceneManager.LoadScene("Instructions");
    }

    public void CreditsScene()
    {
        SceneManager.LoadScene("Credits");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
