using Assets.Scripts;
using UnityEngine.Events;
using UnityEngine.UI;

public class TradeLettersBtn : MyMonoBehaviour
{
    public Text ButtonText;
    public Image TradeRotateImage;
    public event UnityAction OnTradeTouched;
    public LetterManager LetterManager { get; set; }
    public void ButtonClicked()
    {
        OnTradeTouched();
    }   
}
