using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendsLeaderboardScript : MonoBehaviour
{
    public GameObject ContentHolder;
    public LeaderboardEntry LeaderboardEntry;

    void Start()
    {
        foreach (PlayerLeaderboardEntry entry in AccountManager.instance.GetFriendsLeaderboard())
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
