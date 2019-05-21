using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StartScript : MonoBehaviour
{
    public GameInstance gameInstance;
    public GameObject StartSceneCanvas;
    public DisplayNamePopup DisplayNamePopupClass;
    public Text creditsText;

    // Start is called before the first frame update
    void Awake()
    {
        if (GameInstance.instance == null)
        {
            //Instantiate gameManager prefab
            Instantiate(gameInstance);
        }
        creditsText.text = AccountManager.instance?.Credits ?? "0";  
        if (string.IsNullOrEmpty(AccountManager.CurrentPlayer.DisplayName))
        {
            //Show popup for display name
            DisplayNamePopupClass = Instantiate(DisplayNamePopupClass);
            DisplayNamePopupClass.OnDisplayNameSave += AccountManager.instance.SetDisplayName;
            DisplayNamePopupClass.transform.SetParent(StartSceneCanvas.transform, false);
            AccountManager.instance.DisplayNamePopupClass = DisplayNamePopupClass;
        }
    }
}
