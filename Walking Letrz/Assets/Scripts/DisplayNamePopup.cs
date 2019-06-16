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
    
    /// <summary>
    /// When the save button is touched check if the name is not too long and save it
    /// </summary>
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

    /// <summary>
    /// When editing ended set the text in text field to a local variable
    /// </summary>
    /// <param name="displayName"></param>
    public void OnDisplayNameEditingEnded(string displayName)
    {
        _displayName = displayName;
    }

    /// <summary>
    /// Destroys the popup
    /// </summary>
    public void DestroyPopup()
    {
        Destroy(PopupOverlay);
    }
}
