using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private int startHealth;
    [SerializeField] private int startAmmo;
    [SerializeField] private float moveSpeed;
    [SerializeField] private GameObject bullet;

    private int health;
    private int ammo;
    private Rigidbody2D rb;
    //private bool isMoving;
    private Vector2 moveVector;
    private bool isAiming;
    private bool usingMouse;
    private Vector2 mouseDirVector;
    private Vector2 mousePosition;

    // Start is called before the first frame update
    void Start()
    {
        health = startHealth;
        //isMoving = false;
        moveVector = Vector2.zero;
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if(health <= 0)
        {
            health = 0;
            Debug.Log("Dead");
        }

        if (usingMouse)
        {
            {
                mousePosition = GetMousePosition();
                mouseDirVector = GetMouseVector();
            }
            //crosshair.transform.position = mousePosition;
        }
        else if (isAiming)
        {
            //RaycastHit2D hit = Physics2D.Raycast(transform.position, mouseDirVector);
            //if (hit.collider != null)
            //{
            //    //crosshair.transform.position = hit.point;
            //}
        }
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

    public void Shoot(InputAction.CallbackContext context)
    {
        if (context.performed && isAiming)
        {
            //soundManager.PlaySound("shoot");
            GameObject bulletClone;
            Vector2 bulletSpawnPosition;
            //if (playerManager.isFacingRight)
                bulletSpawnPosition = new Vector2(transform.position.x + 0.5f, transform.position.y);
            //else
            //    bulletSpawnPosition = new Vector2(transform.position.x - 0.5f, transform.position.y);
            bulletClone = Instantiate(bullet, bulletSpawnPosition, transform.rotation);
            bulletClone.GetComponent<Bullet>().InitialMove(mouseDirVector);
        }
    }

    public void Aim(InputAction.CallbackContext context)
    {
        Debug.Log(context.control.displayName);
        //Controller Controls
        if (context.control.displayName == "Right Stick")
        {
            mouseDirVector = context.ReadValue<Vector2>();
            //mouseDirVector.x = Mathf.Round(mouseDirVector.x * 50) / 50;
            //mouseDirVector.y = Mathf.Round(mouseDirVector.y * 50) / 50;

            isAiming = true;
            usingMouse = false;
            //crosshair.SetActive(true);
        }
        //Mouse Controls
        else if (context.control.displayName == "Position")
        {
            isAiming = true;
            usingMouse = true;
            //crosshair.SetActive(true);
        }

        if (context.canceled)
        {
            isAiming = false;
            //crosshair.SetActive(false);
        }
    }

    //Gets the Aiming direction for mouse
    public Vector2 GetMouseVector()
    {

        Vector3 playerPos = transform.position;
        return new Vector2(mousePosition.x - playerPos.x, mousePosition.y - playerPos.y).normalized;
    }

    //get mouse position on the screen
    public Vector2 GetMousePosition()
    {
        Vector3 playerPos = transform.position;
        Vector3 mousePos = Mouse.current.position.ReadValue();
        Vector3 Worldpos = Camera.main.ScreenToWorldPoint(mousePos);
        return new Vector2(Worldpos.x, Worldpos.y);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Zombie")
        {
            health--;
            Debug.Log("OUCH, health = " + health);
        }
    }
}
