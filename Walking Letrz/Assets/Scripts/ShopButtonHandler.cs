using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopButtonHandler : MonoBehaviour
{
    public GameObject ShopPanel;

    public void OpenCloseShop()
    {
        if (!ShopPanel.activeInHierarchy)
        {
            ShopPanel.SetActive(true);
        }
        else
        {
            ShopPanel.SetActive(false);
        }
    }
}
