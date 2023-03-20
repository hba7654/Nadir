using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Global varibales")]
    public static bool isPaused;
    [SerializeField] private Camera camera;
    [SerializeField] private GameObject zombieObject;
    [SerializeField] private GameObject playerObject;

    [Header("Zombie Spawning Variables")]
    public static List<GameObject> zombies;
    public static List<Vector2> zombieSpawnPoints;
    [SerializeField] private int zombieSpawnRadius;
    [SerializeField] private int maxZombieCount;
    [SerializeField] private int zombieSpawnFrequency;
    [SerializeField] private int zombiesToSpawnAtOnce;
    [SerializeField] private int zombieDopamineMultiplier;
    private bool isSpawning;

    [Header("Dopamine Variables")]
    [SerializeField] private float timeToStartLosingDopamine;
    [SerializeField] private float dopamineStart;
    [SerializeField] private float dopamineMax;
    public static float dopamine = 2;
    private static float dopamineDecreaseRate = 0.25f;
    private static float dopamineIncreaseRate = 1;
    private static float dopamineLimit;
    private static float timeSinceLastKill;
    private static float dopamineIncrease;

    [Header("UI Objects")]
    [SerializeField] private Text dopamineText;
    [SerializeField] private Text healthText;


    // Start is called before the first frame update
    void Start()
    {
        dopamine = dopamineStart;
        dopamineLimit = dopamineMax;

        zombies = new List<GameObject>();
        zombieSpawnPoints = new List<Vector2>();

        isSpawning = false;

        timeSinceLastKill = 0;
        dopamineIncrease = 0;

        //FindNearestSpawns();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isPaused)
        {
            dopamineText.text = string.Format("Dopamine: {0:F1}", dopamine);
            healthText.text = "Health: " + playerObject.GetComponent<PlayerController>().health;

            if (!isSpawning && (zombies.Count == 0 || Mathf.FloorToInt(Time.time) % (zombieSpawnFrequency * 5 / Mathf.Floor(dopamine)) == 0))
            {
                Debug.Log("Time to Spawn");
                isSpawning = true;
                //FindNearestSpawns();
                StartCoroutine(SpawnZombie());
            }

            //Player starts losing dopamine after a certain amount of time after gaining it
            if (timeSinceLastKill > timeToStartLosingDopamine)
            {
                if (dopamine > dopamineStart)
                    dopamine -= (Time.deltaTime * dopamineDecreaseRate);
                else
                    dopamine = dopamineStart;
            }

            timeSinceLastKill += Time.deltaTime;
        }
    }

    public void DopamineTest(InputAction.CallbackContext context)
    {
        if(context.started)
            dopamine += context.ReadValue<float>();

        Debug.Log(dopamine);
    }

    public static void IncreaseDopamine()
    {
        dopamineIncrease = 1 / timeSinceLastKill;

        if ((dopamine + dopamineIncrease) <= dopamineLimit)
        {
            dopamine += dopamineIncrease;
        }
        else
        {
            dopamine = dopamineLimit;
        }

        timeSinceLastKill = 0;
        
    }

    //private void FindNearestSpawns()
    //{
    //    Collider2D[] collisions = Physics2D.OverlapCircleAll(playerObject.transform.position, zombieSpawnRadius, LayerMask.NameToLayer("ZombieSpawns"));
    //    Debug.Log(collisions.Length);
    //    for (int i = 0; i < collisions.Length; i++)
    //    {
    //        closestZombieSpawnPoints.Add(collisions[i].transform.position);
    //        //Debug.Log("Spawn point " + i + ": " + closestZombieSpawnPoints[i]);
    //    }
    //}

    private IEnumerator SpawnZombie()
    {
        int numSpawned = 0;

        while (zombies.Count < (maxZombieCount + dopamine * zombieDopamineMultiplier) && numSpawned < (zombiesToSpawnAtOnce + dopamine))
        {
            if (zombieSpawnPoints.Count > 0)
            {
                int spawnIndex = Random.Range(0, zombieSpawnPoints.Count);
                Vector2 spawnPos = zombieSpawnPoints[spawnIndex];
                //Debug.Log("SpawnPoint: " + spawnPos);

                GameObject zombie = Instantiate(zombieObject, spawnPos, Quaternion.identity);
                zombie.GetComponent<ZombieController>().playerObject = playerObject;
                zombies.Add(zombie);

                numSpawned++;

                yield return new WaitForSeconds(zombieSpawnFrequency / dopamine);
            }
            else
            {
                break;
            }
        }

        zombieSpawnPoints.Clear();


        isSpawning = false;
        
    }

    public static void Pause(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            isPaused = !isPaused;
        }
    }
}
