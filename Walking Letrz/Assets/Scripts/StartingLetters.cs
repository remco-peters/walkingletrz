using Assets.Scripts;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class StartingLetters : MyMonoBehaviour
{

    public LetterBlock StartingLetterBlockObject;
    public LetterManager LetterManager { get; set; }    
    public Vector3 lastLetterPosition { get; set; }
    public char firstLetter { get; set; }
    public char secondLetter { get; set; }

    private Vector3 pos;
    // Start is called before the first frame update
    private void Awake()
    {
        InitStartingLetters();
    }

    private void InitStartingLetters()
    {
        firstLetter = LetterManager.GetLetters(1)[0];
        secondLetter = LetterManager.GetLetters(1)[0];
        pos = lastLetterPosition;

        LetterManager.InstantiateLetterButton(firstLetter, pos, true);
        pos.x += 0.8f;
        LetterManager.InstantiateLetterButton(secondLetter, pos, false, true);
        pos.x += 0.8f;
        
        Debug.Log($"{firstLetter}, {secondLetter}");
    }

    public Vector3 GetLastLetterPosition()
    {
        return pos;
    }
}
