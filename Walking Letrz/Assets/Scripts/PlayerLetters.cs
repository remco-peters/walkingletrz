using Assets.Scripts;

public class PlayerLetters : MyMonoBehaviour
{    
    private char[] availableLetters = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l',
        'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };

    public LetterManager letterManager { get; set; }
    // Start is called before the first frame update

//    private int letterCount = 15;
    private char[] startingLetters = new char[15];

    private void Awake()
    {
        startingLetters = letterManager.GetLetters(15);
    }

    public char[] getLetters()
    {
        return startingLetters;
    }
}
