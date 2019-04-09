using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;
using UnityEngine;
using UnityEngine.Events;

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
    private List<LetterPosition> playerLetterPositions;

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
        else if (timer < timeDelay && Vector2.Distance(oldPosition, transform.position) < 0.3)
        {
            OnLetterTouched(this);
        }
        else
        {
            transform.position = oldPosition;
        }
        timer = 0;
    }

    public void SetToOldPosition(List<LetterPosition> placedLetterPositions)
    {
        Vector3 pos;
        if (placedLetterPositions.FirstOrDefault(x => x.LetterBlock == this) != null)
        {
            playerLetterPositions.FirstOrDefault(x => x.LetterBlock == this)?.RemoveLetter();
        }
        else
        {
            placedLetterPositions.FirstOrDefault(x => x.LetterBlock == this)?.RemoveLetter();
        }
        if (IsFirstLetter) pos = firstLetterPosition;
        else if (IsSecondLetter) pos = secondLetterPosition;
        else
        {
            if (playerLetterPositions.FirstOrDefault(x => x.LetterBlock == null) != null)
            {
                playerLetterPositions.FirstOrDefault(x => x.LetterBlock == null)?.AddLetter(this);
            }
            else
            {
                playerLetterPositions.FirstOrDefault(x => x.LetterBlock == this)?.AddLetter(this);
            }
        }
        transform.localScale = new Vector3(0.5f, 0.5f, 1);
    }

    public void LetterDragged(List<LetterPosition> placedLetterPositions, List<LetterPosition> playerLetterPositions)
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

        foreach (var pos in placedLetterPositions.Select(x => x.Position))
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
        if (placedLetterPositions.Count(x => x != null) > indexToUse)
        {
            InsertLetterAndMoveOtherLetters(placedLetterPositions, indexToUse, this);
        }
        else
        {
            placedLetterPositions.FirstOrDefault(x => x.LetterBlock == null)?.AddLetter(this);
            if (placedLetterPositions.Count(x => x.LetterBlock != null) >= 12) return;
            if (!IsWalkingLetter())
            {
                playerLetterPositions.FirstOrDefault(x => x.LetterBlock == this)?.RemoveLetter();
            }
        }
    }

    private void InsertLetterAndMoveOtherLetters(IEnumerable<LetterPosition> placedLetterPos, int index, LetterBlock newLetter)
    {
        if (!newLetter.IsWalkingLetter()) playerLetterPositions.FirstOrDefault(x => x.LetterBlock == this)?.RemoveLetter();
        foreach (var position in placedLetterPos.Skip(index))
        {
            if (newLetter == null) return;
            var current = position.LetterBlock;
            position.AddLetter(newLetter);
            newLetter = current;
        }
        if (newLetter == null) return; // else set last letter to playerpos
        if (newLetter.IsFirstLetter) newLetter.transform.position = firstLetterPosition;
        else if (newLetter.IsSecondLetter) newLetter.transform.position = secondLetterPosition;
        else playerLetterPositions.FirstOrDefault(x => x.LetterBlock == null)?.AddLetter(newLetter);       
        newLetter.transform.localScale = new Vector3(0.5f, 0.5f, 1);
    }

    internal bool IsWalkingLetter()
    {
        return IsFirstLetter || IsSecondLetter;
    }

    internal char GetLetter()
    {
        return GetComponentsInChildren<TextMesh>()[0].text[0];
    }
}