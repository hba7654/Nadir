using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Cinemachine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Global varibales")]
    public static bool isPaused;
    [SerializeField] private Camera camera;
    [SerializeField] private GameObject regZombieObject;
    [SerializeField] private GameObject bruteZombieObject;
    public GameObject playerObject;

    [Header("Zombie Spawning Variables")]
    public static List<GameObject> zombies;
    public static List<Vector2> zombieSpawnPoints;
    [SerializeField] private int zombieSpawnRadius;
    public int maxZombieCount;
    public int zombieSpawnFrequency;
    [SerializeField] private int zombiesToSpawnAtOnce;
    [SerializeField] private int zombieDopamineMultiplier;
    private bool isSpawning;

    [Header("Dopamine Variables")]
    public float timeToStartLosingDopamine;
    public float dopamineStart;
    public float dopamineMax;
    public static float dopamineStartStatic;
    public static float dopamine = 2;
    public static float dopamineIncreaseRate;
    public static float dopamineDecreaseRate;
    public static float initDopamineDecreaseRate = 1.15f;
    private static float dopamineLimit;
    public static float timeSinceLastKill;
    private static float dopamineIncrease;
    private static float dopamineIncreaseLimit = 1;
    public static bool canGainDopamine;
    public static int zombieCounter;
    public static bool isLosingDopamine;
    public static bool reverseDopamine;


    [Header("UI Objects")]
    [SerializeField] private Text zombieCounterText;
    public Text questText;
    [SerializeField] private Text ammoText;
    [SerializeField] private Text dopEnhanceText;
    [SerializeField] private GameObject gunImage;
    [SerializeField] private GameObject panel;
    [SerializeField] private GameObject minimap;
    [SerializeField] private GameObject healthBar;
    [SerializeField] private GameObject dopamineBar;
    [SerializeField] private Sprite[] gunImages;
    private SpriteRenderer gunImageSprite;

    //Gamemode related variables/enum
    public enum GameMode { Story, Cranked, Friends, Infected, Unlimited};
    public static GameMode gameMode;

    //Scores to keep track of
    public static int kills;
    public static float timeElapsed;

    //MISC
    private PlayerShooting playerShooting;
    private CinemachineBrain camBrain;

    private void Awake()
    {
        reverseDopamine = false;

        kills = 0;
        timeElapsed = 0;
    }

    // Start is called before the first frame update
    void Start()
    {
        dopamine = dopamineStart;
        dopamineLimit = dopamineMax;
        dopamineDecreaseRate = initDopamineDecreaseRate;

        zombies = new List<GameObject>();
        zombieSpawnPoints = new List<Vector2>();

        isSpawning = false;

        isPaused = false;

        timeSinceLastKill = 0;
        dopamineIncrease = 0;

        playerShooting = playerObject.GetComponent<PlayerShooting>();

        zombieCounter = 0;

        panel.SetActive(false);
        minimap.SetActive(false);

        canGainDopamine = true;

        gunImageSprite = gunImage.GetComponent<SpriteRenderer>();

        camBrain = camera.GetComponent<CinemachineBrain>();

        isLosingDopamine = false;

        dopamineStartStatic = dopamineStart;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isPaused)
        {
            timeElapsed += Time.deltaTime;

            dopamineLimit = dopamineMax;
            panel.SetActive(false);
            minimap.SetActive(false);

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
            else if (dopamine >= 15)
            {
                UpdateText(dopEnhanceText, "Weapon Special Active");
            }
            else if (dopamine >= 10)
            {
                UpdateText(dopEnhanceText, "Damage Boost Active");
            }
            else
            {
                UpdateText(dopEnhanceText, "No Dopamine Enhancements");
            }

            gunImageSprite.sprite = gunImages[(int)playerShooting.weapon];


            zombieCounterText.text = string.Format("Kills {0}", zombieCounter); 
            

            //Conditions to spawn zombies
            if (!isSpawning && (zombies.Count == 0 || Mathf.FloorToInt(Time.time) % (zombieSpawnFrequency * 5 / Mathf.Floor(dopamine)) == 0))
            {
                //Debug.Log("Time to Spawn");
                isSpawning = true;
                StartCoroutine(SpawnZombie());
            }

            //Player starts losing dopamine after a certain amount of time after gaining it
            if (timeSinceLastKill > timeToStartLosingDopamine)
            {
                isLosingDopamine = true;
            }
            else
            {
                isLosingDopamine = false;
            }

            if(isLosingDopamine)
            {
                if (!reverseDopamine)
                {
                    if (dopamine > dopamineStart)
                        dopamine -= (Time.deltaTime * dopamineDecreaseRate);
                    else
                        dopamine = dopamineStart;
                }
                else
                {
                    if (dopamine < dopamineMax)
                        dopamine += (Time.deltaTime * dopamineDecreaseRate);
                    else
                        dopamine = dopamineMax;
                }
            }

            timeSinceLastKill += Time.deltaTime;

            //Change camera transition time based on dopamine
            camBrain.m_DefaultBlend.m_Time = 2 - (1.5f * dopamine / 20f);
        }
        else
        {
            panel.SetActive(true);
            minimap.SetActive(true);
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
        timeSinceLastKill = 100;
    }
    public static void IncreaseDopamine()
    {
        dopamineIncrease = dopamineIncreaseRate / timeSinceLastKill;
        if (canGainDopamine)
        {
            if (!reverseDopamine)
            {
                //Debug.Log("Dopamine Increase = " + dopamineIncrease);
                if (dopamineIncrease >= dopamineIncreaseLimit)
                {
                    //Debug.Log("Having a bit too much fun there, eh? Increase is now 2.5");
                    dopamineIncrease = dopamineIncreaseLimit;
                }

                if ((dopamine + dopamineIncrease) >= dopamineLimit)
                {
                    //Debug.Log("Way too high dope, set to max");
                    dopamine = dopamineLimit;
                }
                else
                {
                    //Debug.Log("Good, take your time, relax c:");
                    dopamine += dopamineIncrease;
                }
            } 
            else
            {
                //Debug.Log("Dopamine Increase = " + dopamineIncrease);
                if (dopamineIncrease >= dopamineIncreaseLimit)
                {
                    //Debug.Log("Having a bit too much fun there, eh? Increase is now 2.5");
                    dopamineIncrease = dopamineIncreaseLimit;
                }

                if ((dopamine - dopamineIncrease) <= dopamineStartStatic)
                {
                    //Debug.Log("Way too high dope, set to max");
                    dopamine = dopamineStartStatic;
                }
                else
                {
                    //Debug.Log("Good, take your time, relax c:");
                    dopamine -= dopamineIncrease;
                }
            }

            timeSinceLastKill = 0;
        }

       
    }

    private IEnumerator SpawnZombie()
    {
        int numSpawned = 0;

        while (zombies.Count < (maxZombieCount + dopamine * zombieDopamineMultiplier) && numSpawned < (zombiesToSpawnAtOnce + dopamine))
        {
            if (isPaused)
                yield return new WaitUntil(() => !isPaused);
            if (zombieSpawnPoints.Count > 0)
            {
                int spawnIndex = zombieSpawnPoints.Count > 1 ? Random.Range(0, zombieSpawnPoints.Count) : 0;
                Vector2 spawnPos = zombieSpawnPoints[spawnIndex];
                //Debug.Log("SpawnPoint: " + spawnPos);
                GameObject zombie;

                if(Random.Range(0,100) < 5)
                {
                     zombie = Instantiate(bruteZombieObject, spawnPos, Quaternion.identity);
                }
                else
                {
                     zombie = Instantiate(regZombieObject, spawnPos, Quaternion.identity);
                }

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
            Debug.Log("Panel is: " + panel);
            isPaused = !isPaused;
            panel.SetActive(!panel.activeInHierarchy);
            minimap.SetActive(!minimap.activeInHierarchy);
        }
    }

    public void UpdateText(Text text,string data)
    {
        text.text = data;
    }

    public static void Die()
    {
        SceneManager.LoadScene("End Screen");
    }
}
