using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{
    public Camera CameraClass;
    public PlayerManager PlayerManagerClass;

    public LetterManager LetterManagerClass;
    // Start is called before the first frame update
    void Start()
    {
        Instantiate(CameraClass);
//        Instantiate(PlayerManagerClass);
        Instantiate(LetterManagerClass);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
