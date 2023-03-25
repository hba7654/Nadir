using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private GameObject mainFloor;
    [SerializeField] private GameObject caveOne;
    [SerializeField] private GameObject caveTwo;
    [SerializeField] private GameObject caveTwoInner;

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

        mainFloor.SetActive(true);
        caveOne.SetActive(false);
        caveTwo.SetActive(false);
        caveTwoInner.SetActive(false);
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
            mainFloor.SetActive(false);
            caveOne.SetActive(true);
        }
        else if (collision.tag == "Cave 2")
        {
            mainFloor.SetActive(false);
            caveTwoInner.SetActive(false);
            caveTwo.SetActive(true);
        }
        else if (collision.tag == "Cave 2 Inner")
        {
            caveTwo.SetActive(false);
            caveTwoInner.SetActive(true);
        }
        else if (collision.tag == "Main Floor")
        {
            mainFloor.SetActive(true);
            caveOne.SetActive(false);
            caveTwo.SetActive(false);
            caveTwoInner.SetActive(false);
        }
    }
}