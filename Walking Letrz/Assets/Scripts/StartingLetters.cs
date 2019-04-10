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

        LetterManager.InstantiateLetterButton(firstLetter, pos, true, false, row);
        pos.x += 0.8f;
        LetterManager.InstantiateLetterButton(secondLetter, pos, false, true, row);
        pos.x += 0.8f;
        
        Debug.Log($"{firstLetter}, {secondLetter}");
    }

    // Todo remove this one or give back the index (0 or 1) and row (always 1)
    public Vector3 GetLastLetterPosition()
    {
        return pos;
    }
}
