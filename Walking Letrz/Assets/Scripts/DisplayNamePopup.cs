using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DisplayNamePopup : MonoBehaviour
{
    public GameObject PopupOverlay;
    public UnityAction<string> OnDisplayNameSave;
    public Text ErrorText;

    private string _displayName;
    
    // Start is called before the first frame update
    public void OnDisplayNameSaveTouched()
    {
        if (_displayName.Length > 13)
            ErrorText.enabled = true;
        else
        {
            ErrorText.enabled = false;
            OnDisplayNameSave(_displayName);
        }
    }

    public void OnDisplayNameEditingEnded(string displayName)
    {
        _displayName = displayName;
    }

    public void DestroyPopup()
    {
        Destroy(PopupOverlay);
    }
}
