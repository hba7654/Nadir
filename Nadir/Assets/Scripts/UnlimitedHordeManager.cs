using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlimitedHordeManager : MonoBehaviour
{
    private GameManager gameManager;
    private float timer;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = this.GetComponent<GameManager>();
        timer = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        gameManager.UpdateText(gameManager.questText, string.Format("Time: {0:F2}s", timer));

    }
}
