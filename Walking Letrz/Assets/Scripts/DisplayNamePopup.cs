using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DisplayNamePopup : MonoBehaviour
{
    public GameObject PopupOverlay;
    public UnityAction<string> OnDisplayNameSave;

    private string _displayName;
    // Start is called before the first frame update
    public void OnDisplayNameSaveTouched()
    {
        OnDisplayNameSave(_displayName);
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
