using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AddFriendBtnScript : EventTrigger
{
    public UnityAction OnAddFriendBtnTouched;

    public override void OnPointerClick(PointerEventData eventData)
    {
        OnAddFriendBtnTouched();
    }
}
