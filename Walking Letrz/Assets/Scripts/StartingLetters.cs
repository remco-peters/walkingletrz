using Assets.Scripts;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class StartingLetters : MyMonoBehaviour
{

    public LetterManager LetterManager { get; set; }    
    public TheLetterManager TheLetterManager { get; set; }    
    public char firstLetter { get; set; }
    public char secondLetter { get; set; }
    //Todo remove all the positions
    private Vector3 pos;
    private int row = 1;
    // Start is called before the first frame update
    private void Awake()
    {
        InitStartingLetters();
    }

    private void InitStartingLetters()
    {
        firstLetter = TheLetterManager.GetLetters(1)[0];
        secondLetter = TheLetterManager.GetLetters(1)[0];

        LetterManager.InstantiateLetterButton(firstLetter, true, false, row);
        LetterManager.InstantiateLetterButton(secondLetter, false, true, row);
        
        Debug.Log($"{firstLetter}, {secondLetter}");
    }
}
