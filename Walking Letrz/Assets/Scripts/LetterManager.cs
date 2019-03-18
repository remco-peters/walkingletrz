using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.Experimental.UIElements;
using Button = UnityEngine.UI.Button;

public class LetterManager : MonoBehaviour
{
    public StartingLetters StartingLettersClass;
    public PlayerLetters PlayerLettersClass;
    public GameObject LetterBlockObject;

    public GameObject LetterBoard { get; set; }

    private Vector3 lastLetterPosition = new Vector3(-2.5f, -2.5f);

    private void Start()
    {
        instantiateStartingLetters();
        instantiatePlayerLetters();
    }

    private void instantiatePlayerLetters()
    {
        PlayerLetters playerLetters = Instantiate(PlayerLettersClass);
        char[] letters = playerLetters.getLetters();
        for (int i = 0; i < letters.Length; i++)
        {
            if (i > 0)
                lastLetterPosition.x += 0.80f;
            if (i > 0 && i % 7 == 0)
            {
                lastLetterPosition.x = -2.5f;
                lastLetterPosition.y -= 0.75f;
            }

            var letterBlock = Instantiate(LetterBlockObject, lastLetterPosition, new Quaternion());

            lastLetterPosition = letterBlock.transform.position;
            letterBlock.GetComponentInChildren<TextMesh>().text = letters[i].ToString().ToUpper();
        }
    }

    private void instantiateStartingLetters()
    {
        {
            StartingLetters startingLetters = Instantiate(StartingLettersClass);

            var startingLetterBlock = Instantiate(LetterBlockObject, lastLetterPosition, new Quaternion());

            startingLetterBlock.GetComponentInChildren<TextMesh>().text = startingLetters.firstLetter;

        }
    }
}
