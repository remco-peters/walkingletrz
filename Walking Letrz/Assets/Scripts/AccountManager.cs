﻿using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class AccountManager : MonoBehaviour
{
    public Text amountOfCredits;
    public GameObject LoadImage;
    public static PlayerProfileModel CurrentPlayer;
    public static UserAccountInfo CurrentPlayerAccount;
    public static GetUserInventoryResult CurrentPlayerInventory;
    public static List<PlayerLeaderboardEntry> Leaderboard;
    private GetLeaderboardRequest leaderboardRequest;
    public DisplayNamePopup DisplayNamePopupClass;
    public GameObject StartSceneCanvas;
    private string _displayName;
    public static AccountManager instance;
    public string Credits { get; set; } = "0";
    public bool fullyLoaded = false;
    public bool failure = false;
    public string PlayerName { get; set; }
    public Credit creditClass;
    private List<FriendInfo> _friends = null;
    private List<PlayerLeaderboardEntry> _friendsLeaderboard = null;
    public UnityAction<List<PlayerLeaderboardEntry>> OnLeaderboardReceived;

    public List<Achievement> listOfAchievements = new List<Achievement>();
    private void Awake()
    {
        // check if instance exists
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            // Then destroy  this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameInstance.
            Destroy(gameObject);
        }
        CreateListOfAchievements();
        DontDestroyOnLoad(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        if (CurrentPlayer == null)
        {
            var request = new LoginWithAndroidDeviceIDRequest
            {
                OS = SystemInfo.operatingSystem,
                TitleId = "F537C",
                AndroidDevice = SystemInfo.deviceModel,
                AndroidDeviceId = SystemInfo.deviceUniqueIdentifier,
                CreateAccount = true
            };
            PlayFabClientAPI.LoginWithAndroidDeviceID(request, Success, OnFailure);
        }
    }

    /// <summary>
    /// Creates the list of achievements currently in the game
    /// </summary>
    private void CreateListOfAchievements()
    {
        listOfAchievements.Add(new Achievement("AmountOfWordsPerMin", 3, 25, 1, "AmountOfWordsPerMin"));
        listOfAchievements.Add(new Achievement("AmountOfWordsPerMin", 5, 50, 2, "AmountOfWordsPerMin"));
        listOfAchievements.Add(new Achievement("AmountOfWordsPerMin", 10, 100, 3, "AmountOfWordsPerMin"));
        listOfAchievements.Add(new Achievement("AmountOfWordsPerMin", 15, 150, 4, "AmountOfWordsPerMin"));
        listOfAchievements.Add(new Achievement("PointsInGame", 50, 10, 1, "Score"));
        listOfAchievements.Add(new Achievement("PointsInGame", 75, 25, 2, "Score"));
        listOfAchievements.Add(new Achievement("PointsInGame", 100, 50, 3, "Score"));
        listOfAchievements.Add(new Achievement("PointsInGame", 125, 100, 4, "Score"));
        listOfAchievements.Add(new Achievement("PointsInGame", 150, 150, 5, "Score"));
        listOfAchievements.Add(new Achievement("PointsInGame", 200, 250, 6, "Score"));
        listOfAchievements.Add(new Achievement("PointsTotal", 1000, 10, 1, "TotalScore"));
        listOfAchievements.Add(new Achievement("PointsTotal", 2500, 25, 2, "TotalScore"));
        listOfAchievements.Add(new Achievement("PointsTotal", 5000, 50, 3, "TotalScore"));
        listOfAchievements.Add(new Achievement("PointsTotal", 10000, 100, 4, "TotalScore"));
        listOfAchievements.Add(new Achievement("PointsTotal", 25000, 150, 5, "TotalScore"));
        listOfAchievements.Add(new Achievement("WordLengthOfTwelve", 1, 10, 1, "WordLengthOfTwelve"));
        listOfAchievements.Add(new Achievement("WordLengthOfTwelve", 3, 25, 2, "WordLengthOfTwelve"));
        listOfAchievements.Add(new Achievement("WordLengthOfTwelve", 5, 50, 3, "WordLengthOfTwelve"));
        listOfAchievements.Add(new Achievement("WordLengthOfTwelve", 10, 100, 4, "WordLengthOfTwelve"));
        listOfAchievements.Add(new Achievement("WordLengthOfTwelve", 15, 150, 5, "WordLengthOfTwelve"));
    }
    /// <summary>
    /// Gets the correct leaderboard from playfab using the difficulty parameter
    /// </summary>
    /// <param name="difficulty"></param>
    public void GetLeaderboard(string difficulty)
    {
        GetLeaderboardRequest glb = new GetLeaderboardRequest { StatisticName = $"{difficulty}_score", StartPosition = 0, MaxResultsCount = 10 };
        PlayFabClientAPI.GetLeaderboard(glb, LeaderboardSuccess, OnFailure);
    }

    /// <summary>
    /// When leaderboard is successfully received the leaderboard received delegate gets called
    /// </summary>
    /// <param name="result"></param> the resulting leaderboard
    private void LeaderboardSuccess(GetLeaderboardResult result)
    {
        Leaderboard = result.Leaderboard;
        OnLeaderboardReceived(Leaderboard);
    }

    /// <summary>
    /// Refreshes the accountstats after a purchase or so
    /// </summary>
    public void RefreshAccountStats()
    {
        fullyLoaded = false;
        var combinedReq = new GetPlayerCombinedInfoRequest
        {
            PlayFabId = CurrentPlayer.PlayerId,
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams()
            {
                GetUserAccountInfo = true,
                GetUserInventory = false,
                GetUserVirtualCurrency = true,
                GetUserData = false,
                GetUserReadOnlyData = false,
                GetCharacterInventories = false,
                GetCharacterList = false,
                GetTitleData = false,
                GetPlayerStatistics = true,
                GetPlayerProfile = true
            }
        };
        PlayFabClientAPI.GetPlayerCombinedInfo(combinedReq, instance.CombinedInfoSuccess, OnFailure);
    }

    /// <summary>
    /// Successfunction that is called after the combinedInfoRequest was successfull
    /// </summary>
    /// <param name="result"></param>
    public void CombinedInfoSuccess(GetPlayerCombinedInfoResult result)
    {
        CurrentPlayer = result.InfoResultPayload.PlayerProfile;
        CurrentPlayerAccount = result.InfoResultPayload.AccountInfo;

        if (result.InfoResultPayload.UserVirtualCurrency.TryGetValue("CR", out int balance))
        {
            GameInstance.instance.credits = balance;
            Credits = balance.ToString();
        }
        else
        {
            GameInstance.instance.credits = 0;
            Credits = "0";
        }

        var statisticList = new List<StatisticModel>();
        foreach (var statisticValue in result.InfoResultPayload.PlayerStatistics)
        {
            var model = new StatisticModel
            {
                Value = statisticValue.Value,
                Name = statisticValue.StatisticName
            };
            statisticList.Add(model);
        }

        CurrentPlayer.Statistics = statisticList;

        fullyLoaded = true;
    }

    /// <summary>
    /// Successfunction called after the loginrequest was successfull
    /// </summary>
    /// <param name="result"></param>
    private void Success(LoginResult result)
    {
        var combinedReq = new GetPlayerCombinedInfoRequest();
        combinedReq.PlayFabId = result.PlayFabId;
        combinedReq.InfoRequestParameters = new GetPlayerCombinedInfoRequestParams()
        {
            GetUserAccountInfo = true,
            GetUserInventory = false,
            GetUserVirtualCurrency = true,
            GetUserData = false,
            GetUserReadOnlyData = false,
            GetCharacterInventories = false,
            GetCharacterList = false,
            GetTitleData = false,
            GetPlayerStatistics = true,
            GetPlayerProfile = true
        };
        PlayFabClientAPI.GetPlayerCombinedInfo(combinedReq, StartGetInfoSuccess, OnFailure);
        UpdateFriendsList();
        UpdateFriendsLeaderboard();
    }

    /// <summary>
    /// Successfunction called after the first init combinedResultRequest was successfull
    /// </summary>
    /// <param name="result"></param>
    private void StartGetInfoSuccess(GetPlayerCombinedInfoResult result)
    {
        CurrentPlayer = result.InfoResultPayload.PlayerProfile;
        if (!string.IsNullOrEmpty(CurrentPlayer.DisplayName))
            PlayerName = CurrentPlayer.DisplayName;

        CurrentPlayerAccount = result.InfoResultPayload.AccountInfo;

        if (result.InfoResultPayload.UserVirtualCurrency.TryGetValue("CR", out int balance))
        {
            GameInstance.instance.credits = balance;
            Credits = balance.ToString();
        }
        else
        {
            GameInstance.instance.credits = 0;
            Credits = "0";
        }

        var statisticList = new List<StatisticModel>();
        foreach (var statisticValue in result.InfoResultPayload.PlayerStatistics)
        {
            var model = new StatisticModel
            {
                Value = statisticValue.Value,
                Name = statisticValue.StatisticName
            };
            statisticList.Add(model);
        }

        CurrentPlayer.Statistics = statisticList;

        fullyLoaded = true;
    }
    
    /// <summary>
    /// Called when a request has failed
    /// </summary>
    /// <param name="error"></param>
    private void OnFailure(PlayFabError error)
    {
        Debug.Log(error.GenerateErrorReport());
        failure = true;
    }

    //used from input field from editor on profile scene
    /// <summary>
    /// Sets the entered displayname of a user
    /// </summary>
    /// <param name="displayName"></param> The name that is entered in the textfield
    public void SetDisplayName(string displayName)
    {
        var displayNameRequest = new UpdateUserTitleDisplayNameRequest { DisplayName = displayName };
        PlayFabClientAPI.UpdateUserTitleDisplayName(displayNameRequest, DisplayNameSuccess, OnFailure);
    }
    
    /// <summary>
    /// When the displayname is successfully saved a short toast message is shown to the user and the popup gets destroyed
    /// </summary>
    /// <param name="result"></param>
    private void DisplayNameSuccess(UpdateUserTitleDisplayNameResult result)
    {
        GameObject.FindGameObjectWithTag("SavedUsernameSuccess").GetComponent<ShowInfoText>().ShowToast(3);
        CurrentPlayer.DisplayName = result.DisplayName;
        DestroyPopup();
    }

    /// <summary>
    /// Destroys the popup for setting a display name
    /// </summary>
    private void DestroyPopup()
    {
        DisplayNamePopupClass.DestroyPopup();
    }

    /// <summary>
    /// Adds a username and a password in PlayFab
    /// </summary>
    /// <param name="email"></param>
    /// <param name="password"></param>
    public void AddUsernameAndPassword(string email, string password)
    {
        var addUsernameAndPassword = new AddUsernamePasswordRequest { Email = email, Password = password, Username = CurrentPlayer.DisplayName };
        PlayFabClientAPI.AddUsernamePassword(addUsernameAndPassword, AddUsernameSuccess, OnFailure);
    }

    /// <summary>
    /// Successfunction, called when the username and password are successfully added in PlayFab
    /// </summary>
    /// <param name="result"></param>
    private void AddUsernameSuccess(AddUsernamePasswordResult result)
    {
        GameObject.FindGameObjectWithTag("SavedEmailAddressSuccess").GetComponent<ShowInfoText>().ShowToast(3);
        creditClass.AddCredits(100);
        RefreshAccountStats();
    }

    /// <summary>
    /// Function to add the Facebook link in PlayFab
    /// </summary>
    /// <param name="facebookToken"></param>
    public void AddFacebookLink(string facebookToken)
    {
        var request = new LinkFacebookAccountRequest { AccessToken = facebookToken };
        PlayFabClientAPI.LinkFacebookAccount(request, AddFacebookSuccess, OnFailure);
    }

    /// <summary>
    /// Successfuntion, called when the facebook link was successfully added in PlayFab
    /// </summary>
    /// <param name="result"></param>
    private void AddFacebookSuccess(LinkFacebookAccountResult result)
    {
        creditClass.AddCredits(100);
        RefreshAccountStats();
    }

    #region friendsStuff

    /// <summary>
    /// Function that will update/reload the friends list
    /// </summary>
    public void UpdateFriendsList()
    {
        GetFriendsListRequest req = new GetFriendsListRequest
        {
            ProfileConstraints = new PlayerProfileViewConstraints() { ShowLastLogin = true, ShowDisplayName = true },
            IncludeFacebookFriends = true
        };

        PlayFabClientAPI.GetFriendsList(req, r =>
        {
            _friends = r.Friends;
        }, OnFailure);
    }

    /// <summary>
    /// Function to get a list of friends
    /// </summary>
    /// <returns>List<FriendsInfo></returns>
    public List<FriendInfo> GetFriends()
    {
        return _friends;
    }


    /// <summary>
    /// Function to add a friend in PlayFab
    /// </summary>
    /// <param name="friendId"></param>
    public void AddFriend(string friendId)
    {
        var request = new AddFriendRequest
        {
            FriendPlayFabId = friendId
        };

        // Execute request and update friends when we are done
        PlayFabClientAPI.AddFriend(request, result =>
        {
            Debug.Log("Friend added successfully!");
        }, OnFailure);

        UpdateFriendsList();
        UpdateFriendsLeaderboard();
    }

    /// <summary>
    /// Function to remove a friend from Playfab
    /// </summary>
    /// <param name="friendInfo"></param>
    public void RemoveFriend(FriendInfo friendInfo)
    {
        PlayFabClientAPI.RemoveFriend(new RemoveFriendRequest
        {
            FriendPlayFabId = friendInfo.FriendPlayFabId
        }, result =>
        {
            _friends.Remove(friendInfo);
        }, OnFailure);


        UpdateFriendsList();
        UpdateFriendsLeaderboard();
    }

    /// <summary>
    /// Function to update the friends leaderboard
    /// </summary>
    public void UpdateFriendsLeaderboard()
    {
        PlayFabClientAPI.GetFriendLeaderboard(new GetFriendLeaderboardRequest
        {
            StartPosition = 0,
            StatisticName = "Score"
        }, result =>
        {
            _friendsLeaderboard = result.Leaderboard;
        }, OnFailure);
    }

    /// <summary>
    /// To get a list with leaderboardentries
    /// </summary>
    /// <returns>List of leaderboardentries</returns>
    public List<PlayerLeaderboardEntry> GetFriendsLeaderboard()
    {
        return _friendsLeaderboard;
    }
    /// <summary>
    /// Function that will check if you're already friends with a specific person
    /// </summary>
    /// <param name="playfabId"></param>
    /// <returns>bool</returns>
    public bool AlreadyFriends(string playfabId)
    {
        foreach(FriendInfo friend in _friends)
        {
            if (friend.FriendPlayFabId == playfabId)
                return true;
        }

        return false;
    }

#endregion
}
