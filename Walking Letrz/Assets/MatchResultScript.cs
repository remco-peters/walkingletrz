using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.UI;

public class MatchResultScript : MonoBehaviour
{
    public GameObject PlayerPanelHolder;
    public PlayerPanel PlayerPanelClass;

    public Sprite bronze;
    public Sprite silver;
    public Sprite gold;
    
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
        pp.GetComponent<PlayerPanel>().crownImg.sprite = GetRightImg(p.place);
        if (p.BestWords.Count > 0) pp.GetComponent<PlayerPanel>().firstWord.text = p.BestWords[0].ToUpper();
        if (p.BestWords.Count > 1) pp.GetComponent<PlayerPanel>().secondWord.text = p.BestWords[1].ToUpper();
        if (p.BestWords.Count > 2)pp.GetComponent<PlayerPanel>().thirdWord.text = p.BestWords[2].ToUpper();
        pp.transform.SetParent(PlayerPanelHolder.transform, false);

        if (p.localPlayer)
        {
            PlayFabClientAPI.WritePlayerEvent(new WriteClientPlayerEventRequest()
                {
                    Body = new Dictionary<string, object>()
                    {
                        {"PlayerScore", p.Points},
                        {"PlayerPlace", p.place},
                        {"PlayerPlacedWordCount", p.WordCount}
                    },
                    EventName = "player_finished_game"
                },
                result => Debug.Log("Success saving scores"),
                error => Debug.LogError(error.GenerateErrorReport()));
        }
    }

    private Sprite GetRightImg(int place)
    {
        switch(place)
        {
            case 1:
                return gold;
            case 2:
                return silver;
            case 3:
                return bronze;
            case 4:
                return null;
            default:
                return null;
        }
    }
}
