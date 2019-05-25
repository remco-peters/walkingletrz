using System;
using System.Data.Common;
using UnityEngine;

[Serializable]
public class LetterPosition
{
    
    public byte Id { get; set; }
    // Start is called before the first frame update
    public LetterBlock LetterBlock { get; set; }
    private int Row;
    private int OldIndex;
    private int CurrentIndex;

    public static object Deserialize(byte[] data)
    {
        var result = new LetterPosition {Id = data[0], Row = data[1], OldIndex = data[2], CurrentIndex = data[3]};
        return result;
    }

    public static byte[] Serialize(object letterPosition)
    {
        var lp = (LetterPosition) letterPosition;
        if (lp.LetterBlock == null)
        {
            return new byte[] {lp.Id, (byte) lp.Row, (byte) lp.OldIndex, (byte) lp.CurrentIndex};
        }
        else
        {
            return new byte[] {lp.Id, (byte) lp.Row, (byte) lp.OldIndex, (byte) lp.CurrentIndex};
        }
    }
    
    internal LetterPosition(int row, int index, LetterBlock letterBlock)
    {
        Id = 1;
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
