using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LetterManager : MonoBehaviour
{
    public StartingLetters StartingLettersClass;
    public PlayerLetters PlayerLettersClass;

    private void Start()
    {
        Instantiate(StartingLettersClass);
        Instantiate(PlayerLettersClass);
    }
}
