using Assets.Scripts;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class HUD : MyMonoBehaviour
{
    public MyPlayer Player { get; set; }
    public LetterManager LetterManager { get; set; }
    public List<Player> PlayersList { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        Assert.IsNotNull(Player, "Player misses in HUDClass");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}