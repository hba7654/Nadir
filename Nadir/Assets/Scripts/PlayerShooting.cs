using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooting : MonoBehaviour
{
    public int bulletDamage;
    public int mgAmmo;
    public int sgAmmo;

    [SerializeField] private GameObject bullet;
    [SerializeField] private Weapons weapon;

    private int startMGAmmo;
    private int startSGAmmo;
    private float fireRate;
    private bool isAiming;
    private bool usingMouse;
    private Vector2 mouseDirVector;
    private Vector2 mousePosition;
    private float shootTimer;

    enum Weapons { Pistol, Machinegun, Shotgun };
    // Start is called before the first frame update
    void Start()
    {
        shootTimer = 0;

        startMGAmmo = 60;
        startSGAmmo = 16;

    }

    // Update is called once per frame
    void Update()
    {
        if(!GameManager.isPaused)
        {
            switch (weapon)
            {
                case Weapons.Pistol:
                    fireRate = 1f;
                    bulletDamage = 5;
                    break;
                case Weapons.Machinegun:
                    fireRate = 5f;
                    bulletDamage = 10;
                    break;
                case Weapons.Shotgun:
                    fireRate = 0.25f;
                    bulletDamage = 2;
                    break;
            }

            shootTimer -= (Time.deltaTime * GameManager.dopamine);

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

    }

    public void Shoot(InputAction.CallbackContext context)
    {
        if (!GameManager.isPaused && context.performed && isAiming && /*ammo > 0 &&*/ shootTimer <= 0)
        {
            shootTimer = 1 / fireRate;

            //ammo--;
            //Debug.Log(ammo + " ammo left");

            //soundManager.PlaySound("shoot");
            GameObject bulletClone;
            Vector2 bulletSpawnPosition;
            //if (playerManager.isFacingRight)
            bulletSpawnPosition = (Vector2)transform.position + mouseDirVector / 2;
            //else
            //    bulletSpawnPosition = new Vector2(transform.position.x - 0.5f, transform.position.y);
            bulletClone = Instantiate(bullet, bulletSpawnPosition, transform.rotation);
            bulletClone.GetComponent<Bullet>().InitialMove(mouseDirVector);
        }
    }

    public void Aim(InputAction.CallbackContext context)
    {
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Ammo")
        {
            sgAmmo += 4;
            mgAmmo += 15;
            Destroy(collision.gameObject);
        }
    }
}
