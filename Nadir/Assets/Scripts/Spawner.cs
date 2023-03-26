using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    private void OnBecameVisible()
    {
        enabled = true;
        Debug.Log("FOUND SPWNER");
        GameManager.zombieSpawnPoints.Add(transform.position);
    }

    private void OnBecameInvisible()
    {
        enabled = false;
        Debug.Log("LOST SPWNER");
        GameManager.zombieSpawnPoints.Remove(transform.position);
    }
}
