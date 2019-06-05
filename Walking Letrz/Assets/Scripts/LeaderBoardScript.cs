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

    private void SetLeaderboardTitle(string difficulty)
    {
        LeaderboardTitle.text = I2.Loc.LocalizationManager.GetTranslation($"difficulty_{difficulty}");
    }

    private void ClearContentHolder()
    {
        foreach (Transform child in ContentHolder.transform)
            Destroy(child.gameObject);
    }

    public void GetEasyLeaderboard()
    {
        AccountManager.instance.GetLeaderboard("easy");
        SetLeaderboardTitle("easy");
    }

    public void GetMediumLeaderboard()
    {
        AccountManager.instance.GetLeaderboard("medium");
        SetLeaderboardTitle("medium");
    }

    public void GetHardLeaderboard()
    {
        AccountManager.instance.GetLeaderboard("hard");
        SetLeaderboardTitle("hard");
    }
    
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
