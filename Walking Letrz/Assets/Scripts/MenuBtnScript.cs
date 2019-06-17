using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuBtnScript : MonoBehaviour
{
    public GameObject menuPanel;
    public Sprite arrowLeft;
    private Sprite hamburgerImg;
    public GameObject ShopPanel;

    void Start()
    {
        hamburgerImg = GetComponent<Image>().sprite;
    }

    /// <summary>
    /// Method that will be called after the player clicks on the menu
    /// </summary>
    public void OnMenuClicked()
    {
        if (!menuPanel.activeInHierarchy)
        {
            menuPanel.SetActive(true);
            GetComponent<Image>().sprite = arrowLeft;
            if (ShopPanel.activeInHierarchy)
                ShopPanel.SetActive(false);
        } else
        {
            menuPanel.SetActive(false);
            GetComponent<Image>().sprite = hamburgerImg;
        }
    }
}
