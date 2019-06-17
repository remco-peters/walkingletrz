using Assets.Scripts;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    /// <summary>
    /// Method to switch between scenes
    /// </summary>
    /// <param name="SceneName"></param>
    public void SwitchScene(string SceneName)
    {
        SceneManager.LoadScene(SceneName);
    }

    /// <summary>
    /// Method to swith to the tutorial level and set pref "HadTutorialGame" on zero
    /// </summary>
    /// <param name="SceneName"></param>
    public void SetTutorialLevel(string SceneName)
    {
        GameInstance.instance.difficulty = Difficulty.Easy;
        PlayerPrefs.SetInt("HadTutorialGame", 0);
        SceneManager.LoadScene(SceneName);
    }

    /// <summary>
    /// Static sceneswitcher
    /// </summary>
    /// <param name="SceneName"></param>
    public static void SwitchSceneStatic(string SceneName)
    {
        SceneManager.LoadScene(SceneName);
    }

    /// <summary>
    /// To switch to the gamescene with Easy. Go to the Lobby when MPG, otherwhise go to the game
    /// </summary>
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

    /// <summary>
    /// To switch to the gamescene with Medium. Go to the Lobby when MPG, otherwhise go to the game
    /// </summary>
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
    }

    /// <summary>
    /// To switch to the gamescene with Hard. Go to the Lobby when MPG, otherwhise go to the game
    /// </summary>
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

    /// <summary>
    /// The method that will be called when you leave the lobby
    /// </summary>
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
