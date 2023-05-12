using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CrankedManager : MonoBehaviour
{
    private GameManager gameManager;
    private float timer;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = this.GetComponent<GameManager>();
        timer = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.isPaused)
        {
            timer += Time.deltaTime;
            gameManager.UpdateText(gameManager.questText, string.Format("Time Remaining {0:F2}s", gameManager.timeToStartLosingDopamine - GameManager.timeSinceLastKill));
            //gameManager.maxZombieCount = ...
            if(GameManager.isLosingDopamine)
            {
                Die();
            }
        }
    }

    void Die()
    {
        SceneManager.LoadScene("Main Menu");
    }
}
