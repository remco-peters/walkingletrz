using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;
using UnityEngine.UI;
using PlayFab;

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
                if(FB.IsInitialized)
                {
                    FB.ActivateApp();
                } else
                {
                    Debug.LogError("Couldnt initialize");
                }
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
        FB.LogInWithReadPermissions(null, OnFacebookLoggedIn);
        AddFacebookButton.SetActive(false);
        ShowFacebookInfo.SetActive(true);
    }

    private void OnFacebookLoggedIn(ILoginResult result)
    {
        if (result == null || string.IsNullOrEmpty(result.Error))
        {
            AccountManager.instance.AddFacebookLink(AccessToken.CurrentAccessToken.TokenString);
            AccountManager.CurrentPlayerAccount.FacebookInfo.FacebookId = AccessToken.CurrentAccessToken.TokenString;
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
        //FB.Mobile.AppInvite(new System.Uri())
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