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
    private static float dopamineStartStatic;
    public static float dopamineIncreaseRate;
    private static float dopamineDecreaseRate = 1.5f;
    private static float dopamineLimit;
    private static float timeSinceLastKill;
    private static float dopamineIncrease;
    private static float dopamineIncreaseLimit = 1;
    private static int zombieCounter; 

    [Header("UI Objects")]
    [SerializeField] private Text zombieCounterText;
    [SerializeField] private Text questText;
    [SerializeField] private Text ammoText;
    [SerializeField] private Text dopEnhanceText;
    [SerializeField] private GameObject gunImage;
    [SerializeField] private GameObject panel;

    [SerializeField] private GameObject healthBar;
    [SerializeField] private GameObject dopamineBar;

    [SerializeField] private Sprite[] gunImages;


    private float timer;

    private PlayerShooting playerShooting;


    // Start is called before the first frame update
    void Start()
    {
        dopamine = dopamineStart;
        dopamineStartStatic = dopamineStart;
        dopamineLimit = dopamineMax;

        zombies = new List<GameObject>();
        zombieSpawnPoints = new List<Vector2>();

        isSpawning = false;

        isPaused = false;

        timeSinceLastKill = 0;
        dopamineIncrease = 0;

        playerShooting = playerObject.GetComponent<PlayerShooting>();

        zombieCounter = 0;
        timer = 300.0f;

        panel.SetActive(false);
        //FindNearestSpawns();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isPaused)
        {
            panel.SetActive(false);

            dopamineBar.transform.localScale = new Vector3((dopamine - 5) / 2.173f, 1, 1);
            healthBar.transform.localScale = new Vector3(playerObject.GetComponent<PlayerManager>().health/2.87f, 1, 1);

            // Updates the ammo count in the UI
            switch (playerShooting.weapon)
            {
                case PlayerShooting.Weapons.Pistol:
                    UpdateText(ammoText, "\u221E");

                    if (dopamine > 15 && dopamine < 20)
                    {
                        UpdateText(dopEnhanceText, "Pistol Enhanced");
                    }
                    break;

                case PlayerShooting.Weapons.Machinegun:
                    UpdateText(ammoText, playerShooting.mgAmmo.ToString());
                    if (dopamine > 15 && dopamine < 20)
                    {
                        UpdateText(dopEnhanceText, "Ammo packs give double ammo");
                    }
                    break;

                case PlayerShooting.Weapons.Shotgun:
                    UpdateText(ammoText, playerShooting.sgAmmo.ToString());
                    if (dopamine > 15 && dopamine < 20)
                    {
                        UpdateText(dopEnhanceText, "Tighter bullet spread");
                    }
                    break;

            }

            if (dopamine >= 20)
            {
                UpdateText(dopEnhanceText, "Alternate Fire ready!");
            }
            else if (dopamine >= 10 && dopamine < 15)
            {
                UpdateText(dopEnhanceText, "Damage boost active");
            }
            else if (dopamine < 10) 
            {
                UpdateText(dopEnhanceText, "No Dopamine Enhancements");
            }

            gunImage.GetComponent<SpriteRenderer>().sprite = gunImages[(int)playerShooting.weapon];

            // Updates the quest and the quest text in the UI
            switch (playerObject.GetComponent<PlayerManager>().questStep)
            {
                // Quest step 1
                case 1:
                    questText.text = string.Format("Kill {0} zombies", 15 - zombieCounter);
                    if (zombieCounter >= 15)
                    {
                        playerObject.GetComponent<PlayerManager>().questStep++;
                    }
                    break;

                // Quest step 2
                case 2:
                    questText.text = string.Format("Find {0} bomb parts", 3 - playerObject.GetComponent<PlayerManager>().bombParts);
                    break;

                // Quest step 3
                case 3:
                    UpdateText(questText, "Find the Zombie Kingdom");
                    maxZombieCount = 25;
                    dopamineMax = 20;
                    dopamineLimit = dopamineMax;
                    break;

                // Quest step 4
                case 4:
                    UpdateText(questText, "Find the Zombie Key. Hint nearby cave");
                    maxZombieCount = 40;
                    zombieSpawnFrequency = 4;
                    break;

                // Quest step 5
                case 5:
                    timer -= Time.deltaTime * dopamine;
                    UpdateText(questText, string.Format("SURVIVE for {0:F2}", timer / dopamineStart));
                    
                    if(timer < 0)
                    {
                        playerObject.GetComponent<PlayerManager>().mountainTileSet.SetActive(false);
                        playerObject.GetComponent<PlayerManager>().questStep++;
                    }

                    break;

                // Quest step 6
                case 6:
                    UpdateText(questText, "Use the key on the gates of the Zombie Kingdom. Hint Top of the map");
                    break;

                // Quest step 6
                case 7:
                    UpdateText(questText, "ANDDD THATS ALL FOLKS... GOOD LUCK SURVIVING");
                    maxZombieCount = 60;
                    zombieSpawnFrequency = 3;
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
    }

    public void DopamineTest(InputAction.CallbackContext context)
    {
        if(context.started)
            dopamine += context.ReadValue<float>();

        Debug.Log(dopamine);
    }

    public static void DepleteDopamine()
    {
        dopamine = dopamineStartStatic;
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

    public void Pause(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            isPaused = !isPaused;
            panel.SetActive(!panel.activeInHierarchy);
        }
    }

    private void UpdateText(Text text,string data)
    {
        text.text = data;
    }
}
