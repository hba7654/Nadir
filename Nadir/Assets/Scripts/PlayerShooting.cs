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
    [SerializeField] private GameObject crosshair;
    [SerializeField] private Sprite[] gunImages;

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
    private SpriteRenderer crosshairSprite;

    public enum Weapons { Pistol, Machinegun, Shotgun };
    // Start is called before the first frame update
    void Start()
    {
        shootTimer = 0;

        startMGAmmo = 60;
        startSGAmmo = 16;
        mgAmmo = startMGAmmo;
        sgAmmo = startSGAmmo;
        crosshairSprite = crosshair.GetComponent<SpriteRenderer>();
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
                    crosshairSprite.sprite = gunImages[0];
                    break;
                case Weapons.Machinegun:
                    fireRate = 2.5f;
                    bulletDamage = 8;
                    GameManager.dopamineIncreaseRate = 0.05f;
                    crosshairSprite.sprite = gunImages[1];
                    break;
                case Weapons.Shotgun:
                    fireRate = 0.125f;
                    bulletDamage = 4;
                    GameManager.dopamineIncreaseRate = 0.15f;
                    crosshairSprite.sprite = gunImages[2];
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
                    if (mgAmmo > 0)
                    {
                        StartCoroutine(ShootMG());
                    }
                    else
                    {
                        isShooting = false;
                    }
                    break;
                case Weapons.Shotgun:
                    if (sgAmmo > 0)
                    {
                        ShootSG();
                    }
                    else
                    {
                        isShooting = false;
                    }
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
            Debug.Log("STOP");
            isShooting = false;
        }
        if (context.started && canShoot())
        {
            isShooting = true;
            Debug.Log("SHOOT");
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

    private IEnumerator ShootMG()
    {
        //if (mgAmmo > 0)
        {
            mgAmmo--;
            shootTimer = 1 / fireRate;

            GameObject bulletClone;
            Vector2 bulletSpawnPosition;
            bulletSpawnPosition = (Vector2)transform.position + mouseDirVector / 2;
            bulletClone = Instantiate(bullet, bulletSpawnPosition, transform.rotation);
            bulletClone.GetComponent<Bullet>().InitialMove(mouseDirVector);

            yield return null;
        }
        //yield return null;
    }

    private void ShootSG()
    {
        //if(sgAmmo > 0)
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
        Vector2 spawnPos = (Vector2)transform.position + mouseDirVector / 2.5f;
        GameObject bulletSiblingClone = Instantiate(bulletSibling, spawnPos, Quaternion.identity);
        bulletSiblingClone.GetComponent<SiblingController>().playerObject = gameObject;
        GameManager.canGainDopamine = false;

        yield return new WaitForSeconds(altFireTime);

        GameManager.canGainDopamine = true;
        Destroy(bulletSiblingClone);
    }
    private IEnumerator MGAlt()
    {
        int tempMGAmmo = mgAmmo;
        mgAmmo = 100000;
        GameManager.canGainDopamine = false;

        yield return new WaitForSeconds(altFireTime);

        GameManager.canGainDopamine = true;
        mgAmmo = tempMGAmmo;
    }
    private IEnumerator SGAlt()
    {
        GameObject bulletClone;
        Vector2 bulletSpawnPosition;

        Vector2 initialAngle = mouseDirVector;
        GameManager.canGainDopamine = false;

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
        GameManager.canGainDopamine = true;

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
            Debug.Log("AIMING W CONTROLLER");
            mouseDirVector = context.ReadValue<Vector2>();
            //mouseDirVector.x = Mathf.Round(mouseDirVector.x * 50) / 50;
            //mouseDirVector.y = Mathf.Round(mouseDirVector.y * 50) / 50;

            isAiming = true;
            usingMouse = false;
            crosshair.SetActive(true);
            crosshair.transform.position = (Vector2)transform.position + mouseDirVector/2.5f;

        }
        //Mouse Controls
        else if (context.control.displayName == "Position")
        {
            Debug.Log("AIMING W MOUSE");
            isAiming = true;
            usingMouse = true;
            crosshair.SetActive(true);
            crosshair.transform.rotation = Quaternion.EulerRotation(0, 0, MathF.Tanh(mouseDirVector.y / mouseDirVector.x));
            crosshair.transform.position = (Vector2)transform.position + mouseDirVector/2.5f;
            if (mouseDirVector.x < 0)
            {
                crosshairSprite.flipX = false;
                crosshairSprite.flipY = false;
            }
            else
            {
                crosshairSprite.flipX = true;
                crosshairSprite.flipY = false;
            }
        }
        if (context.canceled)
        {
            Debug.Log("STOPPED AIMING W CONTROLLER");
            isAiming = false;
            crosshair.SetActive(false);
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
