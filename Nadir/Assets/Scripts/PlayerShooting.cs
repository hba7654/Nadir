using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.PlayerSettings;

public class PlayerShooting : MonoBehaviour
{
    public int bulletDamage;
    public int mgAmmo;
    public int sgAmmo;

    [SerializeField] private GameObject bullet;
    [SerializeField] private Weapons weapon;

    private int startMGAmmo;
    private int startSGAmmo;
    private bool isShootingMG;
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
        mgAmmo = startMGAmmo;
        sgAmmo = startSGAmmo;
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

    private void FixedUpdate()
    {
        //Shoot Machine Gun
        while (mgAmmo > 0 && isShootingMG && canShoot())
        {
            mgAmmo--;
            StartCoroutine(ShootBullet());
        }
    }

    public void NextWeapon(InputAction.CallbackContext context)
    {
        if(context.started)
            weapon = (Weapons)((int)(weapon + 1) % (Enum.GetValues(typeof(Weapons)).Length));
    }

    public void Shoot(InputAction.CallbackContext context)
    {
        if (context.canceled)
        {
            isShootingMG = false;
        }
        if (context.started && canShoot())
        {
            switch(weapon)
            {
                case Weapons.Pistol:
                    StartCoroutine(ShootBullet());
                    break;
                case Weapons.Machinegun: //Code handling machine gin fire is in FixedUpdate
                    isShootingMG=true;
                    break;
                case Weapons.Shotgun:
                    ShootSG();
                    break;
            }
        }
    }


    private IEnumerator ShootBullet()
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

        yield return null;
    }

    private void ShootSG()
    {
        if(sgAmmo > 0)
        {
            sgAmmo--;
            shootTimer = 1 / fireRate;

            //soundManager.PlaySound("shotgun");
            GameObject bulletClone;
            Vector2 bulletSpawnPosition;
            //if (playerManager.isFacingRight)
            bulletSpawnPosition = (Vector2)transform.position + mouseDirVector / 2;
            //else
            //    bulletSpawnPosition = new Vector2(transform.position.x - 0.5f, transform.position.y);
            Vector2 initialAngle = Quaternion.Euler(0, 0, -20) * mouseDirVector;
            for (int i = 0; i < 5; i++)
            {
                bulletClone = Instantiate(bullet, bulletSpawnPosition, transform.rotation);
                bulletClone.GetComponent<Bullet>().InitialMove(initialAngle);
                initialAngle = Quaternion.Euler(0, 0, 10) * initialAngle;
            }
        }
    }
    private bool canShoot()
    {
        return (!GameManager.isPaused && isAiming && /*ammo > 0 &&*/ shootTimer <= 0);
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
