using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyPlayer : MonoBehaviour
{
    public LetterManager LetterManagerClass;
    // Start is called before the first frame update
    void Start()
    {
        Instantiate(LetterManagerClass);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
