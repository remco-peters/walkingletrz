using UnityEngine;
using UnityEngine.Events;

public class GenericButton : MonoBehaviour
{
    public event UnityAction OnTouched;
    public void ButtonClicked()
    {
        OnTouched();
    }  
}
