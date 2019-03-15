using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LetterManager : MonoBehaviour
{
    public StartingLetters StartingLettersClass;

    private void Start()
    {
        Instantiate(StartingLettersClass);
    }
}
