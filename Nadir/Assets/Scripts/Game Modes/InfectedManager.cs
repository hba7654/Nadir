using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InfectedManager : MonoBehaviour
{
    private GameManager gameManager;
    [SerializeField] private float timer;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = this.GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.isPaused)
        {
            //gameManager.maxZombieCount = ...
            timer -= Time.deltaTime;
            gameManager.UpdateText(gameManager.questText, string.Format("Time Remaining {0:F2}s", timer));

            if(timer <= 0)
            {
                GameManager.Die();
            }
        }
    }
}
