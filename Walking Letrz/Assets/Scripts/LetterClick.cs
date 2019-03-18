using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LetterClick : MonoBehaviour
{
    public void OnClick(BaseEventData data)
    {
        PointerEventData pData = (PointerEventData)data;
        Debug.Log(pData);
    }
}
