using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static float dopamine;
    public static ArrayList zombies;

    [SerializeField] private int maxZombieCount;
    [SerializeField] private int zombieSpawnFrequency;
    [SerializeField] private Camera camera;
    [SerializeField] private GameObject zombieObject;
    [SerializeField] private GameObject playerObject;

    // Start is called before the first frame update
    void Start()
    {
        dopamine = 5;

        zombies = new ArrayList();
    }

    // Update is called once per frame
    void Update()
    {
        if(zombies.Count < maxZombieCount && Mathf.FloorToInt(Time.time) % zombieSpawnFrequency == 0)
        {
            SpawnZombie();
        }
    }

    public void DopamineTest(InputAction.CallbackContext context)
    {
        if(context.started)
            dopamine += context.ReadValue<float>();

        Debug.Log(dopamine);
    }

    private void SpawnZombie()
    {
        float height = camera.orthographicSize * 2;
        float width = height * camera.aspect;
        int spawnIndex = Random.Range(0, 4);
        Vector2 spawnPos;
        switch(spawnIndex)
        {
            case 0:
                spawnPos = new Vector2(camera.transform.position.x + Random.Range(-width, width), camera.transform.position.y + height);
                break;

            case 1:
                spawnPos = new Vector2(camera.transform.position.x + Random.Range(-width, width), camera.transform.position.y - height);
                break;

            case 2:
                spawnPos = new Vector2(camera.transform.position.x + width, camera.transform.position.y + Random.Range(-height, height));
                break;

            default:
                spawnPos = new Vector2(camera.transform.position.x - width, camera.transform.position.y + Random.Range(-height, height));
                break;

        }
        GameObject zombie = Instantiate(zombieObject, spawnPos, Quaternion.identity);
        zombie.GetComponent<ZombieController>().playerObject = playerObject;
        zombies.Add(zombie);
        
    }
}
