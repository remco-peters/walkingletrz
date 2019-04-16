using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchResultScript : MonoBehaviour
{
    public GameObject FirstPlayerPanel;
    public GameObject SecondPlayerPanel;
    public GameObject ThirdPlayerPanel;
    public GameObject FourthPlayerPanel;
    static public Player MyPlayer { get; set; }
    static public List<Player> PlayersList { get; set; }
    
    void Awake()
    {
        // Put all the stuff nicely on their place!
        switch(PlayersList.Count)
        {
            case 1:
                Destroy(SecondPlayerPanel);
                Destroy(ThirdPlayerPanel);
                Destroy(FourthPlayerPanel);
                break;
            case 2:
                Destroy(ThirdPlayerPanel);
                Destroy(FourthPlayerPanel);
                break;
            case 3:
                Destroy(FourthPlayerPanel);
                break;
            default:
                break;
        }
        Debug.Log(PlayersList.Count);
    }
    
}
