using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;
/*
public class PlayerLetters : MyMonoBehaviour
{    
    private char[] availableLetters = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l',
        'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };

    public LetterBlock LetterBlockObject;
    public LetterManager LetterManager { get; set; }
    public Vector3 LastLetterPosition { get; set; }

//    private int letterCount = 15;
    private char[] startingLetters = new char[15];
    private Vector3 pos;
    private int row;
    private int version = 2;
    private int index;

    private void Awake()
    {
        pos = LastLetterPosition;
        InitPlayerLetters();
    }

    public char[] GetLetters()
    {
        return startingLetters;
    }
    
    public void InitPlayerLetters()
    {
        startingLetters = TheLetterManager.GetLetters(15);
        for (int i = 0; i < startingLetters.Length; i++)
        {
            if(version == 2)
            {
                if(i < 5)
                {
                    row = 1;
                    index = i;
                }
                else if(i < 12)
                {
                    row = 2;
                    index = i - 5;
                }
                else
                {
                    row = 3;
                    index = i - 12;
                }
            }


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
            //Todo remove all the positions
            LetterBlock letterBlock = LetterManager.InstantiateLetterButton(startingLetters[i], pos, false, false, row);
            LetterManager.PlayerLetters.Add(new LetterPosition(row, index, letterBlock));
        }
        pos.x += 0.80f;
    }
}*/
