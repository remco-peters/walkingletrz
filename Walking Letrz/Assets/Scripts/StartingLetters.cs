using Assets.Scripts;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class StartingLetters : MyMonoBehaviour
{

    public LetterManager LetterManager { get; set; }    
    public char firstLetter { get; set; }
    public char secondLetter { get; set; }
    public TheLetterManager TheLetterManager { get;set; }

    private Vector3 pos = new Vector3(-2.5f, -2.5f);
    // Start is called before the first frame update
    private void Awake()
    {
        InitStartingLetters();
    }

    private void InitStartingLetters()
    {
        firstLetter = TheLetterManager.GetLetters(1)[0];
        secondLetter = TheLetterManager.GetLetters(1)[0];

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
