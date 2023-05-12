using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainGameManager : MonoBehaviour
{
    private GameManager gameManager;
    private float timer;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = this.GetComponent<GameManager>();
        timer = 300.0f;
    }

    // Update is called once per frame
    void Update()
    {
        // Updates the quest and the quest text in the UI
        switch (gameManager.playerObject.GetComponent<PlayerManager>().questStep)
        {
            // Quest step 1
            case 1:
                gameManager.questText.text = string.Format("Kill {0} zombies", 15 - GameManager.zombieCounter);
                if (GameManager.zombieCounter >= 15)
                {
                    gameManager.playerObject.GetComponent<PlayerManager>().questStep++;
                }
                break;

            // Quest step 2
            case 2:
                gameManager.questText.text = string.Format("Find {0} bomb parts", 3 - gameManager.playerObject.GetComponent<PlayerManager>().bombParts);
                break;

            // Quest step 3
            case 3:
                gameManager.UpdateText(gameManager.questText, "Find the Zombie Kingdom");
                gameManager.maxZombieCount = 25;
                gameManager.dopamineMax = 20;
                break;

            // Quest step 4
            case 4:
                gameManager.UpdateText(gameManager.questText, "Find the Zombie Key. Hint nearby cave");
                gameManager.maxZombieCount = 40;
                gameManager.zombieSpawnFrequency = 4;
                break;

            // Quest step 5
            case 5:
                timer -= Time.deltaTime * GameManager.dopamine;
                gameManager.UpdateText(gameManager.questText, string.Format("SURVIVE for {0:F2}", timer / gameManager.dopamineStart));

                if (timer < 0)
                {
                    gameManager.playerObject.GetComponent<PlayerManager>().mountainTileSet.SetActive(false);
                    gameManager.playerObject.GetComponent<PlayerManager>().questStep++;
                }

                break;

            // Quest step 6
            case 6:
                gameManager.UpdateText(gameManager.questText, "Use the key on the gates of the Zombie Kingdom. Hint Top of the map");
                break;

            // Quest step 6
            case 7:
                gameManager.UpdateText(gameManager.questText, "ANDDD THATS ALL FOLKS... GOOD LUCK SURVIVING");
                gameManager.maxZombieCount = 60;
                gameManager.zombieSpawnFrequency = 3;
                break;
        }
    }
}
