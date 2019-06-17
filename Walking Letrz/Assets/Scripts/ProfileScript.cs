using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProfileScript : MonoBehaviour
{
    public GameObject AddEmailPanel;
    public GameObject AddEmailButton;
    public GameObject ShowEmailInfo;
    public GameObject AddFacebookButton;
    public GameObject ShowFacebookInfo;
    public GameObject InviteMailPopUp;
    public Text UsernameError;

    void Awake()
    {
        ReplaceAddEmailPanel();
        ReplaceAddFacebookPanel();
    }

    /// <summary>
    /// To show the panel where a user can enter his emailadredd
    /// </summary>
    public void ShowAddEmailPanel()
    {
        AddEmailPanel.SetActive(true);
    }

    /// <summary>
    /// Set the emailaddress on screen when an address is already filled in
    /// </summary>
    public void ReplaceAddEmailPanel()
    {
        if(AccountManager.CurrentPlayerAccount.PrivateInfo.Email != null && AccountManager.CurrentPlayerAccount.PrivateInfo.Email.Length > 0)
        {
            AddEmailButton.SetActive(false);
            ShowEmailInfo.SetActive(true);
            ShowEmailInfo.GetComponentInChildren<Text>().text = AccountManager.CurrentPlayerAccount.PrivateInfo.Email;
        }
    }

    /// <summary>
    /// Sets the text that facebook is already linked, when there is a link available
    /// </summary>
    public void ReplaceAddFacebookPanel()
    {
        if (AccountManager.CurrentPlayerAccount.FacebookInfo != null && AccountManager.CurrentPlayerAccount.FacebookInfo.FacebookId.Length > 0)
        {
            AddFacebookButton.SetActive(false);
            ShowFacebookInfo.SetActive(true);
        }
    }

    /// <summary>
    /// Popup for inviting a  friend
    /// </summary>
    public void AddFriendClick()
    {
        InviteMailPopUp.SetActive(!InviteMailPopUp.activeSelf);
    }

    /// <summary>
    /// Gets called when the editing of the display name ends
    /// if the new displayname is longer than 13 characters an error is shown and the name is not saved
    /// </summary>
    /// <param name="newDisplayName"></param> the new display name to be saved
    public void DisplayNameEditingEnded(string newDisplayName)
    {
        if (newDisplayName == AccountManager.CurrentPlayer.DisplayName) return;
        if (newDisplayName.Length > 13)
            UsernameError.enabled = true;
        else
        {
            UsernameError.enabled = false;
            AccountManager.instance.SetDisplayName(newDisplayName);
        }
    }
}
