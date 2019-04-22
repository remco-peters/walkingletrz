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
    
    private IEnumerator AddTimeToPlayerScore(PlayerData p, PlayerPanel pp)
    {
        while (true)
        {
            pp.playerTimeLeft.text = $" +{p.timeLeft.ToString()} sec";
            pp.playerScore.text = p.Points.ToString();
            if (p.timeLeft <= 0){
                pp.playerTimeLeft.text = "";
                yield return null;
            }
            else
            {
                p.timeLeft -= 1;
                p.Points += 1;
                yield return new WaitForSeconds(0.2f);
            }
        }
    }

    private void SetUpInfo(PlayerData p)
    {
        PlayerPanel pp = Instantiate(PlayerPanelClass);
        pp.GetComponent<PlayerPanel>().playerName.text = p.Name;
       // pp.GetComponent<PlayerPanel>().playerScore.text = p.Points.ToString();
        pp.GetComponent<PlayerPanel>().crownImg.sprite = GetRightImg(p.place);
        //pp.GetComponent<PlayerPanel>().playerTimeLeft.text = $"+ {p.timeLeft.ToString()}";
        if (p.BestWords.Count > 0) pp.GetComponent<PlayerPanel>().firstWord.text = p.BestWords[0].ToUpper();
        if (p.BestWords.Count > 1) pp.GetComponent<PlayerPanel>().secondWord.text = p.BestWords[1].ToUpper();
        if (p.BestWords.Count > 2)pp.GetComponent<PlayerPanel>().thirdWord.text = p.BestWords[2].ToUpper();
        pp.transform.SetParent(PlayerPanelHolder.transform, false);

        StartCoroutine(AddTimeToPlayerScore(p, pp));

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

            // Check if there are statistics present. If so, set previousScore & statisticsPresent to true
            int previousScore = 0;
            bool statisticsPresent = false;
            if (AccountManager.CurrentPlayer.Statistics.Count > 0)
            {
                statisticsPresent = true;
                previousScore = AccountManager.CurrentPlayer.Statistics.Find(model => model.Name == "Score").Value;
            }

            // if new points are more than other points, put the new points
            if (p.Points > previousScore)
            {
                var updateStatisticsRequest = new UpdatePlayerStatisticsRequest();
                var statistics = new List<StatisticUpdate>();
                var statistic = new StatisticUpdate { StatisticName = "Score", Value = (int)p.Points };
                statistics.Add(statistic);

                updateStatisticsRequest.Statistics = statistics;
                PlayFabClientAPI.UpdatePlayerStatistics(updateStatisticsRequest, OnSuccess, OnFailure);

                // When statistics aren't present, set these to currentPlayer
                if (!statisticsPresent)
                {
                    AccountManager.CurrentPlayer.Statistics = new List<StatisticModel> {
                        new StatisticModel
                        {
                            Value = (int) p.Points,
                            Name = "Score"
                        }
                    };
                } else
                {
                    // If they are present, make sure the new score is added to currentPlayer
                    AccountManager.CurrentPlayer.Statistics.Find(model => model.Name == "Score").Value = (int) p.Points;
                }
                
            }
        }
    }

    private void OnSuccess(UpdatePlayerStatisticsResult result)
    {
        Debug.Log("Statistic successfully updated");
    }

    private void OnFailure(PlayFabError error)
    {
        Debug.Log(error.GenerateErrorReport());
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
