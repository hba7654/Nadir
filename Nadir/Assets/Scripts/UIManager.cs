using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject playMenu;
    [SerializeField] private GameObject howTo;
    [SerializeField] private GameObject credits;

    [SerializeField] private Image bg;
    [SerializeField] private Sprite[] bgAnim;

    private int arrCounter;
    private int updateCounter;

    // Start is called before the first frame update
    void Start()
    {
        mainMenu.SetActive(true);

        howTo.SetActive(false);
        credits.SetActive(false);
        playMenu.SetActive(false);

        arrCounter = 0;
        updateCounter = 0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (bg != null)
        {
            updateCounter = (updateCounter + 1) % 5;
            if (updateCounter == 0)
            {
                bg.sprite = bgAnim[arrCounter];
                arrCounter = (arrCounter + 1) % bgAnim.Length;
            }
        }
    }

    public void StartStory()
    {
        SceneManager.LoadScene("Level 1");
    }

    public void StartCranked()
    {
        SceneManager.LoadScene("Cranked");
    }

    public void StartFriends()
    {
        SceneManager.LoadScene("Friends");
    }

    public void StartInfected()
    {
        SceneManager.LoadScene("Infected");
    }

    public void StartHorde()
    {
        SceneManager.LoadScene("UnlimitedHorde");
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
        playMenu.SetActive(false);
    }
    public void PlayScene()
    {
        playMenu.SetActive(true);

        howTo.SetActive(false);
        mainMenu.SetActive(false);
        credits.SetActive(false);
    }

    public void InstructionsScene()
    {
        howTo.SetActive(true);

        mainMenu.SetActive(false);
        credits.SetActive(false);
        playMenu.SetActive(false);
    }

    public void CreditsScene()
    {
        credits.SetActive(true);

        mainMenu.SetActive(false);
        howTo.SetActive(false);
        playMenu.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
