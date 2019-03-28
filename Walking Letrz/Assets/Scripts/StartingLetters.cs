using Assets.Scripts;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class StartingLetters : MyMonoBehaviour
{

    public LetterBlock StartingLetterBlockObject;
    public LetterManager LetterManager { get; set; }    
    public Vector3 lastLetterPosition { get; set; }
    public string firstLetter { get; set; }
    public string secondLetter { get; set; }

    private Vector3 pos;
    // Start is called before the first frame update
    private void Awake()
    {
        InitStartingLetters();
    }

    private void InitStartingLetters()
    {
        firstLetter = LetterManager.GetLetters(1)[0].ToString();
        secondLetter =  LetterManager.GetLetters(1)[0].ToString();
        pos = lastLetterPosition;
        
        LetterBlock startingLetterBlock = Instantiate(StartingLetterBlockObject, pos, new Quaternion());
        startingLetterBlock.GetComponentInChildren<TextMesh>().text = firstLetter.ToUpper();
        startingLetterBlock.OnLetterTouched += LetterManager.LetterTouched;
        pos.x += 0.8f;
        startingLetterBlock.IsFirstLetter = true;
        
        startingLetterBlock = Instantiate(StartingLetterBlockObject, pos, new Quaternion());
        startingLetterBlock.GetComponentInChildren<TextMesh>().text = secondLetter.ToUpper();
        startingLetterBlock.OnLetterTouched += LetterManager.LetterTouched;
        pos.x += 0.8f;
        startingLetterBlock.IsSecondLetter = true;
        
        Debug.Log($"{firstLetter}, {secondLetter}");
    }

    public Vector3 GetLastLetterPosition()
    {
        return pos;
    }
}
