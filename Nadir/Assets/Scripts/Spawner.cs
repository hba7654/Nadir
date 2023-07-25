using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private bool inCave;
    private void OnBecameVisible()
    {
        enabled = true;
        GameManager.AddToList(transform.position, inCave);
    }

    private void OnBecameInvisible()
    {
        enabled = false;
        GameManager.RemoveFromList(transform.position, inCave);
    }
}
