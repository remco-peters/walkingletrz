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
    public GameObject PlaceHolderObject { get; set; }
    public TheLetterManager TheLM { get; set; }
    public LetterBlock PlayerLettersBlockObject { get; set; }
    public LetterBlock FixedLettersBlockObject { get; set; }

    public LetterBlock FixedLettersBlockObjectGameBoard;
    public LetterBlock PlayerLetterBlockObjectGameBoard;

    public Material FixedLetterOtherPlayerMaterial;
    public Material PlayerLetterOtherPlayerMaterial;

    public Material NormalFixedMaterial;
    public Material NormalPlayerLetterMaterial;

    private Vector3 _pos;
    private LetterManager _letterManager;
    private PhotonView _photonView;
    
    public LetterManager LetterManager
    {
        get => _letterManager;
        set
        {
            _letterManager = value;
            _letterManager.OnWordPlaced = CallRPC;
        }
    }
    
    /// <summary>
    /// When a game is started get the photonview so RPC's can be called
    /// </summary>
    private void Start()
    {
        _photonView = PhotonView.Get(this);
        _photonView.ViewID = 1337;

    }

    public void CallRPC(long points, List<LetterPosition> placedLetters, string pID)
    {
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

        _photonView.RPC(nameof(PlaceWordInGameBoard), RpcTarget.All, word, first, second, pID, points);
        RemoveAndChangeLetters(placedLetters, points);

    }

    public void CallRPCPlaceLtrz(string letter, bool isFirst, bool isSecond, int row, int index, int newLetrz = 0)
    {
        StartCoroutine(ExecutePlaceLettersAfterTime(2, letter, isFirst, isSecond, row, index, newLetrz));
    }

    /// <summary>
    /// Call the rpc after 2 seconds so the rpc can be called on all clients (sometimes rpc was called before photon view was set)
    /// </summary>
    /// <param name="time"></param>
    /// <param name="letter"></param>
    /// <param name="isFirst"></param>
    /// <param name="isSecond"></param>
    /// <param name="row"></param>
    /// <param name="index"></param>
    /// <param name="newLetrz"></param>
    /// <returns></returns>
    IEnumerator ExecutePlaceLettersAfterTime(float time, string letter, bool isFirst, bool isSecond, int row, int index, int newLetrz = 0)
    {
        yield return new WaitForSeconds(time);
        
        _photonView.RPC(nameof(PlaceLetters), RpcTarget.Others, letter, isFirst, isSecond, row, index, newLetrz);
        
    }

    public void CallRPCInitPlayerLetters()
    {
        StartCoroutine(ExecuteInitPlayerLettersAfterTime(2));
    }

    /// <summary>
    /// Call the rpc after 2 seconds so the rpc can be called on all clients (sometimes rpc was called before photon view was set)
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    IEnumerator ExecuteInitPlayerLettersAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        
        
        _photonView.RPC(nameof(InitPlayerLetters), RpcTarget.Others);
    }

    [PunRPC]
    private void InitPlayerLetters()
    {
        _letterManager.InitPlayerLetters();
    }

    [PunRPC]
    private void PlaceLetters(string letter, bool isFirstLetter, bool isSecondLetter, int row, int index, int newLetrz = 0)
    {
        LetterBlock block;
        if (isFirstLetter || isSecondLetter)
        {
            GameObject parentRow = _letterManager.GetRightRow(row);

            if (isFirstLetter)
                TheLM.FirstLetter = letter[0];
            if (isSecondLetter)
                TheLM.SecondLetter = letter[0];
            
            if (newLetrz == 1)
            {
                DestroyImmediate(parentRow.transform.GetChild(index).gameObject);
            }

            block = FixedLettersBlockObject;
            block = Instantiate(block);
            block.IsFirstLetter = isFirstLetter;
            block.IsSecondLetter = isSecondLetter;
            block.OnLetterTouched += _letterManager.LetterTouched;
            //Todo
            //lttrBlock.OnLetterDragged += LetterDragged;
            bool canMove = (bool)PhotonNetwork.LocalPlayer.CustomProperties["CanMove"];
            block.GetComponentsInChildren<Text>()[0].text = letter.ToString().ToUpper();
            block.GetComponentsInChildren<Text>()[1].text = TheLM.CharactersValues
                .First(x => x.Key == char.ToLower(letter[0])).Value.ToString();
            block.transform.SetParent(parentRow.transform, false);
                block.transform.SetSiblingIndex((int)index);
            
            

            _letterManager.PlayerLetters.Add(new LetterPosition(row, block.transform.GetSiblingIndex(), block));
            /*if (isFirstLetter || isSecondLetter)
            {
                block.GetComponentsInChildren<Text>()[0].text = canMove ? letter.ToString().ToUpper() : "?";
                block.GetComponentsInChildren<Text>()[1].text = canMove ? TheLM.CharactersValues
                .First(x => x.Key == char.ToLower(letter[0])).Value.ToString() : "";
            }*/
            if (isFirstLetter){
                _letterManager.FirstLetterBlock = block;                
            }
            else if (isSecondLetter) {              
                _letterManager.SecondLetterBlock = block;
            }
        } else
        {
            block = PlayerLettersBlockObject;
            block = Instantiate(block);
            block.IsFirstLetter = isFirstLetter;
            block.IsSecondLetter = isSecondLetter;
            block.OnLetterTouched += _letterManager.LetterTouched;

            block.GetComponentsInChildren<Text>()[0].text = letter.ToString().ToUpper();
            block.GetComponentsInChildren<Text>()[1].text = TheLM.CharactersValues.First(x => x.Key == char.ToLower(letter[0])).Value.ToString();
            GameObject parentRow = _letterManager.GetRightRow(row);
            block.transform.SetParent(parentRow.transform, false);
        
            block.transform.SetSiblingIndex((int)index);
        
            _letterManager.PlayerLetters.Add(new LetterPosition(row, block.transform.GetSiblingIndex(), block));
        }
    }

    [PunRPC]
    private void PlaceWordInGameBoard(string word, int firstIndex, int secondIndex, string pID, long points)
    {
        Debug.Log($"LocalPlayer: {PhotonNetwork.LocalPlayer.UserId}, sended: {pID}");
        GameState.PlacedWordsInThisGame.Add(word.ToLower());
        GameObject wordHolder = Instantiate(GameBoardWordHolder);
        int i = 0;
        foreach (char letter in word)
        {
            LetterBlock block;
            if(i == firstIndex || i == secondIndex)
            {
                block = FixedLettersBlockObjectGameBoard;
                if(PhotonNetwork.LocalPlayer.UserId != pID)
                {
                    block.GetComponent<Image>().material = FixedLetterOtherPlayerMaterial;
                } else
                {
                    block.GetComponent<Image>().material = NormalFixedMaterial;
                }
            } else
            {
                block = PlayerLetterBlockObjectGameBoard;

                if (PhotonNetwork.LocalPlayer.UserId != pID)
                {
                    block.GetComponent<Image>().material = PlayerLetterOtherPlayerMaterial;
                } else
                {
                    block.GetComponent<Image>().material = NormalPlayerLetterMaterial;
                }
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

        for (int j = word.Length; j <= 12; j++)
        {
            GameObject emptyPlaceHolder = Instantiate(PlaceHolderObject);
            emptyPlaceHolder.transform.SetParent(wordHolder.transform, false);
        }

        wordHolder.transform.SetParent(transform, false);
        
        _pos = new Vector3();
        _pos.y = wordHolder.transform.position.y - 25;
        _pos.x = wordHolder.transform.GetChild(word.Length).transform.position.x + 100;
        
        _letterManager.ShowScoreGainedText(points, _pos);
    }

    /// <summary>
    /// Moves the letters from the playerboard to the word list and creates new objects to replace them
    /// </summary>
    /// <param name="placedLetters"></param>
    /// <param name="points"></param>
    private void RemoveAndChangeLetters(List<LetterPosition> placedLetters, long points)
    {
        foreach (LetterPosition letterPos in placedLetters)
        {
            LetterBlock block = letterPos.LetterBlock;
            if (block != null)
            {

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