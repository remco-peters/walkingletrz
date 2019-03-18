using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{
    public Camera CameraClass;
    public MyPlayer PlayerClass;

    // Start is called before the first frame update
    void Start()
    {
        Instantiate(CameraClass);
        Instantiate(PlayerClass);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
