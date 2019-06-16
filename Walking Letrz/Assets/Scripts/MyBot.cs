using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class MyBot : Player
    {
        // Start is called before the first frame update
        public float timeRemaining;
        public PlayerManager playerManager;
        public Difficulty difficulty;
        private List<char> Letters;
        private bool hasFoundWord;
        int indexFirstLetter = -1;
        int indexSecondLetter = -1;
        string foundWord = "";

    

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
                timeRemaining -= Time.deltaTime;
                if (!hasFoundWord)
                {
                    FindWord();
                    hasFoundWord = true;
                }
                if (timeRemaining <= 0)
                {   
                    PlaceWord();
                    hasFoundWord = false;
                }
            }
            base.Update();
        }

        private void PlaceWord()
        {         
            if (foundWord == "")
            {
                Letters = TheLetterManager.GetLetters(TheLetterManager.FirstPlayerLetters.Count()).ToList();
                hasFoundWord = false;
                return;
            }
            playerManager.NextTurn();
            ChangeLetters(foundWord, indexFirstLetter, indexSecondLetter);
            long points = TheLetterManager.CalculatePoints(foundWord);
            EarnedPoints += points;
            PlacedInBoard(foundWord, indexFirstLetter, indexSecondLetter);
            LetterManager.ChangeFixedLetters(foundWord, true);
            TheLetterManager.PlacedWords.Add(foundWord);
            LetterManager.GameBoardWordContainer.transform.parent.transform.parent.GetComponent<GameboardScroll>().ScrollDownBar();
            int bestWordIndex = BestWordsThisGame.Count(word => word.points > points);
            BestWordsThisGame.Insert(bestWordIndex, new Word(foundWord, points));   
        }

        private void FindWord()
        {
            char firstLetter = char.ToLower(TheLetterManager.FirstLetter);
            char secondLetter = char.ToLower(TheLetterManager.SecondLetter);
            List<string> possibleWords = new List<string>();
            foundWord = "";
            int prefferedMaxWordLength;
            if (difficulty == Difficulty.Easy) prefferedMaxWordLength = Random.Range(4, 5);
            else if (difficulty == Difficulty.Medium) prefferedMaxWordLength = Random.Range(4, 7);
            else prefferedMaxWordLength = Random.Range(6, 12);
            foreach (var word in TheLetterManager.AllWords)
            {
                if (word.Length <= prefferedMaxWordLength && CheckWord(word, word.IndexOf(firstLetter), word.LastIndexOf(secondLetter))) possibleWords.Add(word);
            }
            if (possibleWords.Any())
                foundWord = possibleWords[Random.Range(0, possibleWords.Count())];
            indexFirstLetter = foundWord.IndexOf(firstLetter);
            indexSecondLetter = foundWord.LastIndexOf(secondLetter);
            if (difficulty == Difficulty.Easy) timeRemaining = TheLetterManager.CalculatePoints(foundWord) + 5;
            else if (difficulty == Difficulty.Medium) timeRemaining = TheLetterManager.CalculatePoints(foundWord);
            else timeRemaining = TheLetterManager.CalculatePoints(foundWord);
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

        private bool CheckWord(string word, int firstLetterIndex, int secondLetterIndex)
        {
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

        
        public Material FixedLetterOtherPlayerMaterial {get;set; }
        public Material PlayerLetterOtherPlayerMaterial{get;set; }

        public void PlacedInBoard(string word, int firstLetterIndex, int secondLetterIndex)
        {
            // Insantiate wordHolder
            GameObject wordHolder = Instantiate(LetterManager.GameBoardWordHolder);
            List<LetterBlock> blocks = new List<LetterBlock>();
            for (int i = 0; i < word.Length; i++)
            {
                blocks.Add(LetterManager.InstantiateLetterButton(word[i], i == firstLetterIndex, i == secondLetterIndex));
            }
            while (blocks.Count < 12)
            {
                blocks.Add(null);
            }
            // Walk through all the letters placed
            foreach (LetterBlock block in blocks)
            {
                if (block != null)
                {
                    if (block.IsFirstLetter || block.IsSecondLetter)
                        block.GetComponent<Image>().material = FixedLetterOtherPlayerMaterial;
                    else
                        block.GetComponent<Image>().material = PlayerLetterOtherPlayerMaterial;
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
