using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed;

    private Rigidbody2D rb;
    //private bool isMoving;
    private Vector2 moveVector;

    // Start is called before the first frame update
    void Start()
    {
        //isMoving = false;
        moveVector = Vector2.zero;
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rb.velocity = moveVector * moveSpeed * GameManager.dopamine;
    }

    public void Move(InputAction.CallbackContext context)
    {
        //if (context.started)
        //{
        //    isMoving = true;
        //}
        //else if (context.canceled)
        //{
        //    isMoving = false;
        //}

        moveVector = context.ReadValue<Vector2>();

    }
}
