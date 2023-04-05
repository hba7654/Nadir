using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieKingdom : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnBecameVisible()
    {
        enabled = true;
        Debug.Log("ZOMBIE KINGDOM");
        GameManager.zombieSpawnPoints.Add(transform.position);
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
