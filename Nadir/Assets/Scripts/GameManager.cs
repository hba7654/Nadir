using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

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
    [SerializeField] private float dopamineStart;
    [SerializeField] private float dopamineMax;
    public static float dopamine = 2;
    private static float dopamineDecreaseRate = 0.25f;
    private static float dopamineIncreaseRate = 1;
    private static float dopamineLimit;

    [Header("UI Objects")]
    [SerializeField] private Text dopamineText;
    [SerializeField] private Text healthText;


    // Start is called before the first frame update
    void Start()
    {
        dopamine = dopamineStart;
        dopamineLimit = dopamineMax;

        zombies = new ArrayList();

        isSpawning = false;
    }

    // Update is called once per frame
    void Update()
    {
        dopamineText.text = string.Format("Dopamine: {0:F1}", dopamine);
        healthText.text = "Health: " + playerObject.GetComponent<PlayerController>().health;

        if (!isSpawning && Mathf.FloorToInt(Time.time) % (zombieSpawnFrequency * 5 / Mathf.Floor(dopamine)) == 0)
        {
            Debug.Log("Time to Spawn");
            isSpawning = true;
            StartCoroutine(SpawnZombie());
        }

        if (dopamine > dopamineStart)
            dopamine -= (Time.deltaTime * dopamineDecreaseRate);
        else
            dopamine = dopamineStart;
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
