using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;
using UnityEngine.UI;
using PlayFab;
using System.Collections;

public class PlayfabFacebookAuthExample : MonoBehaviour
{

    public GameObject AddFacebookButton;
    public GameObject ShowFacebookInfo;

    private void Awake()
    {
        if(!FB.IsInitialized)
        {
            FB.Init(() =>
            {
                if (FB.IsInitialized)
                {
                    FB.ActivateApp();
                } else
                {
                    Debug.LogError("Couldnt initialize");
                }
            },
            isGameShown =>
            {
                if (!isGameShown)
                    Time.timeScale = 0; // pause game
                else
                    Time.timeScale = 1; // Continue game
            }
            );
        }
        else
        {
            FB.ActivateApp();
        }
    }

    #region Login / Logout
    public void FacebookLogin()
    {
        FB.LogInWithReadPermissions(new List<string>() { "public_profile", "email", "user_friends" }, OnFacebookLoggedIn);
        AddFacebookButton.SetActive(false);
        ShowFacebookInfo.SetActive(true);
    }

    private void OnFacebookLoggedIn(ILoginResult result)
    {
        if (result == null || string.IsNullOrEmpty(result.Error))
        {
            AccountManager.instance.AddFacebookLink(AccessToken.CurrentAccessToken.TokenString);
        }
    }

    public void FacebookLogout()
    {
        FB.LogOut();
    }
    #endregion

    public void FacebookShare()
    {
        //FB.ShareLink(new System.Uri("http://", "check it out!");
    }

    #region Inviting
    public void FacebookGameRequest()
    {
        FB.AppRequest("Hey, play this game!", title: "testtitle");
    }

    public void FacebookInvite()
    {
        FB.AppRequest(
    "Come play this great game!",
    null, null, null, null, null, null,
    delegate (IAppRequestResult result) {
        if (result == null || string.IsNullOrEmpty(result.Error))
        {
            AccountManager.instance.creditClass.AddCredits(10);
        }
    }
);
    }
    #endregion

    public void GetFriendsPlayingThisGame()
    {/*
        string query = "/me/friends";
        FB.API(query, HttpMethod.GET, result =>
        {
            var dictionary = (Dictionary<string, object>)Facebook.MiniJSON.Json.Deserialize(result.RawResult);
            var friendsList = (List<object>)dictionary["data"];
            FriendsText.text = string.Empty;
            foreach(var dict in friendsList)
            {
                FriendsText.text += ((Dictionary<string, object>)dict)["name"];
            }
        });*/
    }
}