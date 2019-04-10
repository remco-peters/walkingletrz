using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
    public class MyBot : Player
    {
        // Start is called before the first frame update
        public float timeRemaining;
        public PlayerManager playerManager;
        private List<char> Letters;

        new void Start()
        {
            Letters = TheLetterManager.GetLetters(15).ToList();
            timeRemaining = Random.Range(2, 10);
            base.Start();
        }

        // Update is called once per frame
        new void Update()
        {
            if (CanMove)
            {
                timeRemaining -= Time.deltaTime;
                if (timeRemaining <= 0)
                {
                    char firstLetter = char.ToLower(TheLetterManager.FirstLetter);
                    char secondLetter = char.ToLower(TheLetterManager.SecondLetter);
                    int indexFirstLetter = -1;
                    int indexSecondLetter = -1;
                    string foundWord = "";
                    foreach (var word in TheLetterManager.AllWords)
                    {
                        indexFirstLetter = word.IndexOf(firstLetter);
                        indexSecondLetter = word.LastIndexOf(secondLetter);
                        if (indexFirstLetter == -1 || indexSecondLetter == -1 || indexFirstLetter >= indexSecondLetter || !CheckWord(word, indexFirstLetter, indexSecondLetter)) continue;
                        foundWord = word;
                        break;
                    }
                    playerManager.NextTurn();
                    timeRemaining = Random.Range(2, 10);
                    if (foundWord == "")
                    {
                        Letters = TheLetterManager.GetLetters(15).ToList();
                        return;
                    }
                    ChangeLetters(foundWord, indexFirstLetter, indexSecondLetter);
                    EarnedPoints += 10;
                    PlacedInBoard(foundWord, indexFirstLetter, indexSecondLetter);
                    LetterManager.ChangeFixedLetters(foundWord);
                }
            }
            base.Update(); 
        }

        private void ChangeLetters(string word, int firstLetterIndex, int secondLetterIndex)
        {
            for (int i = 0; i < word.Length; i++)
            {
                if (i == firstLetterIndex || i == secondLetterIndex) continue;;
                Letters.Remove(word[i]);
            }
            Letters.AddRange(TheLetterManager.GetLetters(word.Length));
        }

        private bool CheckWord(string word, int firstLetterIndex, int secondLetterIndex)
        {
            List<char> availableLetters = Letters.ToList();
            if (TheLetterManager.PlacedWords.Contains(word)) return false;
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
            List<LetterBlock> blocks = new List<LetterBlock>();
            for (int i = 0; i < word.Length; i++)
            {              
                blocks.Add(TheLetterManager.InstantiateLetterButton(word[i], new Vector3(), i == firstLetterIndex, i == secondLetterIndex));
            }
            TheLetterManager.PlacedWords.Add(word);
            TheLetterManager.PlaceWordInGameBoard(blocks);
            foreach (var block in blocks)
            {
                Destroy(block.gameObject);
            }
        }

    }
}
