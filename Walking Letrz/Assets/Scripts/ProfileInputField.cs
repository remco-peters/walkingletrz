using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.UI;

public class ProfileInputField : MonoBehaviour
{
    public InputField InputField;
    
    /// <summary>
    /// If the current user has a displayname, set it to the profile input field when it is shown
    /// </summary>
    private void Awake()
    {
        if (AccountManager.CurrentPlayer.DisplayName != null && AccountManager.CurrentPlayer.DisplayName.Length > 0)
            InputField.text = AccountManager.CurrentPlayer.DisplayName;
    }
}
