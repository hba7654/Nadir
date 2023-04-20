using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class SiblingController : MonoBehaviour
{
    [SerializeField] public GameObject playerObject;
    [SerializeField] private GameObject bullet;
    [SerializeField] private float siblingSpeed;
    [SerializeField] private float fireRate;

    private Vector2 playerPos;
    private Rigidbody2D rb;
    private NavMeshAgent agent;
    private NavMeshPath path;
    private SpriteRenderer sr;

    private Vector2 lastPos;
    private Vector2 curPos;
    private Vector2 dir;
    private Vector2 shootDirVector;
    float timer;
    float shootTimer;
    float speed;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.speed = 5;
        agent.updatePosition = true;
        path = new NavMeshPath();

        sr = GetComponent<SpriteRenderer>();
        lastPos = transform.position;
        timer = 0;

        transform.rotation = Quaternion.identity;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!GameManager.isPaused)
        {
            timer += Time.deltaTime;
            playerPos = playerObject.transform.position;
            agent.speed = siblingSpeed * GameManager.dopamine;

            StartCoroutine(PathToPlayer());

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

            shootTimer -= Time.deltaTime * GameManager.dopamine;
            if(shootTimer < 0)
            {
                shootTimer = fireRate;
                Shoot();
            }
        }
        else
        {
            agent.speed = 0;
        }
    }

    private IEnumerator PathToPlayer()
    {
        agent.CalculatePath(playerPos, path);

        yield return new WaitUntil(() => !agent.pathPending);

        agent.SetPath(path);
    }

    private void Shoot()
    {
        //shootTimer = 1 / fireRate;
        GameObject bulletClone;
        Vector2 bulletSpawnPosition;
        shootDirVector = GameManager.zombies[Random.Range(0, GameManager.zombies.Count)].transform.position - transform.position;
        //shootDirVector = Mouse.current.position.ReadValue() - (Vector2)transform.position;
        bulletSpawnPosition = (Vector2)transform.position + shootDirVector / 2;
        bulletClone = Instantiate(bullet, bulletSpawnPosition, transform.rotation);
        bulletClone.tag = "Sibling Bullet";
        bulletClone.GetComponent<Bullet>().InitialMove(shootDirVector);
    }
}