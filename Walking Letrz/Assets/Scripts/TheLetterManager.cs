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
        public StartingLetters StartingLetters;
        public char[] FirstPlayerLetters{get;set;}
        private System.Random random;

        public void Start()
        {
            random = new System.Random();
            InitCharactersValues();
            InitAllWords();
            InitStartingLetters();
        }

        public void InitStartingLetters()
        {
            FirstPlayerLetters = GetLetters(15);
            FirstLetter = GetLetters(1)[0];
            SecondLetter = GetLetters(1)[0];
        }

        public char GetVowel()
        {
            string vowels = "aeiou";
            return vowels[random.Next(0, vowels.Length)];
        }

        public char GetConsonant()
        {
            string consonants = "bcdfghjklmnpstvw";
            return consonants[random.Next(0, consonants.Length)];
        }

        public char[] GetLetters(int amount, List<char> currentLetters = null)
        {
            currentLetters = currentLetters ?? new List<char>();
            List<char> availableLetters =new List<char>
            { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 
                'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };

            List<char> lettersToChoseFrom = new List<char>();

            foreach (char c in availableLetters)
            {
                bool isVowel = "aeiou".IndexOf(c) >= 0;
                long val = CharactersValues[c];
                if (isVowel) val -= 10;
                for (int i = 0; i < 8 - val; i++)
                {
                    lettersToChoseFrom.Add(c);
                }
            }
            char[] startingLetters = new char[amount];
            for (int i = 0; i < amount; i++)
            {
                while (startingLetters[i] == default(char))
                {
                    int letterCount = 0;
                    char letter = lettersToChoseFrom[random.Next(0, lettersToChoseFrom.Count)];
                    letterCount += startingLetters.Count(x => x == letter);
                    letterCount += currentLetters.Count(x => x == letter);
                    if (letterCount < 2){
                        startingLetters[i] = letter;
                    }
                }
            }
            return startingLetters;
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
            if (Application.isMobilePlatform)
                AllWords = new HashSet<string>(Woordenlijst.text.Split(new[] {"\r\n"}, StringSplitOptions.None));
            else
                AllWords = new HashSet<string>(Woordenlijst.text.Split(new[] {Environment.NewLine}, StringSplitOptions.None));
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

// Todo: Deze aanpassen aan nieuwe variant
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
        // Todo: Deze aanpassen aan de nieuwe variant
        public void PlaceWordInGameBoard(List<LetterBlock> blocks)
        {
            Vector3 firstLetterPositionWordList = new Vector3(-2.75f, 4.3f);
            firstLetterPositionWordList.y -= 0.45f * PlacedWords.Count;
            foreach(LetterBlock block in blocks)
            {
                if (block == null) continue;
                firstLetterPositionWordList.x += 0.45f;
                InstantiateLetterButton(block.GetLetter(), firstLetterPositionWordList, block.IsFirstLetter, block.IsSecondLetter);
            }
        }
    }
}
