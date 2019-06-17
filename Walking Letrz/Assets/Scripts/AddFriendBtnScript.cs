using UnityEngine.Events;
using UnityEngine.EventSystems;

public class AddFriendBtnScript : EventTrigger
{
    public UnityAction OnAddFriendBtnTouched;
    /// <summary>
    /// When the button is clicked, the unityAction OnAddFriendBtnTouched will be fired
    /// </summary>
    /// <param name="eventData"></param>
    public override void OnPointerClick(PointerEventData eventData)
    {
        OnAddFriendBtnTouched();
    }
}
