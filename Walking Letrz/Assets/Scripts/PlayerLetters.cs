using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;

public class PlayerLetters : MonoBehaviour
{    
    private string[] availableLetters = { "a", "b,", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l",
        "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };
    // Start is called before the first frame update

//    private int letterCount = 15;
    private string[] startingLetters = new string[15];

    private void Awake()
    {
        for (int i = 0; i < 14; i++)
        {
            startingLetters[i] = availableLetters[Random.Range(0, 26)];
            Debug.Log(startingLetters[i]);
        }
        
        Debug.Log(startingLetters);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
