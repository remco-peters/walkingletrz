using UnityEngine;
using UnityEngine.UI;

public class LoadScene : MonoBehaviour
{
    public AccountManager accountManager;
    public GameInstance gameInstance;
    public Text FailureText;
    // Start is called before the first frame update
    private void Awake()
    {
        if (AccountManager.instance == null)
        {
            Instantiate(accountManager);
        }
        if (GameInstance.instance == null)
        {
            Instantiate(gameInstance);
        }
    }

    // Update is called once per frame
    void Update()
    {
         if (AccountManager.instance?.fullyLoaded == true)
            SceneSwitcher.SwitchSceneStatic("StartScene");
         if (AccountManager.instance?.failure == true)
            FailureText.gameObject.SetActive(true);
    }
}
