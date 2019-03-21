using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerLetters : MonoBehaviour
{    
    private char[] availableLetters = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l',
        'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };
    // Start is called before the first frame update

//    private int letterCount = 15;
    private char[] startingLetters = new char[15];

    private void Awake()
    {
        for (int i = 0; i <= 14; i++)
        {
            startingLetters[i] = availableLetters[Random.Range(0, 26)];
        }
    }

    public char[] getLetters()
    {
        return startingLetters;
    }
}
