using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class LetterManager : MonoBehaviour
{
    public StartingLetters StartingLettersClass;
    public PlayerLetters PlayerLettersClass;

    public char[] PlacedLetters{get;set;}

    public Dictionary<string, object> charactersValues;

    private void Start()
    {
        InitCharactersValues();
        Instantiate(StartingLettersClass);
        Instantiate(PlayerLettersClass);
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
