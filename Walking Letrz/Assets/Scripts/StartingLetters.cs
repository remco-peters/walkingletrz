using Assets.Scripts;

public class StartingLetters : MyMonoBehaviour
{

    public LetterManager LetterManager { get; set; }    
    public string firstLetter { get; set; }
    public string secondLetter { get; set; }
    // Start is called before the first frame update
    private void Awake()
    {
        firstLetter = LetterManager.GetLetters(1)[0].ToString();
        secondLetter =  LetterManager.GetLetters(1)[0].ToString();
    }
}
