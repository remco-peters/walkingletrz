using UnityEngine;
using UnityEngine.Events;

public class PlaceWordBtn : MonoBehaviour
{
    public event UnityAction OnPlaceBtnTouched;
    public void ButtonClicked()
    {
        OnPlaceBtnTouched();
    }
}
