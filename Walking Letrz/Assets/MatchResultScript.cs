using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchResultScript : MonoBehaviour
{
    public GameObject PlayerPanelHolder;
    public PlayerPanel PlayerPanelClass;
    
    void Awake()
    {
        // Put all the stuff nicely on their place!
        for (int i = 0; i < GameInstance.instance.PlayerData.Count; i++)
        {
            SetUpInfo(GameInstance.instance.PlayerData[i]);
        }
    }
    
    private void SetUpInfo(PlayerData p)
    {
        PlayerPanel pp = Instantiate(PlayerPanelClass);
        pp.GetComponent<PlayerPanel>().playerName.text = p.Name;
        pp.GetComponent<PlayerPanel>().playerScore.text = p.Points.ToString();
        pp.transform.SetParent(PlayerPanelHolder.transform, false);
    }
}
