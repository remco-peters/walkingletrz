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
    /// <summary>
    ///  To login to facebook & ask permission for public profile, emailaddress and friends
    /// </summary>
    public void FacebookLogin()
    {
        FB.LogInWithReadPermissions(new List<string>() { "public_profile", "email", "user_friends" }, OnFacebookLoggedIn);
        AddFacebookButton.SetActive(false);
        ShowFacebookInfo.SetActive(true);
    }

    /// <summary>
    ///  When logged in successfully, call the AddFaceBookLink in AccountManager
    /// </summary>
    /// <param name="result"></param>
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

   
    
}