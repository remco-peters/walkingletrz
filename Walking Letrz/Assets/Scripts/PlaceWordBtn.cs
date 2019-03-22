using UnityEngine;
using UnityEngine.Events;

public class PlaceWordBtn : MonoBehaviour
{
    public event UnityAction OnPlaceBtnTouched;

    private void OnMouseDown()
    {
        OnPlaceBtnTouched();
    }
}
