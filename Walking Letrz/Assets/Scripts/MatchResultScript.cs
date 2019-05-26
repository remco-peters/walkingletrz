using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

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

        // End the connection if in multiplayer, also put the flag isMultiplayer to false
        if(GameInstance.instance.IsMultiplayer)
        {
            PhotonManager.PhotonInstance.LeaveLobby();
            Destroy(PhotonManager.PhotonInstance.gameObject);
            GameInstance.instance.IsMultiplayer = false;
        }
    }

    private IEnumerator AddTimeToPlayerScore(PlayerData p, PlayerPanel pp)
    {
        while (true)
        {
            pp.playerTimeLeft.text = $" +{(int)Math.Ceiling(p.timeLeft)} sec";
            pp.playerScore.text = p.PointsWithoutTime.ToString();
            if (p.timeLeft <= 0)
            {
                pp.playerTimeLeft.text = "";
                yield return null;
            }
            else
            {
                p.timeLeft -= 2;
                p.PointsWithoutTime += 1;
                yield return new WaitForSeconds(0.2f);
            }
        }
    }

    private void SetUpInfo(PlayerData p)
    {
        PlayerPanel pp = Instantiate(PlayerPanelClass);
        pp.GetComponent<PlayerPanel>().playerName.text = p.Name;
        pp.GetComponent<PlayerPanel>().crownImg.sprite = GetRightImg(p.place);
                
        pp.GetComponent<PlayerPanel>().firstWord.text = (p.BestWords.ElementAtOrDefault(0) != null) ? p.BestWords[0].ToUpper() : "";
        pp.GetComponent<PlayerPanel>().secondWord.text = (p.BestWords.ElementAtOrDefault(1) != null) ? p.BestWords[1].ToUpper() : ""; ;
        pp.GetComponent<PlayerPanel>().thirdWord.text = (p.BestWords.ElementAtOrDefault(2) != null) ? p.BestWords[2].ToUpper() : ""; ;
        
        pp.transform.SetParent(PlayerPanelHolder.transform, false);
        
        if(GameInstance.instance.IsMultiplayer && !p.localPlayer && !AccountManager.instance.AlreadyFriends(p.playfabId))
        {
            Button addFriend = pp.GetComponent<PlayerPanel>().addFriend;
            addFriend.gameObject.SetActive(true);
            addFriend.GetComponent<AddFriendBtnScript>().OnAddFriendBtnTouched = () =>
            {
                AccountManager.instance.AddFriend(p.playfabId);
                addFriend.GetComponentInChildren<Text>().text = I2.Loc.LocalizationManager.GetTranslation("friend_added_friend");
                addFriend.interactable = false;
            };
        }

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

            #region statistics
            // Check if there are statistics present. If so, set previousScore & statisticsPresent to true
            int previousScore = 0;
            int? previousWins = 0;
            int? gamesPlayed = 0;
            int? totalScore = 0;
            int? wordCount = 0;
            int? AmountOfWordsPerMin = 0;
            int? AmountOfWordLenghtOfTwelve = 0;
            bool statisticsPresent = false;
            if (AccountManager.CurrentPlayer.Statistics.Count > 0)
            {
                statisticsPresent = true;
                previousScore = AccountManager.CurrentPlayer.Statistics.Find(model => model.Name == "Score").Value;
                previousWins = AccountManager.CurrentPlayer.Statistics.Find(model => model.Name == "Wins")?.Value;
                gamesPlayed = AccountManager.CurrentPlayer.Statistics.Find(model => model.Name == "GamesPlayed")?.Value;
                totalScore = AccountManager.CurrentPlayer.Statistics.Find(model => model.Name == "TotalScore")?.Value;
                wordCount = AccountManager.CurrentPlayer.Statistics.Find(model => model.Name == "WordCount")?.Value;
                AmountOfWordsPerMin = AccountManager.CurrentPlayer.Statistics.Find(model => model.Name == "AmountOfWordsPerMin")?.Value;
                AmountOfWordLenghtOfTwelve = AccountManager.CurrentPlayer.Statistics.Find(model => model.Name == "WordLengthOfTwelve")?.Value;
            }

            var updateStatisticsRequest = new UpdatePlayerStatisticsRequest();
            var statistics = new List<StatisticUpdate>();
            var statistic = new StatisticUpdate();

            // if new points are more than other points, put the new points
            if (p.Points > previousScore || p.Points == previousScore)
            {
                statistic = new StatisticUpdate {StatisticName = "Score", Value = (int) p.Points};
                statistics.Add(statistic);

                // When statistics aren't present, set these to currentPlayer
                if (!statisticsPresent)
                {
                    AccountManager.CurrentPlayer.Statistics = new List<StatisticModel>
                    {
                        new StatisticModel
                        {
                            Value = (int) p.Points,
                            Name = "Score"
                        }
                    };
                }
                else
                {
                    // If they are present, make sure the new score is added to currentPlayer
                    AccountManager.CurrentPlayer.Statistics.Find(model => model.Name == "Score").Value = (int) p.Points;
                }
            }

            if (p.place == 1)
            {
                StatisticModel newModel = new StatisticModel();
                // if previousWins is not null, put previousWins + 1, else put 1
                statistic = new StatisticUpdate {Value = previousWins + 1 ?? 1, StatisticName = "Wins"};
                statistics.Add(statistic);
                if(AccountManager.CurrentPlayer.Statistics.Find(model => model.Name == "Wins") != null)
                {
                    AccountManager.CurrentPlayer.Statistics.Find(model => model.Name == "Wins").Value++;
                } else
                {
                    AccountManager.CurrentPlayer.Statistics.Add(
                        new StatisticModel
                        {
                            Value = 1,
                            Name = "Wins"
                        }
                    );
                }
            }

            statistic = new StatisticUpdate {Value = gamesPlayed + 1 ?? 1, StatisticName = "GamesPlayed"};
            statistics.Add(statistic);
            if (AccountManager.CurrentPlayer.Statistics.Find(model => model.Name == "GamesPlayed") != null)
            {
                AccountManager.CurrentPlayer.Statistics.Find(model => model.Name == "GamesPlayed").Value++;
            }
            else
            {
                AccountManager.CurrentPlayer.Statistics.Add(
                    new StatisticModel
                    {
                        Value = 1,
                        Name = "GamesPlayed"
                    }
                );
            }

            statistic = new StatisticUpdate
                {Value = totalScore + (int) p.Points ?? (int) p.Points, StatisticName = "TotalScore"};
            statistics.Add(statistic);
            if (AccountManager.CurrentPlayer.Statistics.Find(model => model.Name == "TotalScore") == null)
            {
                AccountManager.CurrentPlayer.Statistics.Add(
                    new StatisticModel
                    {
                        Value = (int)p.Points,
                        Name = "TotalScore"
                    }
                );
            }

            statistic = new StatisticUpdate
                {Value = wordCount + p.WordCount ?? p.WordCount, StatisticName = "WordCount"};
            statistics.Add(statistic);
            if (AccountManager.CurrentPlayer.Statistics.Find(model => model.Name == "WordCount") != null)
            {
                AccountManager.CurrentPlayer.Statistics.Find(model => model.Name == "WordCount").Value += p.WordCount;
            }
            else
            {
                AccountManager.CurrentPlayer.Statistics.Add(
                    new StatisticModel
                    {
                        Value = p.WordCount,
                        Name = "WordCount"
                    }
                );
            }

            if (p.WordCountTwelveLetters > AmountOfWordLenghtOfTwelve)
            {
                statistic = new StatisticUpdate { StatisticName = "WordLengthOfTwelve", Value = p.WordCountTwelveLetters };
                statistics.Add(statistic);

                // When statistics aren't present, set these to currentPlayer
                if (!statisticsPresent)
                {
                    AccountManager.CurrentPlayer.Statistics = new List<StatisticModel>
                    {
                        new StatisticModel
                        {
                            Value = p.WordCountTwelveLetters,
                            Name = "WordLengthOfTwelve"
                        }
                    };
                }
                else
                {
                    // If they are present, make sure the new score is added to currentPlayer
                    AccountManager.CurrentPlayer.Statistics.Find(model => model.Name == "WordLengthOfTwelve").Value = p.WordCountTwelveLetters;
                }
                Debug.Log(p.WordCountTwelveLetters + " MatchResult");
            }

            int finalAmount;
            if(p.FinalWordCountPerMinute > AmountOfWordsPerMin)
            {
                finalAmount = p.FinalWordCountPerMinute;
            } else
            {
                finalAmount = AmountOfWordsPerMin ?? 0;
            }
            statistic = new StatisticUpdate { StatisticName = "AmountOfWordsPerMin", Value = finalAmount };
            statistics.Add(statistic);

            // When statistics aren't present, set these to currentPlayer
            if (!statisticsPresent)
            {
                AccountManager.CurrentPlayer.Statistics = new List<StatisticModel>
                {
                    new StatisticModel
                    {
                        Value = p.FinalWordCountPerMinute,
                        Name = "AmountOfWordsPerMin"
                    }
                };
            }
            

            if (statistics.Count <= 0) return;
            updateStatisticsRequest.Statistics = statistics;
            PlayFabClientAPI.UpdatePlayerStatistics(updateStatisticsRequest, OnSuccess, OnFailure);
            #endregion
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
        switch (place)
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