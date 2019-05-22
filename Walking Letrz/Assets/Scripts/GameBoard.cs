using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Assets.Scripts;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameBoard : MonoBehaviour
{
    public GameObject GameBoardWordHolder { get; set; }

    private LetterManager _letterManager;
    private PhotonView _photonView;
    
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

    public void CallRPC(long points = 0)
    {
        Debug.Log("RPC called");
//        if (PhotonNetwork.PlayerList.Length == 1)
//        {
//            PlaceWordInGameBoard(points);
//            Debug.Log("RPC 1 player");
//        }
//        else
//        {
//            
            _photonView.RPC("PlaceWordInGameBoard", RpcTarget.All, points);
//            Debug.Log("RPC >1 players");
//        }
    }

    [PunRPC]
    private void PlaceWordInGameBoard(long points = 0)
    {
        Debug.Log("Placewordingameboard");
        // Insantiate wordHolder
        GameObject wordHolder = Instantiate(GameBoardWordHolder);

        // Walk through all the letters placed
        foreach (LetterPosition letterPos in _letterManager.PlacedLetters)
        {
            LetterBlock block = letterPos.LetterBlock;
            if (block != null)
            {
                block.transform.SetParent(wordHolder.transform, false);
                block.GetComponent<Button>().interactable = false;


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

                // Lege gameobjecten toevoegen aan writeboard
                GameObject emptyBlock = Instantiate(_letterManager.EmptyLetterBlockObject, _letterManager.WritingBoard.transform, false);
                emptyBlock.transform.SetSiblingIndex(currentIndex);

                // Nieuwe playerletters aanmaken
                if (!block.IsFirstLetter && !block.IsSecondLetter)
                {
                    _letterManager.AddLetter(row, index);
                }
            }
            else
            {
                GameObject emptyPlaceHolder = Instantiate(_letterManager.PlaceHolderObject);
                emptyPlaceHolder.transform.SetParent(wordHolder.transform, false);
            }

            wordHolder.transform.SetParent(transform, false);
        }
    }
}