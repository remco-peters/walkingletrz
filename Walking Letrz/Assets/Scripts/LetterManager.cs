using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using Assets.Scripts;
using UnityEngine;
using UnityEngine.Events;

public class LetterManager : MyMonoBehaviour
{
    public StartingLetters StartingLettersClass;
    public PlayerLetters PlayerLettersClass;
    public LetterBlock LetterBlockObject;
    public LetterBlock StartingLetterBlockObject;
    public RemoveWordBtn RemoveWordBtnClass;
    public PlaceWordBtn PlaceWordBtnClass;

    private Dictionary<Vector3, LetterBlock> PlacedLetterPositions { get;set; } = new Dictionary<Vector3, LetterBlock>();
    private Dictionary<Vector3, LetterBlock> PlayerLetterPositions{get; set; } = new Dictionary<Vector3, LetterBlock>();
    

    public GameObject LetterBoard { get; set; }
    public MyPlayer Player { get; set; }

    private Vector3 lastLetterPosition = new Vector3(-2.5f, -2.5f);

    public List<LetterBlock> PlacedLetters { get; set; } = new List<LetterBlock>();

    public Dictionary<string, object> CharactersValues { get; set; }

    private List<string> PlacedWords { get; set; } = new List<string>();

    private StartingLetters StartingLetters { get; set; }

    private string[] AllWords { get; set; }

    private string MadeWord;

    private void Start()
    {
        InitCharactersValues();
        InstantiateStartingLetters();
        InstantiatePlayerLetters();
        InitAllWords();
    }

    private void InstantiatePlayerLetters()
    {
        PlayerLetters playerLetters = Spawn(PlayerLettersClass, this, arg0 => { arg0.letterManager = this; });
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

        PlaceWordBtn placeWordBtn = Instantiate(PlaceWordBtnClass);
        placeWordBtn.OnPlaceBtnTouched += PlaceWord;
    }

    private void RemoveAllLetters()
    {
        foreach (LetterBlock block in PlacedLetters)
        {
            Vector3 pos = new Vector3();
            if (block.IsFirtsLetter)
            {
                pos = new Vector3(-2.5f, -2.5f);
            }
            else if (block.IsSecondLetter)
            {
                pos = new Vector3(-1.7f, -2.5f);
            }
            else
            {  
                pos = PlayerLetterPositions.FirstOrDefault(x => x.Value == null).Key;
                PlayerLetterPositions[pos] = block;
            }
            block.transform.localScale= new Vector3(0.5f, 0.5f, 1);
            block.transform.position = pos;
        }
        PlacedLetters.RemoveAll(x => true);
    }

    private void PlaceWord()
    {
        // Alleen wanneer mag versturen
        if (Player.CanMove)
        {
            foreach (LetterBlock block in PlacedLetters)
            {
                MadeWord += block.GetComponentInChildren<TextMesh>().text;
            }

            bool isWord = CheckWord(MadeWord.ToLower(), out long points);
            if (isWord)
            {
                // Timer aanzetten zodat er 10 seconden niet gedrukt kan worden
                Player.CanMove = false;
                Player.StartCooldown();
                RemoveAllLettersFromPlayerBoard();
                PlacedLetters.RemoveAll(x => true);
                PlaceWordInGameBoard();

                // Nieuwe letters genereren op lege plekken?
                AddLetters(MadeWord.Length - 2);
                ChangeFixedLetters();
            }
        }
        else
        {
            Debug.Log("Cant move yet: " + Player.CoolDownTime + " seconds remaining");
        }
        MadeWord = "";
    }

    private void AddLetters(int amount)
    {
        char[] letters = GetLetters(amount);
        for (int i = 0; i < amount; i++)
        {
            LetterBlock block = Instantiate(LetterBlockObject);
            Vector3 pos = PlayerLetterPositions.FirstOrDefault(x => x.Value == null).Key;
            PlayerLetterPositions[pos] = block;
            block.transform.position = pos;
            block.GetComponentInChildren<TextMesh>().text = letters[i].ToString().ToUpper();
            block.OnLetterTouched += LetterTouched;
        }
    }

    private void InstantiateStartingLetters()
    {
        StartingLetters startingLetters =  Spawn(StartingLettersClass, this, arg0 => { arg0.LetterManager = this;});
        LetterBlock startingLetterBlock = Instantiate(StartingLetterBlockObject, lastLetterPosition, new Quaternion());
        startingLetterBlock.GetComponentInChildren<TextMesh>().text = startingLetters.firstLetter.ToUpper();
        startingLetterBlock.OnLetterTouched += LetterTouched;
        lastLetterPosition.x += 0.8f;
        startingLetterBlock.IsFirtsLetter = true;

        startingLetterBlock = Instantiate(StartingLetterBlockObject, lastLetterPosition, new Quaternion());
        startingLetterBlock.GetComponentInChildren<TextMesh>().text = startingLetters.secondLetter.ToUpper();
        startingLetterBlock.OnLetterTouched += LetterTouched;
        lastLetterPosition.x += 0.8f;
        startingLetterBlock.IsSecondLetter = true;

        StartingLetters = startingLetters;
    }

