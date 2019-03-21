﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LetterBlock : MonoBehaviour
{
    public bool IsLetterSet;
    public event UnityAction<LetterBlock> OnLetterTouched;
    private void OnMouseDown()
    {
        OnLetterTouched(this);
    }
}
