using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProfileScript : MonoBehaviour
{
    public GameObject AddEmailPanel;
    public GameObject AddEmailButton;
    public GameObject ShowEmailInfo;

    void Awake()
    {
        ReplaceAddEmailPanel();
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
}
