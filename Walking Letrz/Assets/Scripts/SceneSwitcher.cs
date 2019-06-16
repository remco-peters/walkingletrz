using Assets.Scripts;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    public void SwitchScene(string SceneName)
    {
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
        if (GameInstance.instance.IsMultiplayer)
        {
            SwitchScene("LobbyScene");
        }
        else
        {
            SwitchScene("GameScene");
        }
    }

    public void MakeMediumGame()
    {
        GameInstance.instance.difficulty = Difficulty.Medium;
        if (GameInstance.instance.IsMultiplayer)
        {
            SwitchScene("LobbyScene");
        } else
        {
            SwitchScene("GameScene");
        }
            
        //Todo: fixen
//        SwitchScene("BoosterScene");
    }

    public void MakeHardGame()
    {
        GameInstance.instance.difficulty = Difficulty.Hard;
        if (GameInstance.instance.IsMultiplayer)
        {
            SwitchScene("LobbyScene");
        }
        else
        {
            SwitchScene("GameScene");
        }
    }

    public void LeaveLobby()
    {
        PhotonManager.PhotonInstance.LeaveLobby();
        Destroy(PhotonManager.PhotonInstance.gameObject);
        SwitchScene("StartScene");
    }
    
    public void LoadPhotonBoosterScene()
    {
        SwitchScene("GameScene");
    }
}
