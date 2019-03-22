using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;

public class StartingLetters : MyMonoBehaviour
{
    private string[] availableLetters = { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l",
        "m", "n", "o", "p", "r", "s", "t", "u", "v", "w", "z" };

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
