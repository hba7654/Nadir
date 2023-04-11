using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;

public class SiblingController : MonoBehaviour
{
    public GameObject playerObject;

    [SerializeField] private float siblingSpeed;

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
        }
    }

    private IEnumerator Hurt()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.25f);
        spriteRenderer.color = Color.white;
    }

    private IEnumerator PathToPlayer()
    {
        agent.CalculatePath(playerPos, path);

        yield return new WaitUntil(() => !agent.pathPending);

        agent.SetPath(path);
    }
}