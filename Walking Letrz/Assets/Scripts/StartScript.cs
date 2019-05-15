using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StartScript : MonoBehaviour
{
    public GameInstance gameInstance;
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
    }
}
