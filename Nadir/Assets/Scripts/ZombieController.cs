using System.Collections;
using System.Collections.Generic;
using System.Threading;
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

    private Vector2 lastPos, curPos, dir;
    float timer = 0, speed;

    // Start is called before the first frame update
    void Start()
    {
        health = startHealth;
        rb = GetComponent<Rigidbody2D>();

        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.speed = 5;
        agent.updatePosition = true;
        path = new NavMeshPath();

        sr = GetComponent<SpriteRenderer>();
        lastPos = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(!GameManager.isPaused)
        {
            timer += Time.deltaTime;
            playerPos = playerObject.transform.position;
            agent.speed = zombieSpeed * GameManager.dopamine;

            StartCoroutine(NewPath());

            curPos = transform.position;

           //Flip the zombies X based off of where the player is moving
           if (lastPos.x < curPos.x)
           {
               sr.flipX = false;
           }
           else
           {
               sr.flipX = true;
           }

            lastPos = curPos;


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

    private IEnumerator NewPath()
    {
        agent.CalculatePath(playerPos, path);

        yield return new WaitUntil(() => !agent.pathPending);

        agent.SetPath(path);
    }
}
