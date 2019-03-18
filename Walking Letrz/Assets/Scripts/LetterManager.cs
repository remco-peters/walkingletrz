
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class LetterManager : MonoBehaviour
{
    public StartingLetters StartingLettersClass;
    public PlayerLetters PlayerLettersClass;
    public GameObject LetterBlockObject;
    public GameObject StartingLetterBlockObject;

    public GameObject LetterBoard { get; set; }

    private Vector3 lastLetterPosition = new Vector3(-2.5f, -2.5f);

    public char[] PlacedLetters{get;set;}

    public Dictionary<string, object> charactersValues { get; set; }

    private List<string> PlacedWords { get; set; }

    private void Start()
    {
        PlacedWords = new List<string>();
        InitCharactersValues();
        InstantiateStartingLetters();
        instantiatePlayerLetters();
        CheckWord("wouter", out long points);
        Debug.Log(points);
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

            GameObject letterBlock = Instantiate(LetterBlockObject, lastLetterPosition, new Quaternion());

            lastLetterPosition = letterBlock.transform.position;
            letterBlock.GetComponentInChildren<TextMesh>().text = letters[i].ToString().ToUpper();
        }
    }

    private void InstantiateStartingLetters()
    {
        StartingLetters startingLetters = Instantiate(StartingLettersClass);

        GameObject startingLetterBlock = Instantiate(StartingLetterBlockObject, lastLetterPosition, new Quaternion());        
        startingLetterBlock.GetComponentInChildren<TextMesh>().text = startingLetters.firstLetter.ToUpper();
        lastLetterPosition.x += 0.8f;

        startingLetterBlock = Instantiate(StartingLetterBlockObject, lastLetterPosition, new Quaternion());     
        startingLetterBlock.GetComponentInChildren<TextMesh>().text = startingLetters.secondLetter.ToUpper();
        lastLetterPosition.x += 0.8f;
    }

    
    public long CalculatePoints(string word){
        long value = 0;
        foreach(var letter in word)
        {
            value += (long)charactersValues.First(x => x.Key == letter.ToString()).Value;
        }
        return value;
    }

    public void InitCharactersValues()
    {
        using (StreamReader r = new StreamReader("settings.json"))
        {
            string json = r.ReadToEnd();
            var items = (Dictionary<string, object>)MiniJSON.Json.Deserialize(json);
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
        //todo check if word is valid
        if (!PlacedWords.Contains(word))
        {
            PlacedWords.Add(word);
            return true;
        }
        return false;
    }
}
