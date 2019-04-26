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
        PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest(), PlayerAccountSuccess, OnFailure);
        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(), PlayerInventorySuccess, OnFailure);
    }
    
    private void PlayerProfileSuccess(GetPlayerProfileResult result)
    {
        CurrentPlayer = result.PlayerProfile;
        if (string.IsNullOrEmpty(CurrentPlayer.DisplayName))
        {
            //Show popup for display name
            _displayNamePopup = Instantiate(DisplayNamePopupClass);
            _displayNamePopup.OnDisplayNameSave += SetDisplayName;
           _displayNamePopup.transform.SetParent(StartSceneCanvas.transform, false);
        }
        var getStatistics = new GetPlayerStatisticsRequest();
        var statisticNames = new List<string>
        {
            "Score",
            "Wins",
            "GamesPlayed",
            "TotalScore",
            "WordCount"
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
            amountOfCredits.text = balance.ToString();
        } else
        {
            amountOfCredits.text = "0";
        }
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

    public static void AddUsernameAndPassword(string email, string password)
    {
        var addUsernameAndPassword = new AddUsernamePasswordRequest { Email = email, Password = password, Username = CurrentPlayer.DisplayName };
        PlayFabClientAPI.AddUsernamePassword(addUsernameAndPassword, AddUsernameSuccess, OnFailure);
    }

    private static void AddUsernameSuccess(AddUsernamePasswordResult result)
    {
        GameObject.FindGameObjectWithTag("SavedEmailAddressSuccess").GetComponent<ShowInfoText>().ShowToast(3);
    }


}
