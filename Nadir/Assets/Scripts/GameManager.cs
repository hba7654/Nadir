using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static float dopamine;
    // Start is called before the first frame update
    void Start()
    {
        dopamine = 5;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DopamineTest(InputAction.CallbackContext context)
    {
        if(context.started)
            dopamine += context.ReadValue<float>();

        Debug.Log(dopamine);
    }
}
