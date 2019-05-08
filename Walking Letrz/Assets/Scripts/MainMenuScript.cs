using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuScript : MonoBehaviour
{
    public Text Username;
    // Start is called before the first frame update
    void Start()
    {
        Username.text = AccountManager.CurrentPlayer.DisplayName;
    }
}
