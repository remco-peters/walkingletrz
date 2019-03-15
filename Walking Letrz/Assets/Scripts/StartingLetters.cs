using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartingLetters : MonoBehaviour
{
    private string[] availableLetters = { "a", "b,", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l",
        "m", "n", "o", "p", "r", "s", "t", "u", "v", "w", "z" };
    
    public string firstLetter { get; set; }
    public string secondLetter { get; set; }
    // Start is called before the first frame update
    private void Awake()
    {
        firstLetter = availableLetters[Random.Range(0, availableLetters.Length)];
        secondLetter = availableLetters[Random.Range(0, availableLetters.Length)];
        Debug.Log(firstLetter + " " + secondLetter);
    }
}
