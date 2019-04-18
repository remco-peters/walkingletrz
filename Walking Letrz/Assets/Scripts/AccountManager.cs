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
    private GetLeaderboardRequest leaderboardRequest;
    private void Awake()
    {
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
            leaderboardRequest = new GetLeaderboardRequest {StatisticName = "Score", MaxResultsCount = 10};
        }
    }

    private void LeaderboardSuccess(GetLeaderboardResult result)
    {
        //TODO: Show list in UI
        Debug.Log(result.Leaderboard);
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
    }

    private void OnFailure(PlayFabError error)
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
        CurrentPlayer.DisplayName = result.DisplayName;

    }
}
