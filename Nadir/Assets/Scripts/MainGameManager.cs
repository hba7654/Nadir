using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainGameManager : MonoBehaviour
{
    [SerializeField] private bool reverseDopamine;

    private GameManager gameManager;
    private float timer;
    private CinemachineVirtualCamera mainCam, cave1Cam, cave2Cam, cave2InnerCam;
    private float mainCamSize, cave1CamSize, cave2CamSize, cave2InnerCamSize;

    public GameObject mainFloor;
    public GameObject caveOne;
    public GameObject caveTwo;
    public GameObject caveTwoInner;
    private static GameObject mainFloorS, caveOneS, caveTwoS, caveTwoInnerS;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = this.GetComponent<GameManager>();
        timer = 300.0f;

        mainFloor.SetActive(true);
        caveOne.SetActive(false);
        caveTwo.SetActive(false);
        caveTwoInner.SetActive(false);

        mainCam = mainFloor.GetComponent<CinemachineVirtualCamera>();
        cave1Cam = caveOne.GetComponent<CinemachineVirtualCamera>();
        cave2Cam = caveTwo.GetComponent<CinemachineVirtualCamera>();
        cave2InnerCam = caveTwoInner.GetComponent<CinemachineVirtualCamera>();

        mainCamSize = mainCam.m_Lens.OrthographicSize;
        cave1CamSize = cave1Cam.m_Lens.OrthographicSize;
        cave2CamSize = cave2Cam.m_Lens.OrthographicSize;
        cave2InnerCamSize = cave2InnerCam.m_Lens.OrthographicSize;

        mainFloorS = mainFloor;
        caveOneS = caveOne;
        caveTwoS = caveTwo;
        caveTwoInnerS = caveTwoInner;

        GameManager.reverseDopamine = reverseDopamine;
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.isPaused)
        {
            // Updates the quest and the quest text in the UI
            switch (gameManager.playerObject.GetComponent<PlayerManager>().questStep)
            {
                // Quest step 1
                case 1:
                    gameManager.questText.text = string.Format("Kill {0} zombies", 15 - GameManager.zombieCounter);
                    if (GameManager.zombieCounter >= 15)
                    {
                        gameManager.playerObject.GetComponent<PlayerManager>().questStep++;
                    }
                    break;

                // Quest step 2
                case 2:
                    gameManager.questText.text = string.Format("Find {0} bomb parts", 3 - gameManager.playerObject.GetComponent<PlayerManager>().bombParts);
                    break;

                // Quest step 3
                case 3:
                    gameManager.UpdateText(gameManager.questText, "Find the Zombie Kingdom");
                    gameManager.maxZombieCount = 25;
                    gameManager.dopamineMax = 20;
                    break;

                // Quest step 4
                case 4:
                    gameManager.UpdateText(gameManager.questText, "Find the Zombie Key. Hint nearby cave");
                    gameManager.maxZombieCount = 40;
                    gameManager.zombieSpawnFrequency = 4;
                    break;

                // Quest step 5
                case 5:
                    timer -= Time.deltaTime * GameManager.dopamine;
                    gameManager.UpdateText(gameManager.questText, string.Format("SURVIVE for {0:F2}", timer / gameManager.dopamineStart));

                    if (timer < 0)
                    {
                        gameManager.playerObject.GetComponent<PlayerManager>().mountainTileSet.SetActive(false);
                        gameManager.playerObject.GetComponent<PlayerManager>().questStep++;
                    }

                    break;

                // Quest step 6
                case 6:
                    gameManager.UpdateText(gameManager.questText, "Use the key on the gates of the Zombie Kingdom. Hint Top of the map");
                    break;

                // Quest step 6
                case 7:
                    gameManager.UpdateText(gameManager.questText, "ANDDD THATS ALL FOLKS... GOOD LUCK SURVIVING");
                    gameManager.maxZombieCount = 60;
                    gameManager.zombieSpawnFrequency = 3;
                    break;
            }


            mainCam.m_Lens.OrthographicSize = mainCamSize - 1 + GameManager.dopamine / 5;
            cave1Cam.m_Lens.OrthographicSize = cave1CamSize - 1 + GameManager.dopamine / 5;
            cave2Cam.m_Lens.OrthographicSize = cave2CamSize - 1 + GameManager.dopamine / 5;
            cave2InnerCam.m_Lens.OrthographicSize = cave2InnerCamSize - 1 + GameManager.dopamine / 5;
        }
    }

    public static void ChangeToCam(string name)
    {
        switch (name)
        {
            case "main":
                mainFloorS.SetActive(true);
                caveOneS.SetActive(false);
                caveTwoS.SetActive(false);
                caveTwoInnerS.SetActive(false);
                break;
            case "cave 1":
                mainFloorS.SetActive(false);
                caveOneS.SetActive(true);
                break;
            case "cave 2":
                mainFloorS.SetActive(false);
                caveTwoInnerS.SetActive(false);
                caveTwoS.SetActive(true);
                break;
            case "cave 2 inner":
                caveTwoS.SetActive(false);
                caveTwoInnerS.SetActive(true);
                break;
        }
    }
}