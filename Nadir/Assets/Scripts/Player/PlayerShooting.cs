using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooting : MonoBehaviour
{
    [SerializeField] private GameObject bullet;
    [SerializeField] private GameObject bulletSibling;
    [SerializeField] public List<Weapon> weapons;
    [SerializeField] private float altFireTime;
    [SerializeField] private GameObject crosshair;
    [SerializeField] private Sprite[] gunImages;
    [SerializeField] private Sprite[] bulletImages;

    private int equippedWeapon;

    private bool isShooting;
    private bool alting;
    private bool isAiming;
    private bool usingMouse;
    private Vector2 mouseDirVector;
    private Vector2 mousePosition;
    private float shootTimer;

    // Start is called before the first frame update
    void Start()
    {
        shootTimer = 0;
        alting = false;

        equippedWeapon = 0;
        weapons[0].SetWeapon();

        GameManager.dopamineIncreaseRate = 0.25f;

    }

    // Update is called once per frame
    void Update()
    {
        if(!GameManager.isPaused)
        {
            shootTimer -= (Time.deltaTime * GameManager.dopamine);

            if (usingMouse)
            {
                {
                    mousePosition = GetMousePosition();
                    mouseDirVector = GetMouseVector();
                }
            }
        }

    }

    private void FixedUpdate()
    {
        if (isShooting && canShoot())
        {
            shootTimer = 1 / weapons[equippedWeapon].weaponData.fireRate;
            weapons[equippedWeapon].Shoot(gameObject, bullet, mouseDirVector);
        }
    }

    public void NextWeapon(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            equippedWeapon = (equippedWeapon + 1) % weapons.Count;
            weapons[equippedWeapon].SetWeapon();
        }
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

    public void AltFire()
    {
        if (!alting && GameManager.dopamine >= 20)
        {
            alting = true;
            StartCoroutine(DoAltFire());
        }
    }

    private IEnumerator DoAltFire()
    {
            GameManager.DepleteDopamine();
            StartCoroutine(weapons[equippedWeapon].AltFire(gameObject, mouseDirVector, bullet));
            yield return new WaitForSeconds(0.5f);
            alting = false;
    }

    private bool canShoot()
    {
        return (!GameManager.isPaused && isAiming && shootTimer <= 0);
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
            crosshair.SetActive(true);
            crosshair.transform.position = (Vector2)transform.position + mouseDirVector/2.5f;

        }
        //Mouse Controls
        else if (context.control.displayName == "Position")
        {
            isAiming = true;
            usingMouse = true;
        }
        if (context.canceled)
        {
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
        //TODO
        if (collision.tag == "Ammo")
        {
            //Play ammo pickup sound
            SoundManager.PlaySound("ammo");

            for(int i = 0; i < weapons.Count; i++)
            {
                weapons[i].weaponData.ammo += weapons[i].weaponData.ammoInAmmoPack;
            }

            Destroy(collision.gameObject);
        }
        else if(collision.tag == "GunPickup")
        {
            weapons.Add(collision.GetComponent<Weapon>());
            Destroy(collision.gameObject);
        }
    }
}
