using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed;

    private Rigidbody2D rb;
    private bool isMoving;
    private Vector2 moveVector;
    private Animator animator;
    private SpriteRenderer playerSr;
    // Start is called before the first frame update
    void Start()
    {
        //isMoving = false;
        moveVector = Vector2.zero;
        rb = GetComponent<Rigidbody2D>();

        animator = GetComponent<Animator>();

        playerSr = GetComponent<SpriteRenderer>();


    }

    void FixedUpdate()
    {
        if (!GameManager.isPaused)
        {
            rb.velocity = moveVector * moveSpeed * GameManager.dopamine;
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            isMoving = true;
            animator.SetBool("Walking", true);
        }
        else if (context.canceled)
        {
            isMoving = false;
            animator.SetBool("Walking", false);
        }

        moveVector = context.ReadValue<Vector2>();

        if (moveVector.x > 0)
        {
            playerSr.flipX = false;
        }
        else if (moveVector.x < 0)
        {
            playerSr.flipX = true;
        }

        // Debug.Log(rb.position.x + " , " + rb.position.y);

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Cave 1")
        {
            MainGameManager.ChangeToCam("cave 1");
        }
        else if (collision.tag == "Cave 2")
        {
            MainGameManager.ChangeToCam("cave 2");
        }
        else if (collision.tag == "Cave 2 Inner")
        {
            MainGameManager.ChangeToCam("cave 2 inner");
        }
        else if (collision.tag == "Main Floor")
        {
            MainGameManager.ChangeToCam("main");
        }
    }
}
