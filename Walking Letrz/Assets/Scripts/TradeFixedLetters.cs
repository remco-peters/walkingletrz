using UnityEngine;
using UnityEngine.Events;

public class TradeFixedLetters : MonoBehaviour
{
    public event UnityAction OnTradeTouched;
    public void ButtonClicked()
    {
        OnTradeTouched();
    }  
}
