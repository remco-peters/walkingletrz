using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGameBtnScript : MonoBehaviour
{
    public void SetSinglePlayer()
    {
        GameInstance.instance.IsMultiplayer = false;
    }

    public void SetMultiplayer()
    {
        GameInstance.instance.IsMultiplayer = true;
    }
}
