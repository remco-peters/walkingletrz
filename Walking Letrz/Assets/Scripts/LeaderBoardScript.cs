using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderBoardScript : MonoBehaviour
{
    public GameObject ContentHolder;
    public LeaderboardEntry LeaderboardEntry;
    public Text LeaderboardTitle;
    
    void Start()
    {
        AccountManager.instance.OnLeaderboardReceived = (entries) =>
        {
            ClearContentHolder();
            SetLeaderboardEntries(entries);
        };
        GetEasyLeaderboard();
    }

    /// <summary>
    /// Sets the correct leaderboard title above the leaderboard 
    /// </summary>
    /// <param name="difficulty"></param>
    private void SetLeaderboardTitle(string difficulty)
    {
        LeaderboardTitle.text = I2.Loc.LocalizationManager.GetTranslation($"difficulty_{difficulty}");
    }

    /// <summary>
    /// Remove all Leaderboard entries before loading in new entries
    /// </summary>
    private void ClearContentHolder()
    {
        foreach (Transform child in ContentHolder.transform)
            Destroy(child.gameObject);
    }

    /// <summary>
    /// Gets the easy leaderboard
    /// </summary>
    public void GetEasyLeaderboard()
    {
        AccountManager.instance.GetLeaderboard("easy");
        SetLeaderboardTitle("easy");
    }

    /// <summary>
    /// Gets the medium leaderboard
    /// </summary>
    public void GetMediumLeaderboard()
    {
        AccountManager.instance.GetLeaderboard("medium");
        SetLeaderboardTitle("medium");
    }

    /// <summary>
    /// gets the hard leaderboard
    /// </summary>
    public void GetHardLeaderboard()
    {
        AccountManager.instance.GetLeaderboard("hard");
        SetLeaderboardTitle("hard");
    }
    
    /// <summary>
    /// Set all the data of an entry to a ui object and place it in the scroll view
    /// </summary>
    /// <param name="entries"></param>
    private void SetLeaderboardEntries(List<PlayerLeaderboardEntry> entries)
    {

        foreach (PlayerLeaderboardEntry entry in entries)
        {
            LeaderboardEntry newEntry = Instantiate(LeaderboardEntry);
            newEntry.SetName(entry.DisplayName);
            newEntry.SetPlace(entry.Position);
            newEntry.SetScore(entry.StatValue);
            newEntry.SetImage(entry.Position);

            newEntry.transform.SetParent(ContentHolder.transform, false);
        }
    }
}
