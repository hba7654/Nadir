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
    [SerializeField] private float despawnTime;
    [SerializeField] private float ammoHealthDropPercentage;
    [SerializeField] private GameObject ammoDrop;
    [SerializeField] private GameObject healthDrop;

    private int health;
    private Vector2 playerPos;
    private Rigidbody2D rb;
    private NavMeshAgent agent;
    private NavMeshPath path;
    private SpriteRenderer sr;
    private float despawnTimer;
    private bool isInvis;

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

        despawnTimer = despawnTime;
        isInvis = false;

        StartCoroutine(Groan());
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!GameManager.isPaused)
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
                    if (Random.Range(0, 1.0f) <= 0.75f)
                        Instantiate(ammoDrop, transform.position, Quaternion.identity);
                    else
                        Instantiate(healthDrop, transform.position, Quaternion.identity);
                }
            }

            if (isInvis)
                despawnTimer -= Time.deltaTime;

            if (despawnTimer < 0)
            {
                GameManager.zombies.Remove(gameObject);
                Destroy(gameObject);
            }
        }
        else
        {
            agent.speed = 0;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //TODO
        if (collision.tag == "Bullet")
        {
            //Play hurt sound

            //Deagle bullets go through
            if(!(playerObject.GetComponent<PlayerShooting>().weapon == PlayerShooting.Weapons.Pistol && GameManager.dopamine >= 15))
                Destroy(collision.gameObject);


            if(GameManager.dopamine >= 10)
                health -= 2 * playerObject.GetComponent<PlayerShooting>().bulletDamage;
            else
                health -= playerObject.GetComponent<PlayerShooting>().bulletDamage;

            StartCoroutine(Hurt());
        }
        else if (collision.tag == "Sibling Bullet")
        {
            //Play hurt sound

            Destroy(collision.gameObject);
            health -= 2;

            StartCoroutine(Hurt());
        }
    }

    private void OnBecameInvisible()
    {
        despawnTimer = despawnTime;
        isInvis = true;
    }

    private IEnumerator Hurt()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.25f);
        spriteRenderer.color = Color.white;
    }

    private IEnumerator Groan()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(3, 10.5f));
            SoundManager.PlaySound("zombie_" + Random.Range(0, 4).ToString());
        }
    }

    private IEnumerator NewPath()
    {
        agent.CalculatePath(playerPos, path);

        yield return new WaitUntil(() => !agent.pathPending);

        agent.SetPath(path);
    }
}
