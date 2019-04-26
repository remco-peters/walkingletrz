using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlaceWordBtn : EventTrigger
{
    public event UnityAction OnPlaceBtnTouched;
    public event UnityAction OnPlaceBtnTouchedWhileInteractive;

    public override void OnPointerClick(PointerEventData eventData)
    {
        if (!GetComponent<Button>().interactable)
        {
            OnPlaceBtnTouchedWhileInteractive();
        } else
        {
            OnPlaceBtnTouched();
        }
    }
}
