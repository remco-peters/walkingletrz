using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultySceneScript : MonoBehaviour
{
    public GameObject medium;
    public GameObject hard;

    // Start is called before the first frame update
    void Start()
    {
        if(GameInstance.instance.IsMultiplayer)
        {
            medium.SetActive(false);
            hard.SetActive(false);
        }
    }
}
