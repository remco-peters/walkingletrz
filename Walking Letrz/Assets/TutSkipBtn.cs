using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class TutSkipBtn : EventTrigger
{
    public event UnityAction OnPlaceBtnTouched;

    public override void OnPointerClick(PointerEventData eventData)
    {
        
            OnPlaceBtnTouched();
    }
}
