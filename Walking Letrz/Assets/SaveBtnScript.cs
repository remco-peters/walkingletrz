using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveBtnScript : MonoBehaviour
{
    private string email;
    private string passwrd;

    public void SetEmail(string email)
    {
        this.email = email;
    }

    public void SetPass(string pass)
    {
        passwrd = pass;
    }

    public void SaveEmail()
    {
        AccountManager.AddUsernameAndPassword(email, passwrd);
        AccountManager.CurrentPlayerAccount.PrivateInfo.Email = email;
        gameObject.transform.parent.gameObject.SetActive(false);
    }
}
