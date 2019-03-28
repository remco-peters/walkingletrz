using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;

public class PlayerLetters : MyMonoBehaviour
{    
    private char[] availableLetters = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l',
        'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };

    public LetterBlock LetterBlockObject;
    public LetterManager letterManager { get; set; }
    public Vector3 lastLetterPosition { get; set; }
    private Dictionary<Vector3, LetterBlock> PlayerLetterPositions { get; } = new Dictionary<Vector3, LetterBlock>();

//    private int letterCount = 15;
    private char[] startingLetters = new char[15];
    private Vector3 pos;

    private void Awake()
    {
        pos = lastLetterPosition;
        InitPlayerLetters();
    }

    public char[] GetLetters()
    {
        return startingLetters;
    }

    public void InitPlayerLetters()
    {
        startingLetters = letterManager.GetLetters(15);
        for (int i = 0; i < startingLetters.Length; i++)
        {
            if (i > 0)
                pos.x += 0.80f;
            if (i == 5)
            {
                pos.x = -2.5f;
                pos.y -= 0.75f;
            }

            if (i == 12)
            {
                pos.x = -0.9f;
                pos.y -= 0.75f;
            }

            LetterBlock letterBlock = Instantiate(LetterBlockObject, pos, new Quaternion());
            letterBlock.IsLetterSet = false;
            PlayerLetterPositions.Add(pos, letterBlock);
            letterBlock.OnLetterTouched += letterManager.LetterTouched; 
            pos = letterBlock.transform.position;
            letterBlock.GetComponentInChildren<TextMesh>().text = startingLetters[i].ToString().ToUpper();
        }

        pos.x += 0.80f;
    }
}
