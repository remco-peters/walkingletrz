
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.UIElements;
using Button = UnityEngine.UI.Button;

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

    private void Start()
    {
        InstantiateStartingLetters();
        InstantiateStartingLetters();
        instantiatePlayerLetters();
    }

    private void instantiatePlayerLetters()
    {
        PlayerLetters playerLetters = Instantiate(PlayerLettersClass);
        char[] letters = playerLetters.getLetters();
        for (int i = 0; i < letters.Length; i++)
        {
            if (i > 0)
                lastLetterPosition.x += 0.80f;
            if (i > 0 && i % 7 == 0)
            {
                lastLetterPosition.x = -2.5f;
                lastLetterPosition.y -= 0.75f;
            }

            var letterBlock = Instantiate(LetterBlockObject, lastLetterPosition, new Quaternion());

            lastLetterPosition = letterBlock.transform.position;
            letterBlock.GetComponentInChildren<TextMesh>().text = letters[i].ToString().ToUpper();
        }
    }

    private void InstantiateStartingLetters()
    {
        StartingLetters startingLetters = Instantiate(StartingLettersClass);

        var startingLetterBlock = Instantiate(LetterBlockObject, lastLetterPosition, new Quaternion());
        
        startingLetterBlock.GetComponentInChildren<TextMesh>().text = startingLetters.firstLetter;
    }

    
    public int CalculatePoints(string[] letters){
        int value = 0;
        foreach(string letter in letters)
        {
            value += (byte)charactersValues.First(x => x.Key == letter).Value;
        }
        return value;
    }

    public void InitCharactersValues(){

        using (StreamReader r = new StreamReader("Assets\\Scripts\\settings.json"))
        {
            string json = r.ReadToEnd();
            var items = (Dictionary<string, object>)MiniJSON.Json.Deserialize(json);
            foreach (var item in items)
            {
                if (item.Key != "lettervalues") continue;
                charactersValues = (Dictionary<string, object>)item.Value;
            }
        }
    }

    public bool CheckWord(string word){
        //todo check if word is in list
        return true;
    }
}
