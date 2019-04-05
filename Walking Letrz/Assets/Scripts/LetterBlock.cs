using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Experimental.UIElements;

public class LetterBlock : MyMonoBehaviour
{
    public event UnityAction<LetterBlock> OnLetterTouched;
    public event UnityAction<LetterBlock> OnLetterDragged;
    public bool IsFirstLetter { get; set; } = false;
    public bool IsSecondLetter { get; set; } = false;

    private Vector3 offset;
    private Vector3 oldPosition;
    private Vector3 firstLetterPosition = new Vector3(-2.5f, -2.5f);
    private Vector3 secondLetterPosition = new Vector3(-1.7f, -2.5f);
    private Dictionary<Vector3, LetterBlock> playerLetterPositions;

    private bool mouseDown;
    private float timer;
    private float timeDelay = 0.5f;

    private void Update()
    {
        if (mouseDown)
        {
            timer += Time.deltaTime;
        }
    }

    private void OnMouseDown()
    {
        offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f));
        oldPosition = transform.position;
        mouseDown = true;
    }

    private void OnMouseDrag()
    {
        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f);
        Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
        transform.position = curPosition;
    }

    private void OnMouseUp()
    {
        mouseDown = false;

        if (timer > timeDelay)
        {
            OnLetterDragged(this);
        }
        else
        {
            OnLetterTouched(this);
        }
        timer = 0;
    }

    public void SetToOldPosition(Dictionary<Vector3, LetterBlock> placedLetterPositions)
    {
        Vector3 pos;
        Vector3 oldPos = placedLetterPositions.FirstOrDefault(x => x.Value == this).Key;
        if (oldPos == new Vector3(0.0f, 0.0f, 0.0f))
        {
            oldPos = playerLetterPositions.FirstOrDefault(x => x.Value == this).Key;
            playerLetterPositions[oldPos] = null;
        }
        else
        {
            placedLetterPositions[oldPos] = null;
        }
        if (IsFirstLetter) pos = firstLetterPosition;
        else if (IsSecondLetter) pos = secondLetterPosition;
        else
        {
            pos = playerLetterPositions.FirstOrDefault(x => x.Value == null).Key;
            if (pos == new Vector3(0.0f, 0.0f, 0.0f))
            {
                pos = playerLetterPositions.FirstOrDefault(x => x.Value == this).Key;
            }
            playerLetterPositions[pos] = this;
        }

        transform.localScale = new Vector3(0.5f, 0.5f, 1);
        transform.position = pos;
    }

    public void LetterDragged(Dictionary<Vector3, LetterBlock> placedLetterPositions, Dictionary<Vector3, LetterBlock> playerLetterPositions)
    {
        this.playerLetterPositions = playerLetterPositions;
        if (!(transform.position.y < 0) || !(transform.position.y > -2))
        {
            SetToOldPosition(placedLetterPositions);
            return;
        }

        float minDist = 100;
        var index = 0;
        var indexToUse = -1;

        foreach (var pos in placedLetterPositions.Keys)
        {
            var distance = Vector3.Distance(transform.position, pos);
            if (distance < minDist)
            {
                minDist = distance;
                indexToUse = index;
            }
            index++;
        }

        transform.localScale = new Vector3(0.4f, 0.4f, 1);
        var placedLetterCount = placedLetterPositions.Values.Count(x => x != null);

        if (placedLetterCount > indexToUse)
        {
            InsertLetterAndMoveOtherLetters(placedLetterPositions, indexToUse, this);
        }
        else
        {
            transform.position = placedLetterPositions.FirstOrDefault(x => x.Value == null).Key;
            placedLetterPositions[transform.position] = this;
            if (placedLetterPositions.Values.Count(x => x != null) >= 12) return;
            if (!IsSecondLetter && !IsFirstLetter)
            {
                Vector3 oldPos = playerLetterPositions.FirstOrDefault(x => x.Value == this).Key;
                playerLetterPositions[oldPos] = null;
            }
        }
    }

    private void InsertLetterAndMoveOtherLetters(Dictionary<Vector3, LetterBlock> placedLetterPos, int index, LetterBlock newLetter)
    {
        LetterBlock previous = newLetter;
        foreach (var key in placedLetterPos.Keys.ToList().Skip(index))
        {
            if (previous == null) break;
            previous.transform.position = key;
            var current = placedLetterPos[key];
            placedLetterPos[key] = previous;
            previous = current;
        }
        if (!IsSecondLetter && !IsFirstLetter)
        {
            Vector3 oldPos = playerLetterPositions.FirstOrDefault(x => x.Value == this).Key;
            playerLetterPositions[oldPos] = null;
        }
        if (previous == null) return; // else set last letter to playerpos
        Vector3 pos;
        if (previous.IsFirstLetter) 
            pos = firstLetterPosition;
        else if (previous.IsSecondLetter) 
            pos = secondLetterPosition;
        else
        {
            pos = playerLetterPositions.FirstOrDefault(x => x.Value == null).Key;
            playerLetterPositions[pos] = previous;
        }
        previous.transform.localScale = new Vector3(0.5f, 0.5f, 1);
        previous.transform.position = pos;
    }
}