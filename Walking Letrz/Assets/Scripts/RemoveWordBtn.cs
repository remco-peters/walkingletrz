using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RemoveWordBtn : MonoBehaviour
{
    public event UnityAction OnRemoveTouched;
    private void OnMouseDown()
    {
        OnRemoveTouched();
    }
}
