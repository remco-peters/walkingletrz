using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

public class AccountManager : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        var request = new LoginWithAndroidDeviceIDRequest();
        request.OS = SystemInfo.operatingSystem;
        request.TitleId = "F537C";
        request.AndroidDevice = SystemInfo.deviceModel;
        request.AndroidDeviceId = SystemInfo.deviceUniqueIdentifier;
        request.CreateAccount = true;
        PlayFabClientAPI.LoginWithAndroidDeviceID(request, Success, Error);
    }

    private void Success(LoginResult result)
    {
        Debug.Log("Success");
    }

    private void Error(PlayFabError error)
    {
        Debug.Log("error");
    }
}
