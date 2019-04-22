using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.PlayStreamModels;
using UnityEngine;
using UnityEngine.UI;

public class AccountManager : MonoBehaviour

{
    public static PlayerProfileModel CurrentPlayer;
    public static List<PlayerLeaderboardEntry> Leaderboard;
    private GetLeaderboardRequest leaderboardRequest;
    public GameObject DisplayNamePopup;
    private string _displayName;
    private void Awake()
    {
        DontDestroyOnLoad(this);
        DisplayNamePopup.SetActive(false);
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
                CreateAccount = true,
                InfoRequestParameters = new GetPlayerCombinedInfoRequestParams()
                {
                    // And make sure PlayerProfile is included
                    GetPlayerProfile = true,
                    // Define rules for PlayerProfile request
                    ProfileConstraints = new PlayerProfileViewConstraints()
                    {
                        ShowLinkedAccounts = true
                    }
                }
            };
            PlayFabClientAPI.LoginWithAndroidDeviceID(request, Success, OnFailure);
            leaderboardRequest = new GetLeaderboardRequest {StatisticName = "Score", StartPosition = 0, MaxResultsCount = 10};
        }
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

    private void Success(LoginResult result)
    {
        var playerRequest = new GetPlayerProfileRequest();
        playerRequest.PlayFabId = result.PlayFabId;
        PlayFabClientAPI.GetPlayerProfile(playerRequest, PlayerProfileSuccess, OnFailure);
        PlayFabClientAPI.GetLeaderboard(leaderboardRequest, LeaderboardSuccess, OnFailure);
    }
    
    private void PlayerProfileSuccess(GetPlayerProfileResult result)
    {
        CurrentPlayer = result.PlayerProfile;
        if (string.IsNullOrEmpty(CurrentPlayer.DisplayName))
        {
            //Show popup for display name
            DisplayNamePopup.SetActive(true);
        }
        var getStatistics = new GetPlayerStatisticsRequest();
        var statisticNames = new List<string>();
        statisticNames.Add("Score");
        getStatistics.StatisticNames = statisticNames;
        PlayFabClientAPI.GetPlayerStatistics(getStatistics, OnStatisticsSuccess, OnFailure);
    }

    private void OnStatisticsSuccess(GetPlayerStatisticsResult result)
    {
        var statisticList = new List<StatisticModel>();
        foreach (var statisticValue in result.Statistics)
        {
            var model = new StatisticModel();
            model.Value = statisticValue.Value;
            model.Name = statisticValue.StatisticName;
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
        DisplayNamePopup.SetActive(false);
    }

    public static void AddUsernameAndPassword(string email, string password)
    {
        var addUsernameAndPassword = new AddUsernamePasswordRequest { Email = email, Password = password, Username = CurrentPlayer.DisplayName };
        PlayFabClientAPI.AddUsernamePassword(addUsernameAndPassword, AddUsernameSuccess, OnFailure);
    }

    private static void AddUsernameSuccess(AddUsernamePasswordResult result)
    {
        GameObject.FindGameObjectWithTag("SavedEmailAddressSuccess").GetComponent<ShowInfoText>().ShowToast(3);
    }

    public void OnDisplayNameSaveTouched()
    {
        SetDisplayName(_displayName);
    }

    public void OnDisplayNameEditingEnded(string displayName)
    {
        _displayName = displayName;
    }
}
