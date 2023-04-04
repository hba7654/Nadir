using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieController : MonoBehaviour
{
    public GameObject playerObject;

    [SerializeField] private int startHealth;
    [SerializeField] private float zombieSpeed;
    [SerializeField] private float ammoHealthDropPercentage;
    [SerializeField] private GameObject ammoDrop;
    [SerializeField] private GameObject healthDrop;

    private int health;
    private Vector2 playerPos;
    private Rigidbody2D rb;
    private NavMeshAgent agent;
    private NavMeshPath path;
    private SpriteRenderer sr;

    // Start is called before the first frame update
    void Start()
    {
        health = startHealth;
        rb = GetComponent<Rigidbody2D>();

        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        path = new NavMeshPath();

        sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!GameManager.isPaused)
        { 
            playerPos = playerObject.transform.position;
            agent.speed = zombieSpeed * GameManager.dopamine * 0.7f;
            agent.SetDestination(playerObject.transform.position);

           //Flip the zombies X based off of where the player is moving
           if (rb.velocity.x > 0)
           {
               sr.flipX = false;
           }
           else
           {
               sr.flipX = true;
           }

            //if (agent.CalculatePath(playerPos, path))
            //{
            //    agent.SetPath(path);
            //}
            //agent.speed = zombieSpeed * GameManager.dopamine;
            //else
            //{
            //    rb.velocity = new Vector2(playerPos.x - transform.position.x, playerPos.y - transform.position.y).normalized * zombieSpeed * GameManager.dopamine;
            //}

            if (health <= 0)
            {
                GameManager.IncreaseDopamine();
                GameManager.zombies.Remove(gameObject);
                Destroy(gameObject);

                if (Random.Range(0, 1.0f) <= ammoHealthDropPercentage)
                {
                    if (Random.Range(0, 1.0f) <= 0.5f)
                        Instantiate(ammoDrop, transform.position, Quaternion.identity);
                    else
                    Instantiate(healthDrop, transform.position, Quaternion.identity);
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Bullet")
        {
            Destroy(collision.gameObject);
            health -= playerObject.GetComponent<PlayerShooting>().bulletDamage;

            StartCoroutine(Hurt());
        }
    }

    private IEnumerator Hurt()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.25f);
        spriteRenderer.color = Color.white;
    }
}
