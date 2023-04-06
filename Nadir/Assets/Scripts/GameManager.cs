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
    public static float dopamineIncreaseRate;
    private static float dopamineDecreaseRate = 0.75f;
    private static float dopamineLimit;
    private static float timeSinceLastKill;
    private static float dopamineIncrease;
    private static float dopamineIncreaseLimit = 2.5f;
    private static int zombieCounter = 0; 

    [Header("UI Objects")]
    [SerializeField] private Text dopamineText;
    [SerializeField] private Text healthText;
    [SerializeField] private Text zombieCounterText;
    [SerializeField] private Text questText;
    [SerializeField] private Text ammoText;
    [SerializeField] private GameObject gunImage;
    [SerializeField] private GameObject panel;

    [SerializeField] private Sprite[] gunImages;


    private float timer = 300.0f;

    private PlayerShooting playerShooting;


    // Start is called before the first frame update
    void Start()
    {
        dopamine = dopamineStart;
        dopamineLimit = dopamineMax;

        zombies = new List<GameObject>();
        zombieSpawnPoints = new List<Vector2>();

        isSpawning = false;

        isPaused = false;

        timeSinceLastKill = 0;
        dopamineIncrease = 0;

        playerShooting = playerObject.GetComponent<PlayerShooting>();

        //FindNearestSpawns();
    }

    // Update is called once per frame
    void Update()
    {
            Debug.Log("FPS: " + 1 / Time.deltaTime);
        if (!isPaused)
        {
            panel.SetActive(false);

            dopamineText.text = string.Format("Dopamine {0:F1}", dopamine);
            healthText.text = "Health " + playerObject.GetComponent<PlayerManager>().health;

            // Updates the ammo count in the UI
            switch (playerShooting.weapon)
            {
                case PlayerShooting.Weapons.Pistol:
                    UpdateText(ammoText, "\u221E");
                    break;

                case PlayerShooting.Weapons.Machinegun:
                    UpdateText(ammoText, playerShooting.mgAmmo.ToString());
                    break;

                case PlayerShooting.Weapons.Shotgun:
                    UpdateText(ammoText, playerShooting.sgAmmo.ToString());
                    break;

            }

            gunImage.GetComponent<SpriteRenderer>().sprite = gunImages[(int)playerShooting.weapon];

            // Updates the quest and the quest text in the UI
            switch (playerObject.GetComponent<PlayerManager>().questStep)
            {
                // Quest step 1
                case 1:
                    if (zombieCounter >= 15)
                    {
                        playerObject.GetComponent<PlayerManager>().questStep++;
                    }
                    break;

                // Quest step 2
                case 2:
                    UpdateText(questText, "Find 3 Bomb Parts");
                    break;

                // Quest step 3
                case 3:
                    UpdateText(questText, "Find the Zombie Kingdom");
                    break;

                // Quest step 4
                case 4:
                    UpdateText(questText, "Find the Zombie Key (Hint: nearby cave)");
                    break;

                // Quest step 5
                case 5:
                    timer -= Time.deltaTime * dopamine;
                    UpdateText(questText, string.Format("SURVIVE for {0:F2}", timer));
                    
                    if(timer < 0)
                    {
                        playerObject.GetComponent<PlayerManager>().mountainTileSet.SetActive(false);
                        playerObject.GetComponent<PlayerManager>().questStep++;
                    }

                    break;

                // Quest step 6
                case 6:
                    UpdateText(questText, "Use the key on the gates of the Zombie Kingdom");
                    break;

                // Quest step 6
                case 7:
                    UpdateText(questText, "ANDDD THATS ALL FOLKS");
                    break;
            }


            zombieCounterText.text = string.Format("Kills {0}", zombieCounter); 
            

            if (!isSpawning && (zombies.Count == 0 || Mathf.FloorToInt(Time.time) % (zombieSpawnFrequency * 5 / Mathf.Floor(dopamine)) == 0))
            {
                //Debug.Log("Time to Spawn");
                isSpawning = true;
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

        if (isPaused)
        {
            panel.SetActive(true);
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
        dopamineIncrease = dopamineIncreaseRate / timeSinceLastKill;
        Debug.Log("Dopamine Increase = " + dopamineIncrease);
        if (dopamineIncrease >= dopamineIncreaseLimit)
        {
            Debug.Log("Having a bit too much fun there, eh? Increase is now 2.5");
            dopamineIncrease = dopamineIncreaseLimit;
        }

        if ((dopamine + dopamineIncrease) >= dopamineLimit)
        {
            Debug.Log("Way too high dope, set to max");
            dopamine = dopamineLimit;
        }
        else
        {
            Debug.Log("Good, take your time, relax c:");
            dopamine += dopamineIncrease;
        }

        timeSinceLastKill = 0;

        // For Quest step 1
        zombieCounter++;

       
    }

    private IEnumerator SpawnZombie()
    {
        int numSpawned = 0;

        while (zombies.Count < (maxZombieCount + dopamine * zombieDopamineMultiplier) && numSpawned < (zombiesToSpawnAtOnce + dopamine))
        {
            if (zombieSpawnPoints.Count > 0)
            {
                int spawnIndex = zombieSpawnPoints.Count > 1 ? Random.Range(0, zombieSpawnPoints.Count) : 0;
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

    private void UpdateText(Text text,string data)
    {
        text.text = data;
    }
}
