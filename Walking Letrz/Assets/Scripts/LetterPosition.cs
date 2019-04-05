using UnityEngine;

public class LetterPosition
{
    // Start is called before the first frame update
    internal LetterBlock LetterBlock { get; set; }
    internal readonly Vector3 Position;

    internal LetterPosition(Vector3 position, LetterBlock letterBlock)
    {
        Position = position;
        LetterBlock = letterBlock;
    }

    internal bool ContainsLetter()
    {
        return LetterBlock != null;
    }

    internal void RemoveLetter()
    {
        LetterBlock = null;
    }

    internal void AddLetter(LetterBlock letter)
    {
        if (letter != null) letter.transform.position = Position;
        LetterBlock = letter;
    }
}
