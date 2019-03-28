using UnityEngine;
using UnityEngine.Events;

public class LetterBlock : MonoBehaviour
{
    public bool IsLetterSet;
    public event UnityAction<LetterBlock> OnLetterTouched;
    public bool IsFirstLetter { get; set; } = false;
    public bool IsSecondLetter { get; set; } = false;
    private void OnMouseDown()
    {
        OnLetterTouched(this);
    }
}
