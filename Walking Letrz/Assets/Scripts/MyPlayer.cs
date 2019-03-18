using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyPlayer : MonoBehaviour
{
    public LetterManager LetterManagerClass;

    public GameObject WriteBoard, LetterBoardObject;
    // Start is called before the first frame update
    void Start()
    {
        Instantiate(WriteBoard);
        GameObject LetterBoard = Instantiate(LetterBoardObject);
        LetterManager letterManager = Instantiate(LetterManagerClass);
        letterManager.LetterBoard = LetterBoard;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
