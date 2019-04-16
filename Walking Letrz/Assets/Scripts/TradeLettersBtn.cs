using Assets.Scripts;
using UnityEngine.Events;

public class TradeLettersBtn : MyMonoBehaviour
{
    public event UnityAction OnTradeTouched;
    public LetterManager LetterManager { get; set; }
    public void ButtonClicked()
    {
        OnTradeTouched();
    }   
}
