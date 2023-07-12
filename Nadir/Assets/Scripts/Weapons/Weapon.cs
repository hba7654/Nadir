using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour 
{
    public WeaponData weaponData;
    protected int damage;
    protected float bulletSpeedMult;

    public void Shoot(GameObject player, GameObject bullet, Vector2 mouseDirVector)
    {
        if ((!weaponData.infinteAmmo && weaponData.ammo > 0) || weaponData.infinteAmmo)
        {
            weaponData.ammo--;


            SoundManager.PlaySound("pistol_shoot");

            GameObject bulletClone;
            Vector2 bulletSpawnPosition;
            bulletSpawnPosition = (Vector2)player.transform.position + mouseDirVector / 2;

            SpriteRenderer bulletSprite;
            Quaternion spriteRotation = Quaternion.EulerRotation(0, 0, MathF.Tanh(mouseDirVector.y / mouseDirVector.x));

            float spreadMult = (GameManager.dopamine >= 15) ? 1.5f : 1;
            Vector2 initialAngle = Quaternion.Euler(0, 0, -2 * weaponData.angleOfSpread * spreadMult) * mouseDirVector;
            for (int i = 0; i < weaponData.projectilesPerShot; i++)
            {
                bulletClone = Instantiate(bullet, bulletSpawnPosition, spriteRotation);
                bulletSprite = bulletClone.GetComponent<SpriteRenderer>();
                bulletClone.GetComponent<Bullet>().damage = weaponData.bulletDamage;
                bulletClone.GetComponent<Bullet>().speedMultiplier = weaponData.bulletSpeedMultiplier;
                if (mouseDirVector.x < 0)
                {
                    bulletSprite.flipX = true;
                    bulletSprite.flipY = false;
                }
                else
                {
                    bulletSprite.flipX = false;
                    bulletSprite.flipY = false;
                }
                bulletClone.GetComponent<Bullet>().InitialMove(initialAngle);
                initialAngle = Quaternion.Euler(0, 0, weaponData.angleOfSpread * spreadMult) * initialAngle;
            }
        }
    }

    public virtual IEnumerator AltFire(GameObject player, Vector2 mouseDirVector, GameObject bullet)
    {
        Debug.Log("ALT NO WORK");
        yield return null;  
    }

    public void SetWeapon()
    {
        damage = weaponData.bulletDamage;
        bulletSpeedMult = weaponData.bulletSpeedMultiplier;

        GameManager.dopamineIncreaseRate = weaponData.dopamineIncreaseOnKill;
    }

}
