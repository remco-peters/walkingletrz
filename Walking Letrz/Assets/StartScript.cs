using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartScript : MonoBehaviour
{
    public GameInstance gameInstance;

    // Start is called before the first frame update
    void Awake()
    {
        if (GameInstance.instance == null)
        {
            //Instantiate gameManager prefab
            Instantiate(gameInstance);
        }
    }
}
