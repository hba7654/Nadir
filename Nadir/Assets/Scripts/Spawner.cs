using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    private void OnBecameVisible()
    {
        enabled = true;
        GameManager.zombieSpawnPoints.Add(transform.position);
    }

    private void OnBecameInvisible()
    {
        enabled = false;
        GameManager.zombieSpawnPoints.Remove(transform.position);
    }
}
