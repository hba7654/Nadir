using System;
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
    [SerializeField] private GameObject bulletSibling;
    [SerializeField] public Weapons weapon;
    [SerializeField] private float altFireTime;

    private int startMGAmmo;
    private int startSGAmmo;
    private bool isShooting;
    private float fireRate;
    private bool isAiming;
    private bool usingMouse;
    private Vector2 mouseDirVector;
    private Vector2 mousePosition;
    private float shootTimer;
    private float sgSpread;

    public enum Weapons { Pistol, Machinegun, Shotgun };
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
                    fireRate = 0.5f;
                    bulletDamage = 5;
                    GameManager.dopamineIncreaseRate = 0.25f;
                    break;
                case Weapons.Machinegun:
                    fireRate = 2.5f;
                    bulletDamage = 8;
                    GameManager.dopamineIncreaseRate = 0.05f;
                    break;
                case Weapons.Shotgun:
                    fireRate = 0.125f;
                    bulletDamage = 4;
                    GameManager.dopamineIncreaseRate = 0.15f;
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
        while (isShooting && canShoot())
        {
            switch (weapon)
            {
                case Weapons.Pistol:
                    StartCoroutine(ShootBullet());
                    break;
                case Weapons.Machinegun:
                    mgAmmo--;
                    if(mgAmmo > 0)
                        StartCoroutine(ShootBullet());
                    break;
                case Weapons.Shotgun:
                    ShootSG();
                    break;
            }
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
            isShooting = false;
        }
        if (context.started && canShoot())
        {
            isShooting = true;
        }
    }


    private IEnumerator ShootBullet()
    {
        shootTimer = 1 / fireRate;
        GameObject bulletClone;
        Vector2 bulletSpawnPosition;
        bulletSpawnPosition = (Vector2)transform.position + mouseDirVector / 2;
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
            bulletSpawnPosition = (Vector2)transform.position + mouseDirVector / 2;
            if (GameManager.dopamine >= 15)
                sgSpread = 5;
            else
                sgSpread = 7.5f;

            Vector2 initialAngle = Quaternion.Euler(0, 0, -2 *sgSpread) * mouseDirVector;
            for (int i = 0; i < 5; i++)
            {
                bulletClone = Instantiate(bullet, bulletSpawnPosition, transform.rotation);
                bulletClone.GetComponent<Bullet>().InitialMove(initialAngle);
                initialAngle = Quaternion.Euler(0, 0, sgSpread) * initialAngle;
            }
        }
    }

    public void AltFire()
    {
        if (GameManager.dopamine >= 20)
        {
            GameManager.DepleteDopamine();
            switch (weapon)
            {
                case Weapons.Pistol:
                    StartCoroutine(PistolAlt());
                    break;
                case Weapons.Machinegun:
                    StartCoroutine(MGAlt());
                    break;
                case Weapons.Shotgun:
                    StartCoroutine(SGAlt());
                    break;
            }
        }
    }

    private IEnumerator PistolAlt()
    {
        GameObject bulletSiblingClone;
        Vector2 spawnPos = (Vector2)transform.position + mouseDirVector / 2;
        bulletSiblingClone = Instantiate(bulletSibling, spawnPos, Quaternion.identity);

        yield return new WaitForSeconds(altFireTime);

        Destroy(bulletSiblingClone);
    }
    private IEnumerator MGAlt()
    {
        int tempMGAmmo = mgAmmo;
        mgAmmo = 100000;

        yield return new WaitForSeconds(altFireTime);

        mgAmmo = tempMGAmmo;
    }
    private IEnumerator SGAlt()
    {
        GameObject bulletClone;
        Vector2 bulletSpawnPosition;

        Vector2 initialAngle = mouseDirVector;

        for (int j = 0; j < 5; j++)
        {
            bulletSpawnPosition = (Vector2)transform.position + mouseDirVector / 2;
            for (int i = 0; i < 72; i++)
            {
                bulletClone = Instantiate(bullet, bulletSpawnPosition, transform.rotation);
                bulletClone.GetComponent<Bullet>().InitialMove(initialAngle);
                initialAngle = Quaternion.Euler(0, 0, 5) * initialAngle;
            }
            yield return new WaitForSeconds(0.8f);
        }

        yield return null;
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
            if (GameManager.dopamine >= 15)
                mgAmmo += 40;
            else
                mgAmmo += 15;
            Destroy(collision.gameObject);
        }
    }
}
