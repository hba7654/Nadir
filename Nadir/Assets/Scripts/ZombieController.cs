using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    // Start is called before the first frame update
    void Start()
    {
        health = startHealth;
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        playerPos = playerObject.transform.position;
        rb.velocity = new Vector2(playerPos.x - transform.position.x, playerPos.y - transform.position.y).normalized * zombieSpeed * GameManager.dopamine;
    
        if(health <= 0)
        {
            GameManager.IncreaseDopamine();
            GameManager.zombies.Remove(gameObject);
            Destroy(gameObject);

            if (Random.Range(0, 1.0f) <= ammoHealthDropPercentage)
            {
                //if (Random.Range(0, 1.0f) <= 0.5f)
                //    Instantiate(ammoDrop, transform.position, Quaternion.identity);
                //else
                Instantiate(healthDrop, transform.position, Quaternion.identity);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Bullet")
        {
            Destroy(collision.gameObject);
            health -= playerObject.GetComponent<PlayerController>().bulletDamage;

            StartCoroutine(Hurt());
        }
    }

    private IEnumerator Hurt()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        spriteRenderer.color = Color.yellow;
        yield return new WaitForSeconds(0.25f);
        spriteRenderer.color = Color.red;
    }
}
