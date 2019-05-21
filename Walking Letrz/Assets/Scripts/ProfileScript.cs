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

    void Awake()
    {
        ReplaceAddEmailPanel();
        ReplaceAddFacebookPanel();
    }

    public void ShowAddEmailPanel()
    {
        AddEmailPanel.SetActive(true);
    }

    public void ReplaceAddEmailPanel()
    {
        if(AccountManager.CurrentPlayerAccount.PrivateInfo.Email != null && AccountManager.CurrentPlayerAccount.PrivateInfo.Email.Length > 0)
        {
            AddEmailButton.SetActive(false);
            ShowEmailInfo.SetActive(true);
            ShowEmailInfo.GetComponentInChildren<Text>().text = AccountManager.CurrentPlayerAccount.PrivateInfo.Email;
        }
    }

    public void ReplaceAddFacebookPanel()
    {
        if (AccountManager.CurrentPlayerAccount.FacebookInfo != null && AccountManager.CurrentPlayerAccount.FacebookInfo.FacebookId.Length > 0)
        {
            AddFacebookButton.SetActive(false);
            ShowFacebookInfo.SetActive(true);
        }
    }
}
