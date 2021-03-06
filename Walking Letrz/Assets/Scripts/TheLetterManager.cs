﻿using System;
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
        private Dictionary<char, long> CharacterOcurences { get; } = new Dictionary<char, long>();
        public HashSet<string> AllWords {get; set; }
        public TextAsset JsonAsset;
        public TextAsset Woordenlijst;
        public List<string> PlacedWords { get;set; } = new List<string>();
        public LetterBlock LetterBlockObject;
        public Material WalkingLetrMaterial;
        public Material NormalLetrMaterial;
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

        /// <summary>
        /// To init all the starting letters, depending on difficulty
        /// </summary>
        public void InitStartingLetters()
        {
            FirstLetter = GetVowelOrConsonant(GameInstance.instance.difficulty == Difficulty.Medium);
            SecondLetter = GetVowelOrConsonant(GameInstance.instance.difficulty != Difficulty.Hard);
            switch (GameInstance.instance.difficulty)
            {
                case Difficulty.Easy:
                    FirstPlayerLetters = GetLetters(19);
                    break;
                case Difficulty.Medium:
                    FirstPlayerLetters = GetLetters(15);
                    break;
                default:
                    FirstPlayerLetters = GetLetters(12);
                    break;
            }
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
                    for (int i = 0; i < CharacterOcurences[key] * 2; i++)
                    {
                        lettersToChoseFrom.Add(key);
                    }
            }
            return lettersToChoseFrom[random.Next(0, lettersToChoseFrom.Count)];
        }

        /// <summary>
        /// To get the right amount of letters
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="currentLetters"></param>
        /// <returns>char array</returns>
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
                    if (vowelsCount < 3)
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
            startingLetters = startingLetters.OrderBy(x => random.Next()).ToArray(); //shuffle 
            return startingLetters;
        }

        /// <summary>
        /// Init the values of the letters
        /// </summary>
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

        /// <summary>
        /// Use the words from our wordslist
        /// </summary>
        private void InitAllWords()
        {            
            if (Application.isMobilePlatform)
                AllWords = new HashSet<string>(Woordenlijst.text.Split(new[] {"\r\n"}, StringSplitOptions.None));
            else
                AllWords = new HashSet<string>(Woordenlijst.text.Split(new[] {Environment.NewLine}, StringSplitOptions.None));
            Woordenlijst = null;
        }

        /// <summary>
        /// Method to check if the word is valid
        /// </summary>
        /// <param name="word"></param>
        /// <param name="points"></param>
        /// <param name="placedLetters"></param>
        /// <param name="p"></param>
        /// <returns>bool</returns>
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
            else if (PlacedWords.Contains(word) || GameState.PlacedWordsInThisGame.Contains(word))
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

        /// <summary>
        /// Method to calculate earned points
        /// </summary>
        /// <param name="word"></param>
        /// <returns>long</returns>
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
        
        /// <summary>
        /// Method to check if the word exists in our wordslist
        /// </summary>
        /// <param name="word"></param>
        /// <returns>bool</returns>
        public bool Exists(string word)
        {
            return AllWords.Contains(word);
        }
    }
}
