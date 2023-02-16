using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Movement Variables")]
    [SerializeField] private float bulletSpeed;
    private Vector2 currentVelocity;
    private Rigidbody2D rb;


    [Header("Cleanup Variables")]
    //This is for cleaning up created bullets when the pass a certain point
    [SerializeField] private float maxDistanceX;
    [SerializeField] private float maxDistanceY;
    private Vector2 initialPos;



    public void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Update()
    {
        //checks position, if out of bounds destroy the object
        if (transform.position.x > initialPos.x + maxDistanceX || transform.position.x < initialPos.x - maxDistanceX
            || transform.position.y > initialPos.y + maxDistanceY || transform.position.y < initialPos.y - maxDistanceY)
        {
            Destroy(gameObject);
        }
    }

    public void InitialMove(Vector2 initalVelocity)
    {
        initialPos = transform.position;
        rb.velocity = initalVelocity * bulletSpeed * GameManager.dopamine;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Zombie")
        {
            Debug.Log("HIT");
            GameManager.zombies.Remove(collision.gameObject);
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
        else if (collision.tag != "Player")
            Destroy(gameObject);
    }
}