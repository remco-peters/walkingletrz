using UnityEngine;

public class LetterPosition
{
    // Start is called before the first frame update
    public LetterBlock LetterBlock { get; set; }
    private int Row;
    private int OldIndex;
    private int CurrentIndex;

    internal LetterPosition(int row, int index, LetterBlock letterBlock)
    {
        Row = row;
        CurrentIndex = index;
        LetterBlock = letterBlock;
    }

    public bool ContainsLetter()
    {
        return LetterBlock != null;
    }

    public void RemoveLetter()
    {
        LetterBlock = null;
        SetRow(0);
    }

    public int GetRow()
    {
        return Row;
    }

    public void SetRow(int row)
    {
        Row = row;
    }

    public int GetOldIndex()
    {
        return OldIndex;
    }

    public void SetIndex(int index)
    {
        OldIndex = index;
    }

    public int GetCurrentIndex()
    {
        return CurrentIndex;
    }

    public void SetCurrentIndex(int wbIndex)
    {
        CurrentIndex = wbIndex;
    }

    public void AddLetter(LetterBlock letter, int oldIndex, int row)
    {
        LetterBlock = letter;
        Row = row;
        OldIndex = oldIndex;
    }
}
