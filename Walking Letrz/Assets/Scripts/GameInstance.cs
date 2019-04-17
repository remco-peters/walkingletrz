using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInstance : MonoBehaviour
{
    public static GameInstance instance = null;
    private int level = 0;
    public List<PlayerData> PlayerData = new List<PlayerData>();

    void Awake()
    {
        // check if instance exists
        if(instance == null)
        {
            instance = this;
        } else if(instance != this)
        {
            // Then destroy  this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameInstance.
            Destroy(gameObject);
        }

        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);
    }
    //public static int DifficultyOfGame { get; set; }
}
