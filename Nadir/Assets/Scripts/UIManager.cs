using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject howTo;
    [SerializeField] private GameObject credits;
    [SerializeField] private GameObject minimap;

    // Start is called before the first frame update
    void Start()
    {
        mainMenu.SetActive(true);

        howTo.SetActive(false);
        credits.SetActive(false);

        minimap.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(GameManager.isPaused)
        {
            minimap.SetActive(true);
        }
        else
        {
            minimap.SetActive(false);
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Level 1");
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }

    public void MainMenu()
    {
        mainMenu.SetActive(true);

        howTo.SetActive(false);
        credits.SetActive(false);
    }

    public void InstructionsScene()
    {
        howTo.SetActive(true);

        mainMenu.SetActive(false);
        credits.SetActive(false);
    }

    public void CreditsScene()
    {
        credits.SetActive(true);

        mainMenu.SetActive(false);
        howTo.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
