using Assets.Scripts;
using UnityEngine;
using UnityEngine.Events;

public class TradeLettersBtn : MyMonoBehaviour
{
    public event UnityAction OnTradeTouched;
    public LetterManager LetterManager { get; set; }
    private void OnMouseDown()
    {
        OnTradeTouched();
    }     
}
