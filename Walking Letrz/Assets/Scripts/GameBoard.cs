using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Assets.Scripts;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameBoard : MonoBehaviour
{
    public GameObject GameBoardWordHolder { get; set; }
    public LetterBlock FixedLettersBlockObject { get; set; }
    public LetterBlock PlayerLetterBlockObject { get; set; }
    public GameObject PlaceHolderObject { get; set; }
    public TheLetterManager TheLM { get; set; }

    private LetterManager _letterManager;
    private PhotonView _photonView;
//    private List<LetterPosition> PlacedLetters;
    
    public LetterManager LetterManager
    {
        get => _letterManager;
        set
        {
            _letterManager = value;
            _letterManager.OnWordPlaced = CallRPC;
            Debug.Log("RPC Set");
        }
    }
    
    private void Start()
    {
        _photonView = PhotonView.Get(this);
        _photonView.ViewID = 1337;

    }

    public void CallRPC(long points, List<LetterPosition> placedLetters)
    {
        Debug.Log("RPC called");
        
        string word = "";
        int first = 0, 
            second = 0;
        foreach (LetterPosition let in placedLetters)
        {
            LetterBlock block = let.LetterBlock;
            if(block != null)
            {
                if(block.IsFirstLetter)
                {
                    first = let.GetCurrentIndex();
                }
                if (block.IsSecondLetter)
                {
                    second = let.GetCurrentIndex();
                }

                word += block.GetLetter();
            }
        }

        _photonView.RPC(nameof(PlaceWordInGameBoard), RpcTarget.All, word, first, second);
        RemoveAndChangeLetters(placedLetters, points);
        GameState.PlacedWordsInThisGame.Add(word);

    }

    [PunRPC]
    private void PlaceWordInGameBoard(string word, int firstIndex, int secondIndex)
    {
        GameObject wordHolder = Instantiate(GameBoardWordHolder);
        int i = 0;
        foreach (char letter in word)
        {
            LetterBlock block;
            if(i == firstIndex || i == secondIndex)
            {
                block = _letterManager.FixedLettersBlockObject;

            } else
            {
                block = _letterManager.PlayerLetterBlockObject;
            }

            block = Instantiate(block);
            block.GetComponent<Button>().interactable = false;
            block.GetComponentsInChildren<Text>()[0].text = letter.ToString().ToUpper();
            block.GetComponentsInChildren<Text>()[1].text = TheLM.CharactersValues.
                First(x => x.Key == char.ToLower(letter)).Value.ToString();

            block.transform.SetParent(wordHolder.transform, false);
            
            block.GetComponent<Button>().interactable = false;
            i++;
        }

        Debug.Log($"{word}, fixedFirst = {firstIndex} fixedSecond: {secondIndex}");

        for(int j = word.Length; j <= 12; j++)
        {
            GameObject emptyPlaceHolder = Instantiate(PlaceHolderObject);
            emptyPlaceHolder.transform.SetParent(wordHolder.transform, false);
        }

        wordHolder.transform.SetParent(transform, false);
    }

    private void RemoveAndChangeLetters(List<LetterPosition> placedLetters, long points)
    {
        foreach (LetterPosition letterPos in placedLetters)
        {
            LetterBlock block = letterPos.LetterBlock;
            if (block != null)
            {
                Vector3 pos = block.transform.position;
                _letterManager.ShowScoreGainedText(points, pos);

                // Replace placeholder with letter on playerBoard
                int row = letterPos.GetRow();
                int index = letterPos.GetOldIndex();
                int currentIndex = letterPos.GetCurrentIndex();
                _letterManager.PlayerLetters.Remove(letterPos);

                if (!block.IsFirstLetter && !block.IsSecondLetter)
                {
                    // Placeholders verwijderen
                    GameObject parentRow = _letterManager.GetRightRow(row);
                    Transform placeHolder = parentRow.transform.GetChild(index);
                    DestroyImmediate(placeHolder.gameObject);
                }

                // Nieuwe playerletters aanmaken
                if (!block.IsFirstLetter && !block.IsSecondLetter)
                {
                    _letterManager.AddLetter(row, index);
                }

                // Lege gameobjecten toevoegen aan writeboard
                DestroyImmediate(block.gameObject);
                GameObject emptyBlock = Instantiate(_letterManager.EmptyLetterBlockObject, _letterManager.WritingBoard.transform, false);
                emptyBlock.transform.SetSiblingIndex(currentIndex);
            }
            
        }
    }
}