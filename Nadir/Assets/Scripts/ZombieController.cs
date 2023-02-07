using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieController : MonoBehaviour
{
    [SerializeField] private float zombieSpeed;
    [SerializeField] private GameObject playerObject;

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
        rb.velocity = new Vector2(-transform.position.x + playerPos.x, -transform.position.y + playerPos.y).normalized * zombieSpeed * GameManager.dopamine;
    }
}
