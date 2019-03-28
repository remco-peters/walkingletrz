using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.PackageManager;
using UnityEngine;

namespace Assets.Scripts
{
    public class LetterManager : MyMonoBehaviour
    {
        public StartingLetters StartingLettersClass;
        public PlayerLetters PlayerLettersClass;
        public LetterBlock LetterBlockObject;
        public LetterBlock StartingLetterBlockObject;
        public RemoveWordBtn RemoveWordBtnClass;
        public PlaceWordBtn PlaceWordBtnClass;
        public TradeLettersBtn TradeLettersBtnClass;
        public TextAsset Woordenlijst;
        public TextAsset JsonAsset;

        private Dictionary<Vector3, LetterBlock> PlacedLetterPositions { get; } = new Dictionary<Vector3, LetterBlock>();
        private Dictionary<Vector3, LetterBlock> PlayerLetterPositions {get; } = new Dictionary<Vector3, LetterBlock>();
    
        public GameObject LetterBoard { get; set; }
        public MyPlayer Player { get; set; }

        private Vector3 lastLetterPosition = new Vector3(-2.5f, -2.5f);

        private Vector3 firstLetterPositionWordList = new Vector3(-2.75f, 4.3f);

        public List<LetterBlock> PlacedLetters { get; set; } = new List<LetterBlock>();

        public Dictionary<string, object> CharactersValues { get; set; }

        private List<string> PlacedWords { get; } = new List<string>();

        private StartingLetters StartingLetters { get; set; }

        private HashSet<string> AllWords { get; set; }

        private void Start()
        {
            InitCharactersValues();
            InstantiateStartingLetters();
            InstantiatePlayerLetters();
            InitAllWords();
            InitPlacedLetterPositions();
            InstantiatieTradeLetterBtn();;
        }

        private void InstantiatieTradeLetterBtn()
        {
            Spawn(TradeLettersBtnClass, this, tradebtn =>
            {
                tradebtn.LetterManager = this;
                tradebtn.OnTradeTouched += () =>
                {
                    if (!Player.CanMove)
                    {
                        Debug.Log("Cant trade yet: " + Player.CoolDownTime + " seconds remaining");
                        return;
                    }
                    Player.CanMove = false;
                    Player.StartCooldown();
                    LetterBlock test;
                    while ((test = PlayerLetterPositions.FirstOrDefault(x => x.Value != null).Value) != null)
                    {
                        Vector3 pos = test.transform.position;
                        Destroy(test.gameObject);
                        PlayerLetterPositions[pos] = null;
                    }
                    while ((test = PlacedLetterPositions.FirstOrDefault(x => x.Value != null && !x.Value.IsFirstLetter && !x.Value.IsSecondLetter).Value) != null)
                    {              
                        Vector3 pos = test.transform.position;
                        Destroy(test.gameObject);
                        PlacedLetterPositions[pos] = null;
                        PlacedLetters.Remove(test);
                    }
                    foreach (var t in GetLetters(15))
                    {
                        Vector3 pos = PlayerLetterPositions.FirstOrDefault(x => x.Value == null).Key;
                        LetterBlock letterBlock = Instantiate(LetterBlockObject);
                        letterBlock.transform.position = pos;
                        letterBlock.OnLetterTouched += LetterTouched;
                        letterBlock.GetComponentInChildren<TextMesh>().text = t.ToString().ToUpper();
                        PlayerLetterPositions[pos] = letterBlock;
                    }
                };
            });
        }

        private void InstantiatePlayerLetters()
        {
            PlayerLetters playerLetters = Spawn(PlayerLettersClass, this, pl =>
            {
                pl.letterManager = this;
                pl.lastLetterPosition = lastLetterPosition;
            });
            
            RemoveWordBtn removeWordBtn = Instantiate(RemoveWordBtnClass);
            removeWordBtn.OnRemoveTouched += RemoveAllLetters;

            PlaceWordBtn placeWordBtn = Instantiate(PlaceWordBtnClass);
            placeWordBtn.OnPlaceBtnTouched += PlaceWord;
        }

        private void RemoveAllLetters()
        {
            foreach (LetterBlock block in PlacedLetters)
            {
                PlacedLetterPositions[block.transform.position] = null;
                Vector3 pos;
                if (block.IsFirstLetter)
                {
                    pos = new Vector3(-2.5f, -2.5f);
                }
                else if (block.IsSecondLetter)
                {
                    pos = new Vector3(-1.7f, -2.5f);
                }
                else
                {  
                    pos = PlayerLetterPositions.FirstOrDefault(x => x.Value == null).Key;
                    PlayerLetterPositions[pos] = block;
                }
                block.transform.localScale= new Vector3(0.5f, 0.5f, 1);
                block.transform.position = pos;
            
            }
            PlacedLetters.RemoveAll(x => true);
        }

