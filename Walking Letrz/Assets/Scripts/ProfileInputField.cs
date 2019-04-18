using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.UI;

public class ProfileInputField : MonoBehaviour
{
    public InputField InputField;
    private void Awake()
    {
        if (AccountManager.CurrentPlayer.DisplayName.Length > 0)
            InputField.text = AccountManager.CurrentPlayer.DisplayName;
    }
}
