using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopButtonHandler : MonoBehaviour
{
    public GameObject ShopPanel;

    public void OpenShop()
    {
        ShopPanel.SetActive(true);
    }

    public void CloseShop()
    {
        ShopPanel.SetActive(false);
    }
}
