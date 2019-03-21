using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LetterBlock : MonoBehaviour
{
    private void OnMouseDown()
    {
        var letter = GetComponentInChildren<TextMesh>().text;
        PlaceWordBtn.IClickedIt();
        Debug.Log($"letter: {letter}");
    }
}
