using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

public class LetterManager : MonoBehaviour
{
    public StartingLetters StartingLettersClass;
    public PlayerLetters PlayerLettersClass;
    public LetterBlock LetterBlockObject;
    public LetterBlock StartingLetterBlockObject;
    public RemoveWordBtn RemoveWordBtnClass;

    private Dictionary<Vector3, LetterBlock> PlacedLetterPositions { get;set; } = new Dictionary<Vector3, LetterBlock>();
    private Dictionary<Vector3, LetterBlock> PlayerLetterPositions{get; set; } = new Dictionary<Vector3, LetterBlock>();

    public GameObject LetterBoard { get; set; }

    private Vector3 lastLetterPosition = new Vector3(-2.5f, -2.5f);

    public List<LetterBlock> PlacedLetters { get; set; } = new List<LetterBlock>();

    public Dictionary<string, object> charactersValues { get; set; }

    private List<string> PlacedWords { get; set; } = new List<string>();

    private void Start()
    {
        InitCharactersValues();
        InstantiateStartingLetters();
        instantiatePlayerLetters();
        bool isWord = CheckWord("bijenkorf", out long points);
        Debug.Log(points);
        Debug.Log(isWord);
    }

    private void instantiatePlayerLetters()
    {
        PlayerLetters playerLetters = Instantiate(PlayerLettersClass);
        char[] letters = playerLetters.getLetters();
        for (int i = 0; i < letters.Length; i++)
        {
            if (i > 0)
                lastLetterPosition.x += 0.80f;
            if (i == 5)
            {
                lastLetterPosition.x = -2.5f;
                lastLetterPosition.y -= 0.75f;
            }

            if (i == 12)
            {
                lastLetterPosition.x = -0.9f;
                lastLetterPosition.y -= 0.75f;
            }

            LetterBlock letterBlock = Instantiate(LetterBlockObject, lastLetterPosition, new Quaternion());
            letterBlock.IsLetterSet = false;
            PlayerLetterPositions.Add(lastLetterPosition, letterBlock);
            letterBlock.OnLetterTouched += LetterTouched; 
            lastLetterPosition = letterBlock.transform.position;
            letterBlock.GetComponentInChildren<TextMesh>().text = letters[i].ToString().ToUpper();
        }
        lastLetterPosition.x += 0.80f;
        RemoveWordBtn removeWordBtn = Instantiate(RemoveWordBtnClass);
        removeWordBtn.OnRemoveTouched += RemoveAllLetters;
    }

    private void RemoveAllLetters()
    {
        foreach (LetterBlock block in PlacedLetters)
        {           
            Vector3 pos = PlayerLetterPositions.FirstOrDefault(x => x.Value == null).Key;
            block.transform.localScale= new Vector3(0.5f, 0.5f, 1);
            PlayerLetterPositions[pos] = block;
            block.transform.position = pos;
        }
        PlacedLetters.RemoveAll(x => true);
    }

    private void InstantiateStartingLetters()
    {
        StartingLetters startingLetters = Instantiate(StartingLettersClass);

        LetterBlock startingLetterBlock = Instantiate(StartingLetterBlockObject, lastLetterPosition, new Quaternion());
        startingLetterBlock.GetComponentInChildren<TextMesh>().text = startingLetters.firstLetter.ToUpper();
        lastLetterPosition.x += 0.8f;

        startingLetterBlock = Instantiate(StartingLetterBlockObject, lastLetterPosition, new Quaternion());
        startingLetterBlock.GetComponentInChildren<TextMesh>().text = startingLetters.secondLetter.ToUpper();
        lastLetterPosition.x += 0.8f;
    }

    public long CalculatePoints(string word)
    {
        long value = 0;
        foreach (var letter in word)
        {
            value += (long) charactersValues.First(x => x.Key == letter.ToString()).Value;
        }

        return value;
    }

    public void InitCharactersValues()
    {
        using (StreamReader r = new StreamReader("settings.json"))
        {
            string json = r.ReadToEnd();
            var items = (Dictionary<string, object>) MiniJSON.Json.Deserialize(json);
            foreach (var item in items)
            {
                if (item.Key != "lettervalues") continue;
                charactersValues = item.Value as Dictionary<string, object>;
            }
        }
    }

    public bool CheckWord(string word, out long points)
    {
        points = CalculatePoints(word);
        if (!Exists(word)) return false;
        if (!PlacedWords.Contains(word))
        {
            PlacedWords.Add(word);
            return true;
        }

        return false;
    }

    private void LetterTouched(LetterBlock block)
    {
        if (PlacedLetters.Contains(block))
        {
            PlacedLetters.Remove(block);
            block.transform.localScale= new Vector3(0.5f, 0.5f, 1);
            Vector3 pos = PlayerLetterPositions.FirstOrDefault(x => x.Value == null).Key;
            PlayerLetterPositions[pos] = block;
            block.transform.position = pos;
        }

        else if (PlacedLetters.Count < 12) // Anders niks doen; Maximaal 12 letterige woorden
        {
            PlayerLetterPositions[block.transform.position] = null;
            block.transform.localScale = new Vector3(0.4f, 0.4f, 1);
            block.transform.position = new Vector3(-2.5f + 0.45f * PlacedLetters.Count, -1.7f);
            PlacedLetters.Add(block);
        }

        Debug.Log(PlacedLetters.Count);
        Debug.Log(PlacedLetters.ToString());
    }

    public bool Exists(string word)
    {
        using (StreamReader r = new StreamReader("woordenlijst.txt"))
        {
            string woorden = r.ReadToEnd();
            if (woorden.Contains(word)) return true;
        }

        return false;
        /* uit zipje lezen
        using (ZipArchive archive = ZipFile.OpenRead("woordenlijst.zip"))
        {
            foreach (ZipArchiveEntry entry in archive.Entries)
            {
                Debug.Log("TEST!@#");
                Stream r = entry.Open();
                return StreamHasString(r, word);
            }
        }
        return false;*/
    }

    private bool StreamHasString(Stream vStream, string word)
    {
        byte[] streamBytes = new byte[vStream.Length];

        int pos = 0;
        int len = (int) vStream.Length;
        while (pos < len)
        {
            int n = vStream.Read(streamBytes, pos, len - pos);
            pos += n;
        }

        string stringOfStream = Encoding.UTF32.GetString(streamBytes);
        if (stringOfStream.Contains(word))
        {
            return true;
        }

        return false;
    }
}