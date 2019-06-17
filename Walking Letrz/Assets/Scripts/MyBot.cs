using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class MyBot : Player
    {
        // Start is called before the first frame update
        public PlayerManager playerManager;
        public Difficulty difficulty;
        private List<char> Letters;
        private bool hasFoundWord;
        private char FirstLetter => char.ToLower(TheLetterManager.FirstLetter);
        private char SecondLetter => char.ToLower(TheLetterManager.SecondLetter);     
        public Material FixedLetterOtherPlayerMaterial {get;set; }
        public Material PlayerLetterOtherPlayerMaterial{get;set; }

        new void Start()
        {
            hasFoundWord = false;
            Letters = TheLetterManager.FirstPlayerLetters.ToList();
            base.Start();
        }

        // Update is called once per frame
        new void Update()
        {
            if (CanMove)
            {
                if (!hasFoundWord)
                {
                    FindWord();
                    hasFoundWord = true;
                }
            }
            base.Update();
        }
        
        private void FindWord()
        {
            List<string> possibleWords = new List<string>();
            int prefferedMaxWordLength;
            if (difficulty == Difficulty.Easy) prefferedMaxWordLength = Random.Range(4, 5);
            else if (difficulty == Difficulty.Medium) prefferedMaxWordLength = Random.Range(4, 7);
            else prefferedMaxWordLength = Random.Range(6, 12);
            foreach (var word in TheLetterManager.AllWords)
            {
                if (word.Length <= prefferedMaxWordLength && CheckWord(word))
                    possibleWords.Add(word);
            }

            string foundWord = "";
            if (possibleWords.Any())
                foundWord = possibleWords[Random.Range(0, possibleWords.Count())];

            float timeRemaining;
            if (difficulty == Difficulty.Easy) timeRemaining = TheLetterManager.CalculatePoints(foundWord) + 5;
            else if (difficulty == Difficulty.Medium) timeRemaining = TheLetterManager.CalculatePoints(foundWord);
            else timeRemaining = TheLetterManager.CalculatePoints(foundWord);
            StartCoroutine(WaitToPlaceWord(timeRemaining, foundWord));
        }

        private IEnumerator WaitToPlaceWord(float time, string word)
        {
            yield return new WaitForSeconds(time);
            PlaceWord(word);
        }

        private void PlaceWord(string foundWord)
        {       
            hasFoundWord = false;
            if (foundWord == "")
            {
                Letters = TheLetterManager.GetLetters(TheLetterManager.FirstPlayerLetters.Count()).ToList();
                return;
            }
            playerManager.NextTurn();
            ChangeLetters(foundWord, foundWord.IndexOf(FirstLetter), foundWord.LastIndexOf(SecondLetter));
            long points = TheLetterManager.CalculatePoints(foundWord);
            EarnedPoints += points;
            PlacedInBoard(foundWord, foundWord.IndexOf(FirstLetter), foundWord.LastIndexOf(SecondLetter));
            LetterManager.ChangeFixedLetters(foundWord, true);
            TheLetterManager.PlacedWords.Add(foundWord);
            LetterManager.GameBoardWordContainer.transform.parent.transform.parent.GetComponent<GameboardScroll>().ScrollDownBar();
            int bestWordIndex = BestWordsThisGame.Count(word => word.points > points);
            BestWordsThisGame.Insert(bestWordIndex, new Word(foundWord, points));   
        }

        private void ChangeLetters(string word, int firstLetterIndex, int secondLetterIndex)
        {
            for (int i = 0; i < word.Length; i++)
            {
                if (i == firstLetterIndex || i == secondLetterIndex) continue;
                Letters.Remove(word[i]);
            }
            Letters.AddRange(TheLetterManager.GetLetters(word.Length));
        }

        private bool CheckWord(string word)
        {
            int firstLetterIndex = word.IndexOf(FirstLetter);
            int secondLetterIndex = word.LastIndexOf(SecondLetter);
            if (firstLetterIndex == -1 || secondLetterIndex == -1 || firstLetterIndex >= secondLetterIndex) return false;
            if (TheLetterManager.PlacedWords.Contains(word)) return false;
            List<char> availableLetters = Letters.ToList();
            for (int i = 0; i < word.Length; i++)
            {
                if (i == firstLetterIndex || i == secondLetterIndex) continue;
                int index = availableLetters.IndexOf(word[i]);
                if (index == -1) return false;
                availableLetters.RemoveAt(index);
            }
            return true;
        }

        public void PlacedInBoard(string word, int firstLetterIndex, int secondLetterIndex)
        {
            // Insantiate wordHolder
            GameObject wordHolder = Instantiate(LetterManager.GameBoardWordHolder);
            for (int i = 0; i < 12; i++)
            {
                LetterBlock block = null;
                if (i < word.Length) 
                    block = LetterManager.InstantiateLetterButton(word[i], i == firstLetterIndex, i == secondLetterIndex);

                if (block != null)
                {
                    block.GetComponent<Image>().material = block.IsWalkingLetter() ? FixedLetterOtherPlayerMaterial : PlayerLetterOtherPlayerMaterial;
                    block.transform.SetParent(wordHolder.transform, false);
                    block.GetComponent<Button>().interactable = false;
                }
                else
                {
                    GameObject emptyPlaceHolder = Instantiate(LetterManager.PlaceHolderObject);
                    emptyPlaceHolder.transform.SetParent(wordHolder.transform, false);
                }
            }
            wordHolder.transform.SetParent(LetterManager.GameBoardWordContainer.transform, false);
        }
    }
}
