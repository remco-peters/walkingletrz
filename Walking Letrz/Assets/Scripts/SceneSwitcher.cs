using Assets.Scripts;
using I2.Loc;
using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    public void SwitchScene(string SceneName)
    {
        //Debug.Log(SceneName);
        SceneManager.LoadScene(SceneName);
    }

    public void SetTutorialLevel(string SceneName)
    {
        PlayerPrefs.SetInt("HadTutorialGame", 0);
        SceneManager.LoadScene(SceneName);
    }

    public static void SwitchSceneStatic(string SceneName)
    {
        SceneManager.LoadScene(SceneName);
    }

    public void MakeEasyGame()
    {
        GameInstance.instance.difficulty = Difficulty.Easy;
        SwitchScene("GameScene");
    }

    public void MakeMediumGame()
    {
        GameInstance.instance.difficulty = Difficulty.Medium;
        PhotonManager.PhotonInstance.OnJoinedRoomDelegate += () =>
        {
            PhotonNetwork.LoadLevel("GameScene");
        };
//        SwitchScene("GameScene");
        
    }

    public void MakeHardGame()
    {
        GameInstance.instance.difficulty = Difficulty.Hard;
        SwitchScene("GameScene");
    }
}
