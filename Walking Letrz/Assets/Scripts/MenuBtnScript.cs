using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuBtnScript : MonoBehaviour
{
    public GameObject menuPanel;
    public Sprite arrowLeft;
    private Sprite hamburgerImg;

    void Start()
    {
        hamburgerImg = GetComponent<Image>().sprite;
    }

    public void OnMenuClicked()
    {
        if (!menuPanel.activeInHierarchy)
        {
            menuPanel.SetActive(true);
            GetComponent<Image>().sprite = arrowLeft;
        } else
        {
            menuPanel.SetActive(false);
            GetComponent<Image>().sprite = hamburgerImg;
        }
    }
}
