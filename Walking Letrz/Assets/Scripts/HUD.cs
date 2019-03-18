using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class HUD : MonoBehaviour
{
    public MyPlayer Player { get; set; }
    // Start is called before the first frame update
    void Start()
    {
        Assert.IsNotNull(Player, "Player misses at row 12, HUDClass");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
