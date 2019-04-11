using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    public void SwitchScene(string SceneName)
    {
        //Debug.Log(SceneName);
        SceneManager.LoadScene(SceneName);
    }
}