    private void InitAllWords()
    {
        int lines = File.ReadLines(@"woordenlijst.txt").Count();
        AllWords = new string[lines];
        using (StreamReader r = File.OpenText(@"woordenlijst.txt"))
        {
            for (int i = 0; i < lines; i++)
            {
                AllWords[i] = r.ReadLine();
            }
        }
    }

    public long CalculatePoints(string word)
    {
        long value = 0;
        foreach (var letter in word)
        {
            value += (long) CharactersValues.First(x => x.Key == letter.ToString()).Value;
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
                CharactersValues = item.Value as Dictionary<string, object>;
            }
        }
    }

    public bool CheckWord(string word, out long points)
    {
        points = CalculatePoints(word);
        if (!word.Contains(StartingLetters.firstLetter) || !word.Contains(StartingLetters.secondLetter))
        {
            Debug.Log("Word does not contain the two letters");
            return false;
        }
        if(word.IndexOf(StartingLetters.firstLetter, StringComparison.Ordinal) > word.LastIndexOf(StartingLetters.secondLetter, StringComparison.Ordinal))
        {
            Debug.Log("First letter is after second letter");
            return false;
        }

        if (!Exists(word))
        {
            Debug.Log("Word does not exist");
            return false;
        }

        if (PlacedWords.Contains(word))
        {
            Debug.Log("Word already placed");
            return false;
        }

        PlacedWords.Add(word);
        return true;

    }

    public char[] GetLetters(int amount)
    {
        char[] startingLetters = new char[15];
        List<char> availableLetters =new List<char> { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l',
            'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };

        List<char> lettersToChoseFrom = new List<char>();

        foreach (char c in availableLetters)
        {
            bool isVowel = "aeiou".IndexOf(c) >= 0;
            long val = (long) CharactersValues[c.ToString()];
            if (isVowel) val -= 10;
            for (int i = 0; i < 10 - val; i++)
            {
                lettersToChoseFrom.Add(c);
            }
        }
        for (int i = 0; i < amount; i++)
        {
            startingLetters[i] = lettersToChoseFrom[UnityEngine.Random.Range(0, lettersToChoseFrom.Count)];
        }
        return startingLetters;
    }

    private void StartLetterTouched(LetterBlock block, Vector3 pos)
    {
        if (PlacedLetters.Contains(block))
        {
            PlacedLetters.Remove(block);
            block.transform.localScale = new Vector3(0.5f, 0.5f, 1);
            block.transform.position = pos;
        }
        else if (PlacedLetters.Count < 12) // Anders niks doen; Maximaal 12 letterige woorden
        {
            block.transform.localScale = new Vector3(0.4f, 0.4f, 1);
            block.transform.position = new Vector3(-2.5f + 0.45f * PlacedLetters.Count, -1.7f);
            PlacedLetters.Add(block);
        }
    }

    private void LetterTouched(LetterBlock block)
    {
        if (block.IsFirtsLetter)
        {
            StartLetterTouched(block, new Vector3(-2.5f, -2.5f));
            return;
        }

        if (block.IsSecondLetter)
        {
            StartLetterTouched(block, new Vector3(-1.7f, -2.5f));
            return;
        }
        if (PlacedLetters.Contains(block))
        {
            PlacedLetters.Remove(block);
            block.transform.localScale = new Vector3(0.5f, 0.5f, 1);
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
    }

    private void ChangeFixedLetters()
    {
        StartingLetters.secondLetter = StartingLetters.firstLetter;
        var lastIndex = MadeWord.Length;
        StartingLetters.firstLetter = MadeWord[lastIndex - 1].ToString().ToLower();
        
        Vector3 startingLetterPos = new Vector3(-2.5f, -2.5f);

        LetterBlock startingLetterBlock = Instantiate(StartingLetterBlockObject, startingLetterPos, new Quaternion());
        startingLetterBlock.GetComponentInChildren<TextMesh>().text = StartingLetters.firstLetter.ToUpper();
        startingLetterBlock.OnLetterTouched += LetterTouched;
        startingLetterPos.x += 0.8f;
        startingLetterBlock.IsFirtsLetter = true;

        startingLetterBlock = Instantiate(StartingLetterBlockObject, startingLetterPos, new Quaternion());
        startingLetterBlock.GetComponentInChildren<TextMesh>().text = StartingLetters.secondLetter.ToUpper();
        startingLetterBlock.OnLetterTouched += LetterTouched;
        startingLetterBlock.IsSecondLetter = true;
    }    

    public bool Exists(string word)
    {
        return AllWords.Contains(word);
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

    private void RemoveAllLettersFromPlayerBoard()
    {
        foreach(LetterBlock block in PlacedLetters)
        {
            Destroy(block.gameObject);
        }
    }

    private void PlaceWordInGameBoard()
    {
        string lastWord = PlacedWords[PlacedWords.Count - 1];
    }
}