using PlayFab.ClientModels;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FriendManager : MonoBehaviour
{

    public GameObject PlayerNameHolder;
    public GameObject PlayerNameObject;
    public Text FriendsTxt;

    private List<FriendInfo> _friends;

    // Start is called before the first frame update
    void Awake()
    {
        _friends = AccountManager.instance.GetFriends();
    }

    // Update is called once per frame
    void Start()
    {
        if (_friends.Count > 0)
        {
            foreach (FriendInfo friend in _friends)
            {
                GameObject PlayerInfo = Instantiate(PlayerNameObject, PlayerNameHolder.transform, false);
                PlayerInfo.GetComponentsInChildren<Text>()[0].text = friend.Profile.DisplayName;
                DateTime dt = (DateTime)friend.Profile.LastLogin;
                PlayerInfo.GetComponentsInChildren<Text>()[1].text = dt.ToString("dd-MM-yyyy, HH:mm");
            }

            FriendsTxt.text += $" ( {_friends.Count} )";
        }
        else
        {
            GameObject PlayerInfo = Instantiate(PlayerNameObject, PlayerNameHolder.transform, false);
            PlayerInfo.GetComponentsInChildren<Text>()[0].text = I2.Loc.LocalizationManager.GetTranslation("friends_none");
            PlayerInfo.GetComponentsInChildren<Text>()[1].text = string.Empty;
        }
    }
}
