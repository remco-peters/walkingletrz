﻿using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderBoardScript : MonoBehaviour
{
    public GameObject ContentHolder;
    public LeaderboardEntry LeaderboardEntry;
    
    void Awake()
    {
        foreach (PlayerLeaderboardEntry entry in AccountManager.Leaderboard)
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
