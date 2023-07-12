using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Shotgun : Weapon
{

    public override IEnumerator AltFire(GameObject player, Vector2 mouseDirVector, GameObject bullet)
    {
        //Play sg alt sound

        GameObject bulletClone;
        Vector2 bulletSpawnPosition;

        Vector2 initialAngle = mouseDirVector;

        GameManager.dopamineDecreaseRate = 10.0f;
        GameManager.isLosingDopamine = true;
        GameManager.canGainDopamine = false;

        for (int j = 0; j < 5; j++)
        {
            bulletSpawnPosition = (Vector2)player.transform.position + mouseDirVector / 2;
            for (int i = 0; i < 72; i++)
            {
                bulletClone = Instantiate(bullet, bulletSpawnPosition, transform.rotation);
                bulletClone.GetComponent<Bullet>().damage = weaponData.bulletDamage;
                bulletClone.GetComponent<Bullet>().speedMultiplier = weaponData.bulletSpeedMultiplier;
                bulletClone.GetComponent<Bullet>().InitialMove(initialAngle);
                initialAngle = Quaternion.Euler(0, 0, 5) * initialAngle;
            }
            yield return new WaitForSeconds(0.8f);
        }
        GameManager.dopamineDecreaseRate = GameManager.initDopamineDecreaseRate;
        GameManager.isLosingDopamine = false;
        GameManager.canGainDopamine = true;

        yield return null;
    }
}