        private void InitPlacedLetterPositions()
        {
            for (int i = 0; i < 12; i++)
            {
                PlacedLetterPositions.Add(new Vector3(-2.5f + 0.45f * i, -1.7f), null);
            }
        }

        private void PlaceWord()
        {
            string madeWord = "";
            // Alleen wanneer mag versturen
            if (Player.CanMove)
            {
                if (PlacedLetters.Count > 0)
                {
                    foreach (LetterBlock block in PlacedLetters)
                    {
                        madeWord += block.GetComponentInChildren<TextMesh>().text;
                    }
                    if (CheckWord(madeWord.ToLower(), out long points))
                    {
                        Player.EarnedPoints += points;
                        // Timer aanzetten zodat er 10 seconden niet gedrukt kan worden
                        Player.CanMove = false;
                        Player.StartCooldown();
                        RemoveAllLettersFromPlayerBoard();
                        PlacedLetters.RemoveAll(x => true);
                        PlaceWordInGameBoard();

                        // Nieuwe letters genereren op lege plekken?
                        AddLetters(madeWord.Length - 2);
                        ChangeFixedLetters(madeWord);
                        Player.MustThrowLetterAway = true;
                    }
                } else
                {
                    Player.InfoText = "No letters placed yet";
                    Debug.Log("No letters placed yet");
                }
            }
            else
            {
                if (Player.TimeRemaining <= 0)
                {
                    Debug.Log("Time's over. Play again!");
                }
                else
                {
                    Debug.Log("Cant move yet: " + Player.CoolDownTime + " seconds remaining");
                }
            }
        }

        private void AddLetters(int amount)
        {
            char[] letters = GetLetters(amount);
            for (int i = 0; i < amount; i++)
            {
                LetterBlock block = Instantiate(LetterBlockObject);
                Vector3 pos = PlayerLetterPositions.FirstOrDefault(x => x.Value == null).Key;
                PlayerLetterPositions[pos] = block;
                block.transform.position = pos;
                block.GetComponentInChildren<TextMesh>().text = letters[i].ToString().ToUpper();
                block.OnLetterTouched += LetterTouched;
            }
        }

        private void InstantiateStartingLetters()
        {
            StartingLetters =  Spawn(StartingLettersClass, this, sl =>
            {
                sl.LetterManager = this;
                sl.lastLetterPosition = lastLetterPosition;
            });

            lastLetterPosition = StartingLetters.GetLastLetterPosition();
        }

        private void InitAllWords()
        {
            AllWords = new HashSet<string>(Woordenlijst.text.Split(new[] { Environment.NewLine },StringSplitOptions.None));
            Woordenlijst = null;
        }

        public long CalculatePoints(string word)
        {
            long value = 0;
            foreach (var letter in word)
            {
                value += (long) CharactersValues.First(x => x.Key == letter.ToString()).Value;
            }

            return value;
        }

        public void InitCharactersValues()
        {
            string json = JsonAsset.text;
            var items = (Dictionary<string, object>) MiniJSON.Json.Deserialize(json);
            foreach (var item in items)
            {
                if (item.Key != "lettervalues") continue;
                CharactersValues = item.Value as Dictionary<string, object>;
            }
        }

        public bool CheckWord(string word, out long points)
        {
            int containsFirstLetter = -1;
            int containsSecondLetter = -1;
            points = CalculatePoints(word);
            int i = 0;
            foreach (var letterBlock in PlacedLetters)
            {
                if (letterBlock.IsFirstLetter)
                    containsFirstLetter = i;

                if (letterBlock.IsSecondLetter)
                    containsSecondLetter = i;
                i++;
            }
            if (containsFirstLetter < 0 || containsSecondLetter < 0)
            {
                Debug.Log("Word does not contain the two letters");
                Player.InfoText = "Word does not contain the two letters";
                return false;
            }
            if(containsFirstLetter > containsSecondLetter)
            {
                Debug.Log("First letter is after second letter");
                Player.InfoText = "First letter is after second letter";
                return false;
            }
            if (!Exists(word))
            {
                Debug.Log($"Word does not exist. Word: {word}");
                Player.InfoText = $"Word does not exist. Word: {word}";

                return false;
            }
            if (PlacedWords.Contains(word))
            {
                Debug.Log("Word already placed");
                Player.InfoText = "Word already placed";
                return false;
            }
            Player.InfoText = "";
            PlacedWords.Add(word);
            return true;

        }

