using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RemoveWordBtn : MonoBehaviour
{
    public event UnityAction OnRemoveTouched;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseDown()
    {
        var letter = GetComponentInChildren<TextMesh>().text;
        Debug.Log($"letter: {letter}");
        OnRemoveTouched();
    }
}
