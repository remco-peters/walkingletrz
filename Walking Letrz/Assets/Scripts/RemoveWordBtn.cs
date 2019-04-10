using UnityEngine;
using UnityEngine.Events;

public class RemoveWordBtn : MonoBehaviour
{
    public event UnityAction OnRemoveTouched;
    public void ButtonClicked()
    {
        OnRemoveTouched();
    }
}