        public char[] GetLetters(int amount)
        {
            char[] startingLetters = new char[15];
            List<char> availableLetters =new List<char> { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l',
                'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };

            List<char> lettersToChoseFrom = new List<char>();

            foreach (char c in availableLetters)
            {
                bool isVowel = "aeiou".IndexOf(c) >= 0;
                long val = (long) CharactersValues[c.ToString()];
                if (isVowel) val -= 10;
                for (int i = 0; i < 8 - val; i++)
                {
                    lettersToChoseFrom.Add(c);
                }
            }
            for (int i = 0; i < amount; i++)
            {
                startingLetters[i] = lettersToChoseFrom[UnityEngine.Random.Range(0, lettersToChoseFrom.Count)];
            }
            return startingLetters;
        }
        private void StartLetterTouched(LetterBlock block, Vector3 startPos)
        {
            if (PlacedLetters.Contains(block))
            {
                PlacedLetters.Remove(block);
                PlacedLetterPositions[block.transform.position] = null;
                block.transform.localScale = new Vector3(0.5f, 0.5f, 1);
                block.transform.position = startPos;
            }
            else if (PlacedLetters.Count < 12) // Anders niks doen; Maximaal 12 letterige woorden
            {
                Vector3 pos = PlacedLetterPositions.FirstOrDefault(x => x.Value == null).Key;
                PlacedLetterPositions[pos] = block;
                block.transform.localScale = new Vector3(0.4f, 0.4f, 1);
                block.transform.position = pos;
                PlacedLetters.Add(block);
            }
        }

        public void LetterTouched(LetterBlock block)
        {
            if (Player.MustThrowLetterAway)
            {
                if (block.IsFirstLetter || block.IsSecondLetter) return;
                PlayerLetterPositions[block.transform.position] = null;
                Destroy(block.gameObject);
                Player.MustThrowLetterAway = false;
                AddLetters(1);
                return;
            }
            if (block.IsFirstLetter)
            {
                StartLetterTouched(block, new Vector3(-2.5f, -2.5f));
                return;
            }

            if (block.IsSecondLetter)
            {
                StartLetterTouched(block, new Vector3(-1.7f, -2.5f));
                return;
            }
            if (PlacedLetters.Contains(block))
            {
                PlacedLetters.Remove(block);
                PlacedLetterPositions[block.transform.position] = null;
                block.transform.localScale = new Vector3(0.5f, 0.5f, 1);
                Vector3 pos = PlayerLetterPositions.FirstOrDefault(x => x.Value == null).Key;
                PlayerLetterPositions[pos] = block;
                block.transform.position = pos;
            }

            else if (PlacedLetters.Count < 12) // Anders niks doen; Maximaal 12 letterige woorden
            {
                PlayerLetterPositions[block.transform.position] = null;
                block.transform.localScale = new Vector3(0.4f, 0.4f, 1);
                Vector3 pos = PlacedLetterPositions.FirstOrDefault(x => x.Value == null).Key;
                PlacedLetterPositions[pos] = block;
                var index = 0;
                foreach (var v in PlacedLetterPositions.Keys)
                {
                    if (v == pos)
                    {
                        break;
                    }

                    index++;
                }
                block.transform.position = pos;
                PlacedLetters.Insert(index, block);
            }
        }

        private void ChangeFixedLetters(string madeWord)
        {
            StartingLetters.secondLetter = StartingLetters.firstLetter;
            var lastIndex = madeWord.Length;
            StartingLetters.firstLetter = madeWord[lastIndex - 1].ToString().ToLower();
        
            Vector3 startingLetterPos = new Vector3(-2.5f, -2.5f);

            LetterBlock startingLetterBlock = Instantiate(StartingLetterBlockObject, startingLetterPos, new Quaternion());
            startingLetterBlock.GetComponentInChildren<TextMesh>().text = StartingLetters.firstLetter.ToUpper();
            startingLetterBlock.OnLetterTouched += LetterTouched;
            startingLetterPos.x += 0.8f;
            startingLetterBlock.IsFirstLetter = true;

            startingLetterBlock = Instantiate(StartingLetterBlockObject, startingLetterPos, new Quaternion());
            startingLetterBlock.GetComponentInChildren<TextMesh>().text = StartingLetters.secondLetter.ToUpper();
            startingLetterBlock.OnLetterTouched += LetterTouched;
            startingLetterBlock.IsSecondLetter = true;
        }    

        public bool Exists(string word)
        {
            return AllWords.Contains(word);
        }

        private void RemoveAllLettersFromPlayerBoard()
        {
            foreach(LetterBlock block in PlacedLetters)
            {
                Destroy(block.gameObject);
            }
        }

        private void PlaceWordInGameBoard()
        {
            string lastWord = PlacedWords[PlacedWords.Count - 1];
            if(PlacedWords.Count > 1)
            {
                firstLetterPositionWordList.y -= 0.45f;
                firstLetterPositionWordList.x = -2.75f;
            
            }
            foreach(char letter in lastWord)
            {
                firstLetterPositionWordList.x += 0.45f;
            
                LetterBlock letterBlock = Instantiate(LetterBlockObject, firstLetterPositionWordList, new Quaternion());
                letterBlock.IsLetterSet = false;
                letterBlock.transform.localScale = new Vector3(0.4f, 0.4f, 1);
                letterBlock.GetComponentInChildren<TextMesh>().text = letter.ToString().ToUpper();
            }
        }
    }
}