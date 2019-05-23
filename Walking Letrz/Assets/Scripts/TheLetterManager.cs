using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;

namespace Assets.Scripts
{
    public class TheLetterManager : MyMonoBehaviour
    {
        public char FirstLetter { get; set; }
        public char SecondLetter { get; set; }
        public Dictionary<char, long> CharactersValues { get; } = new Dictionary<char, long>();
        private Dictionary<char, long> CharacterOcurences { get; } = new Dictionary<char, long>();
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
            InitCharactersOcurenaceDictionary();
            InitAllWords();
            InitStartingLetters();
        }

        public void InitStartingLetters()
        {
            Difficulty difficulty = GameInstance.instance.difficulty;
            if (difficulty == Difficulty.Easy) FirstPlayerLetters = GetLetters(19);
            else if (difficulty == Difficulty.Medium) FirstPlayerLetters = GetLetters(15);
            else FirstPlayerLetters = GetLetters(12);
            char[] startingLetters = GetLetters(2);
            FirstLetter = startingLetters[0];
            SecondLetter =startingLetters[1];
            
        }

        private void InitCharactersOcurenaceDictionary()
        {
            string json = JsonAsset.text;
            var items = (Dictionary<string, object>) MiniJSON.Json.Deserialize(json);
            foreach (var item in items)
            {
                if (item.Key != "letterocurances") continue;
                foreach (var val in item.Value as Dictionary<string, object> ?? new Dictionary<string, object>())
                {
                    CharacterOcurences[val.Key[0]] = (long)val.Value;
                }
            }
        }

        public char GetVowelOrConsonant(bool? vowel)
        {
            string availableLetters;
            if (vowel != null)
            {
                availableLetters = (bool)vowel ? "aeiou" : "bcdfghjklmnpstvw";
            }
            else
            {
                availableLetters = "abcdefghijklmnopqrstuvwxyz";
            }
            List<char> lettersToChoseFrom = new List<char>();
            foreach (var key in CharacterOcurences.Keys)
            {
                if (availableLetters.IndexOf(key) != -1)
                    for (int i = 0; i < CharacterOcurences[key] * 3; i++)
                    {
                        lettersToChoseFrom.Add(key);
                    }
            }
            return lettersToChoseFrom[random.Next(0, lettersToChoseFrom.Count)];
        }

        public char[] GetLetters(int amount, List<char> currentLetters = null)
        {
            currentLetters = currentLetters ?? new List<char>();
            List<char> availableLetters = new List<char>();
            List<char> availableVowels = new List<char>();
            List<char> availableConsonants = new List<char>();
            string vowels = "aeiou";
            foreach(char key in CharacterOcurences.Keys)
            {
                for (int i = 0; i < CharacterOcurences[key]; i++)  
                {
                    if (vowels.IndexOf(key) != -1)
                        availableVowels.Add(key);
                    else 
                        availableConsonants.Add(key);
                    availableLetters.Add(key);
                }
            }
            char[] startingLetters = new char[amount];
            int vowelsCount = currentLetters.Count(x => vowels.IndexOf(x) != -1);
            for (int i = 0; i < amount; i++)
            {                         
                while (startingLetters[i] == default(char))
                {
                    char letter;
                    if (vowelsCount < 2 && i + currentLetters.Count() > 2)
                    {
                        letter = availableVowels[random.Next(0, availableVowels.Count)];
                        vowelsCount += 1;
                    } 
                    else if (vowelsCount > 5)       
                        letter = availableConsonants[random.Next(0, availableConsonants.Count)];
                    else
                    {
                        letter = availableLetters[random.Next(0, availableLetters.Count)];
                        if (vowels.IndexOf(letter) != -1) vowelsCount += 1;
                    }                          

                    int letterCount = startingLetters.Count(x => x == letter);
                    letterCount += currentLetters.Count(x => x == letter);
                    if (letterCount >= 2) continue;
                    startingLetters[i] = letter;
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

        public bool CheckWord(string word, out long points, List<LetterPosition> placedLetters, MyPlayer p = null)
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
                if(p != null)
                {
                    p.InfoText = I2.Loc.LocalizationManager.GetTranslation("not_both_fixed_letters");
                }
                Debug.Log("Word does not contain the two letters");
            }
            else if(firstLetterIndex > secondLetterIndex)
            {
                if (p != null)
                {
                    p.InfoText = I2.Loc.LocalizationManager.GetTranslation("first_letter_after_second");
                }
                Debug.Log("First letter is after second letter");
            }
            else if (!Exists(word))
            {
                if (p != null)
                {
                    p.InfoText = I2.Loc.LocalizationManager.GetTranslation("info_word_does_not_exists");
                }
                Debug.Log($"Word does not exist. Word: {word}");
            }
            else if (PlacedWords.Contains(word) || GameState.PlacedWordsInThisGame.Contains(word.ToLower()))
            {
                if (p != null)
                {
                    p.InfoText = I2.Loc.LocalizationManager.GetTranslation("info_word_already_placed");
                }
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
