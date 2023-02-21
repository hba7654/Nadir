using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieController : MonoBehaviour
{
    public GameObject playerObject;

    [SerializeField] private float zombieSpeed;
    [SerializeField] private float ammoHealthDropPercentage;
    [SerializeField] private GameObject ammoDrop;
    [SerializeField] private GameObject healthDrop;

    private Vector2 playerPos;
    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        playerPos = playerObject.transform.position;
        rb.velocity = new Vector2(playerPos.x - transform.position.x, playerPos.y - transform.position.y).normalized * zombieSpeed * GameManager.dopamine;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Bullet")
        {
            Debug.Log("HIT");
            GameManager.zombies.Remove(gameObject);
            Destroy(collision.gameObject);
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
}
