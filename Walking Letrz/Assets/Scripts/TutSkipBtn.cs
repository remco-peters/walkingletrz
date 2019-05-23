using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class TutSkipBtn : EventTrigger
{
    public event UnityAction OnSkipTutorialBtnTouched;

    public override void OnPointerClick(PointerEventData eventData)
    {
        
            OnSkipTutorialBtnTouched();
    }
}
