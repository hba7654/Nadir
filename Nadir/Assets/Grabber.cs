using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grabber : MonoBehaviour
{
    PlayerManager playerManager;

    private void Awake()
    {
        playerManager = GetComponentInParent<PlayerManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.tag == "Health")
        {
            playerManager.health += 5;
            Destroy(collision.gameObject);
        }

        else if (collision.tag == "Bomb Part")
        {
            if (playerManager.questStep == 2)
            {
                playerManager.bombParts++;
                Debug.Log("quest pickup  " + playerManager.bombParts + " bomb parts have been collected");

                Destroy(collision.gameObject);
            }

            else if (playerManager.questStep == 4)
            {
                Debug.Log("OH NO SURVIVE NOW YO UNEED TO SURVIVE BOY");

                // Make the cave wall visable
                playerManager.mountainTileSet.SetActive(true);

                Destroy(collision.gameObject);

                playerManager.questStep++;
            }
        }
        else if (collision.tag == "Zombie Kingdom")
        {
            if (playerManager.questStep == 3 || playerManager.questStep == 6)
            {
                Debug.Log("ZOMBIE KINGDOM");


                playerManager.questStep++;
            }
        }
    }
}
