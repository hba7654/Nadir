using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Movement Variables")]
    [SerializeField] private float bulletSpeed;

    [HideInInspector] public int damage;
    [HideInInspector] public float speedMultiplier;

    private Rigidbody2D rb;



    public void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void OnBecameInvisible()
    {
        Destroy(gameObject);
    }

    public void InitialMove(Vector2 initalVelocity)
    {
        rb.velocity = initalVelocity * bulletSpeed * speedMultiplier * GameManager.dopamine;
    }

}