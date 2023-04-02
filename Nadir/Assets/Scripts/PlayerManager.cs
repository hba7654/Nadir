using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;


public class PlayerManager : MonoBehaviour
{
    public int health;

    [SerializeField] private int startHealth;
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private Tile mockTile;
    [SerializeField] private GameObject mountainTileSet;

    private int bombParts;

    private Rigidbody2D rb;
    private bool questPickup1;
    private bool questPickup2;
    private bool questPickup3;

    public int questStep;

    // Start is called before the first frame update
    void Start()
    {
        health = startHealth;
        rb = GetComponent<Rigidbody2D>();

        questPickup1 = false;
        questPickup2 = false;
        questPickup3 = false;
        bombParts = 0;

        questStep = 1;
    }

    private void Update()
    {
        if (!GameManager.isPaused)
        {
            
            if (health <= 0)
            {
                health = 0;
                Debug.Log("Dead");
                SceneManager.LoadScene("Main Menu");
            }

            if (bombParts == 3)
            {
                Debug.Log("Mountain EXPLODED!!!!");
                mountainTileSet.SetActive(false);
                bombParts++;
                questStep = 3;
            }
        }
    }


    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Zombie")
        {
            health--;
            Debug.Log("OUCH, health = " + health);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.tag == "Health")
        {
            health += 5;
            Destroy(collision.gameObject);
        }

        else if(questStep == 2)
        {
            if (collision.tag == "Bomb Part")
            {
                bombParts++;
                Debug.Log("quest pickup  " + bombParts + " bomb parts have been collected");

                Destroy(collision.gameObject);
            }
        }
    }
}
