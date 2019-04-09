using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
    public class TheLetterManager : MyMonoBehaviour
    {
        public char FirstLetter { get; set; }
        public char SecondLetter { get; set; }
        public Dictionary<char, long> CharactersValues { get; } = new Dictionary<char, long>();
        public HashSet<string> AllWords {get; set; }
        public TextAsset JsonAsset;
        public TextAsset Woordenlijst;
        public List<string> PlacedWords { get;set; } = new List<string>();
        public LetterBlock LetterBlockObject;
        public Material WalkingLetrMaterial;
        public Material NormalLetrMaterial;

        public void Start()
        {
            InitCharactersValues();
            InitAllWords();
        }

        public void InitCharactersValues()
        {
            string json = JsonAsset.text;
            var items = (Dictionary<string, object>) MiniJSON.Json.Deserialize(json);
            foreach (var item in items)
            {
                if (item.Key != "lettervalues") continue;
                foreach (var val in item.Value as Dictionary<string, object> ?? new Dictionary<string, object>())
                {
                    CharactersValues[val.Key[0]] = (long) val.Value;
                }
            }
        }
        private void InitAllWords()
        {
            AllWords = new HashSet<string>(Woordenlijst.text.Split(new[] { Environment.NewLine },StringSplitOptions.None));
            Woordenlijst = null;
        }

        public bool CheckWord(string word, out long points, List<LetterPosition> placedLetters)
        {
            word = word.ToLower();
            int firstLetterIndex = -1;
            int secondLetterIndex = -1;
            points = CalculatePoints(word);
            int i = 0;
            foreach (var letterBlock in placedLetters.Select(x => x.LetterBlock).ToList())
            {
                if (letterBlock == null) continue;
                if (letterBlock.IsFirstLetter)
                    firstLetterIndex = i;
                else if (letterBlock.IsSecondLetter)
                    secondLetterIndex = i;
                i++;
            }
            if (firstLetterIndex < 0 || secondLetterIndex < 0)
            {
                Debug.Log("Word does not contain the two letters");
            }
            else if(firstLetterIndex > secondLetterIndex)
            {
                Debug.Log("First letter is after second letter");
            }
            else if (!Exists(word))
            {
                Debug.Log($"Word does not exist. Word: {word}");
            }
            else if (PlacedWords.Contains(word))
            {
                Debug.Log("Word already placed");
            }
            else
            {               
                PlacedWords.Add(word);
                return true;
            }
            return false;
        }

        public long CalculatePoints(string word)
        {
            long value = 0;
            foreach (var letter in word)
            {
                value += CharactersValues.FirstOrDefault(x => x.Key == letter).Value;
            }
            if (word.Length >= 5 && word.Length <= 7) value = (long)(value * 1.5);
            else if (word.Length >= 8 && word.Length <= 10) value *= 2;
            else if (word.Length == 11) value = (long)(value * 2.5);
            else if (word.Length == 12) value *= 3;
            return value;
        }      
        public bool Exists(string word)
        {
            return AllWords.Contains(word);
        }
       
        public void PlaceWordInGameBoard(char[] placedLetters)
        {
            Vector3 firstLetterPositionWordList = new Vector3(-2.75f, 4.3f);
            firstLetterPositionWordList.y -= 0.45f * PlacedWords.Count;
            foreach(char c in placedLetters)
            {
                firstLetterPositionWordList.x += 0.45f;
                InstantiateLetterButton(c, firstLetterPositionWordList);
            }
        }

        public LetterBlock InstantiateLetterButton(char letter, Vector3 pos, bool isFirstLetter = false, bool isSecondLetter = false)
        {
            return Spawn(LetterBlockObject, this, lttrblock =>
            {
                if (isFirstLetter || isSecondLetter) lttrblock.GetComponent<MeshRenderer>().material = WalkingLetrMaterial;
                else lttrblock.GetComponent<MeshRenderer>().material = NormalLetrMaterial;
                lttrblock.IsFirstLetter = isFirstLetter;
                lttrblock.IsSecondLetter = isSecondLetter;
                lttrblock.GetComponentsInChildren<TextMesh>()[0].text = letter.ToString().ToUpper();
                lttrblock.GetComponentsInChildren<TextMesh>()[1].text = CharactersValues.First(x => x.Key == char.ToLower(letter)).Value.ToString();
                lttrblock.transform.position = pos;
            });
        }
    }
}
