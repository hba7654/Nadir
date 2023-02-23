using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [Header("Global varibales")]
    [SerializeField] private Camera camera;
    [SerializeField] private GameObject zombieObject;
    [SerializeField] private GameObject playerObject;

    [Header("Zombie Spawning Variables")]
    public static ArrayList zombies;
    [SerializeField] private int maxZombieCount;
    [SerializeField] private int zombieSpawnFrequency;
    [SerializeField] private int zombiesToSpawnAtOnce;
    private bool isSpawning;

    [Header("Dopamine Variables")]
    public static float dopamine = 2;
    public static float dopamineDecreaseRate = 0.25f;
    public static float dopamineIncreaseRate = 1;
    public static float dopamineLimit = 20;

    // Start is called before the first frame update
    void Start()
    {
        zombies = new ArrayList();

        isSpawning = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (zombieSpawnFrequency * 5 < Mathf.FloorToInt(dopamine))
        {
            if (!isSpawning)
            {
                Debug.Log("Time to Spawn");
                isSpawning = true;
                StartCoroutine(SpawnZombie());
            }
        }
        else if(Mathf.FloorToInt(Time.time) % zombieSpawnFrequency * 5 < Mathf.FloorToInt(dopamine))
        {
            if (!isSpawning)
            {
                Debug.Log("Time to Spawn");
                isSpawning = true;
                StartCoroutine(SpawnZombie());
            }
        }

        if (dopamine > 2)
            dopamine -= (Time.deltaTime * dopamineDecreaseRate);
        else
            dopamine = 2;
    }

    public void DopamineTest(InputAction.CallbackContext context)
    {
        if(context.started)
            dopamine += context.ReadValue<float>();

        Debug.Log(dopamine);
    }

    public static void IncreaseDopamine()
    {
        if (dopamine < dopamineLimit)
        {
            dopamine += dopamineIncreaseRate;
        }
        else
        {
            dopamine = dopamineLimit;
        }
        
    }

    private IEnumerator SpawnZombie()
    {
        float height = camera.orthographicSize * 2;
        float width = height * camera.aspect;

        int numSpawned = 0;

        while (zombies.Count < (maxZombieCount + dopamine * 3) && numSpawned < (zombiesToSpawnAtOnce + dopamine))
        {
            int spawnIndex = Random.Range(0, 4);
            Vector2 spawnPos;
            switch (spawnIndex)
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

            numSpawned++;

            yield return new WaitForSeconds(zombieSpawnFrequency/dopamine);
        }

        isSpawning = false;
        
    }
}
