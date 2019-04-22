using UnityEngine;
using UnityEngine.Events;

public class BoosterBtn : MonoBehaviour
{
    public event UnityAction OnBoosterTouched;
    public void ButtonClicked()
    {
        OnBoosterTouched();
    }
}
