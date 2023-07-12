using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Pistol : Weapon
{
    [SerializeField] private GameObject altClone;

    public override IEnumerator AltFire(GameObject player, Vector2 mouseDirVector, GameObject bullet)
    {
        GameManager.dopamineDecreaseRate = 10.0f;
        GameManager.isLosingDopamine = true;
        GameManager.canGainDopamine = false;

        //yield return new WaitForSeconds(0.05f);
        GameObject bulletClone = null;

        //Play pistol alt sound

        Vector2 spawnPos = (Vector2)transform.position + mouseDirVector / 2.5f;
        bulletClone = Instantiate(altClone, spawnPos, Quaternion.identity);
        bulletClone.GetComponent<SiblingController>().playerObject = gameObject;


        yield return new WaitForSeconds(weaponData.altFireTime);

        GameManager.dopamineDecreaseRate = GameManager.initDopamineDecreaseRate;
        GameManager.isLosingDopamine = false;
        GameManager.canGainDopamine = true;
        Destroy(bulletClone);
    }
}
