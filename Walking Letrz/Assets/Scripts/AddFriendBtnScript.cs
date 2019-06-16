using UnityEngine.Events;
using UnityEngine.EventSystems;

public class AddFriendBtnScript : EventTrigger
{
    public UnityAction OnAddFriendBtnTouched;

    public override void OnPointerClick(PointerEventData eventData)
    {
        OnAddFriendBtnTouched();
    }
}
