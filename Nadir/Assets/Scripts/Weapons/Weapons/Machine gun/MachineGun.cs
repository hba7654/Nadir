using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MachineGun : Weapon
{
    public override IEnumerator AltFire(GameObject player, Vector2 mouseDirVector, GameObject bullet)
    {
        //Play mg alt sound

        int tempMGAmmo = weaponData.ammo;
        weaponData.ammo = 100000;

        GameManager.dopamineDecreaseRate = 10.0f;
        GameManager.isLosingDopamine = true;
        GameManager.canGainDopamine = false;

        yield return new WaitForSeconds(weaponData.altFireTime);

        GameManager.dopamineDecreaseRate = GameManager.initDopamineDecreaseRate;
        GameManager.isLosingDopamine = false;
        GameManager.canGainDopamine = true;
        weaponData.ammo = tempMGAmmo;
    }
}
