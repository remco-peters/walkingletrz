using Assets.Scripts;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    public void SwitchScene(string SceneName)
    {
        //Debug.Log(SceneName);
        SceneManager.LoadScene(SceneName);
    }

    public void SwithSceneToMatchResult(Player player, List<Player> playerList, string SceneName)
    {
        MatchResultScript.MyPlayer = player;
        MatchResultScript.PlayersList = playerList;
        SceneManager.LoadScene(SceneName);
    }
}
