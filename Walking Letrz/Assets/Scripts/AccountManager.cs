using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.PlayStreamModels;
using UnityEngine;
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
    private DisplayNamePopup _displayNamePopup;
    private string _displayName;
    public static AccountManager instance;
    public string Credits {get;set; } = "0";
    public bool fullyLoaded = false;
    public string playerName {get;set;}
    public Credit creditClass;

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
            leaderboardRequest = new GetLeaderboardRequest {StatisticName = "Score", StartPosition = 0, MaxResultsCount = 10};
        }
    }

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

    public static List<PlayerLeaderboardEntry> GetLeaderboard()
    {
        GetLeaderboardRequest glb = new GetLeaderboardRequest { StatisticName = "Score", StartPosition = 0, MaxResultsCount = 10 };
        PlayFabClientAPI.GetLeaderboard(glb, LeaderboardSuccess, OnFailure);
        return Leaderboard;
    }

    private static void LeaderboardSuccess(GetLeaderboardResult result)
    {
        Leaderboard = result.Leaderboard;
    }

    public void RefreshAccountStats()
    {
        fullyLoaded = false;
        var combinedReq = new GetPlayerCombinedInfoRequest();
        combinedReq.PlayFabId = CurrentPlayer.PlayerId;
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
        PlayFabClientAPI.GetPlayerCombinedInfo(combinedReq, instance.CombinedInfoSuccess, OnFailure);
    }

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
        fullyLoaded = true;
    }

    private void Success(LoginResult result)
    {
        var playerRequest = new GetPlayerProfileRequest();
        playerRequest.PlayFabId = result.PlayFabId;
        PlayFabClientAPI.GetPlayerProfile(playerRequest, PlayerProfileSuccess, OnFailure);
        PlayFabClientAPI.GetLeaderboard(leaderboardRequest, LeaderboardSuccess, OnFailure);
        PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest(), PlayerAccountSuccess, OnFailure);
        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(), PlayerInventorySuccess, OnFailure);
    }
    
    private void PlayerProfileSuccess(GetPlayerProfileResult result)
    {
        CurrentPlayer = result.PlayerProfile;
        if (!string.IsNullOrEmpty(CurrentPlayer.DisplayName))
            playerName = CurrentPlayer.DisplayName;

        var getStatistics = new GetPlayerStatisticsRequest();
        var statisticNames = new List<string>
        {
            "Score",
            "Wins",
            "GamesPlayed",
            "TotalScore",
            "WordCount",
            "AmountOfWordsPerMin",
            "WordLengthOfTwelve"
        };
        getStatistics.StatisticNames = statisticNames;
        PlayFabClientAPI.GetPlayerStatistics(getStatistics, OnStatisticsSuccess, OnFailure);
    }

    private void PlayerAccountSuccess(GetAccountInfoResult result)
    {
        CurrentPlayerAccount = result.AccountInfo;
    }

    private void PlayerInventorySuccess(GetUserInventoryResult result)
    {
        CurrentPlayerInventory = result;
        if(result.VirtualCurrency.TryGetValue("CR", out int balance)) {
            GameInstance.instance.credits = balance;
            Credits = balance.ToString();
        } else
        {
            GameInstance.instance.credits = 0;
            Credits = "0";
        }
        fullyLoaded = true;
    }

    private void OnStatisticsSuccess(GetPlayerStatisticsResult result)
    {
        var statisticList = new List<StatisticModel>();
        foreach (var statisticValue in result.Statistics)
        {
            var model = new StatisticModel
            {
                Value = statisticValue.Value,
                Name = statisticValue.StatisticName
            };
            statisticList.Add(model);
        }

        CurrentPlayer.Statistics = statisticList;
    }

    private static void OnFailure(PlayFabError error)
    {
        Debug.Log(error.GenerateErrorReport());
    }

    //used from input field from editor on profile scene
    public void SetDisplayName(string displayName)
    {
        var displayNameRequest = new UpdateUserTitleDisplayNameRequest {DisplayName = displayName};
        PlayFabClientAPI.UpdateUserTitleDisplayName(displayNameRequest, DisplayNameSuccess, OnFailure);
    }

    private void DisplayNameSuccess(UpdateUserTitleDisplayNameResult result)
    {
        Debug.Log($"New display name: {result.DisplayName}");
        DestroyPopup();
        GameObject.FindGameObjectWithTag("SavedUsernameSuccess").GetComponent<ShowInfoText>().ShowToast(3);
        CurrentPlayer.DisplayName = result.DisplayName;
    }

    private void DestroyPopup()
    {
        _displayNamePopup.DestroyPopup();
    }

    public void AddUsernameAndPassword(string email, string password)
    {
        var addUsernameAndPassword = new AddUsernamePasswordRequest { Email = email, Password = password, Username = CurrentPlayer.DisplayName };
        PlayFabClientAPI.AddUsernamePassword(addUsernameAndPassword, AddUsernameSuccess, OnFailure);
    }

    private void AddUsernameSuccess(AddUsernamePasswordResult result)
    {
        GameObject.FindGameObjectWithTag("SavedEmailAddressSuccess").GetComponent<ShowInfoText>().ShowToast(3);
        creditClass.AddCredits(100);
    }

    public void AddFacebookLink(string facebookToken)
    {
        var request = new LinkFacebookAccountRequest { AccessToken = facebookToken };
        PlayFabClientAPI.LinkFacebookAccount(request, AddFacebookSuccess, OnFailure);
    }

    private void AddFacebookSuccess(LinkFacebookAccountResult result)
    {
        creditClass.AddCredits(100);
        RefreshAccountStats();
    }
}
