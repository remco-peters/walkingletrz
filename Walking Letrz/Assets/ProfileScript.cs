using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProfileScript : MonoBehaviour
{
    public GameObject AddEmailPanel;

    public void showAddEmailPanel()
    {
        AddEmailPanel.SetActive(true);
    }
}
