using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

namespace Assets.Scripts
{
    public class LetterManager : MyMonoBehaviour
    {
        float accelerometerUpdateInterval = 1.0f / 60.0f;
    // The greater the value of LowPassKernelWidthInSeconds, the slower the
    // filtered value will converge towards current input sample (and vice versa).
        float lowPassKernelWidthInSeconds = 1.0f;
    // This next parameter is initialized to 2.0 per Apple's recommendation,
    // or at least according to Brady! ;)
        float shakeDetectionThreshold = 2.0f;

        float lowPassFilterFactor;
        Vector3 lowPassValue;

        public StartingLetters StartingLettersClass;
        public PlayerLetters PlayerLettersClass;
        public LetterBlock LetterBlockObject;
        //public LetterBlock StartingLetterBlockObject;
        public RemoveWordBtn RemoveWordBtnClass;
        public PlaceWordBtn PlaceWordBtnClass;
        public TradeLettersBtn TradeLettersBtnClass;
        public TextAsset Woordenlijst;
        public TextAsset JsonAsset;
        public Material WalkingLetrMaterial;
        public Material NormalLetrMaterial;

        private Dictionary<Vector3, LetterBlock> PlacedLetterPositions { get; } = new Dictionary<Vector3, LetterBlock>();

        public Dictionary<Vector3, LetterBlock> PlayerLetterPositions { get; } = new Dictionary<Vector3, LetterBlock>();
    
        public MyPlayer Player { get; set; }

        private Vector3 lastLetterPosition = new Vector3(-2.5f, -2.5f);

        private Vector3 firstLetterPositionWordList = new Vector3(-2.75f, 4.3f);


        public Dictionary<string, object> CharactersValues { get; set; }

        private List<string> PlacedWords { get; } = new List<string>();

        private StartingLetters StartingLetters { get; set; }

        private HashSet<string> AllWords { get; set; }

        private Vector3 firstLetterPosition = new Vector3(-2.5f, -2.5f);

        private Vector3 secondLetterPosition = new Vector3(-1.7f, -2.5f);
        private float ShuffleTimeRemaining;

        private void Start()
        {
            InitCharactersValues();
            InstantiateStartingLetters();
            InstantiatePlayerLetters();
            InitAllWords();
            InitPlacedLetterPositions();
            InstantiateTradeLetterBtn();

            ShuffleTimeRemaining = 1;
            lowPassFilterFactor = accelerometerUpdateInterval / lowPassKernelWidthInSeconds;
            shakeDetectionThreshold *= shakeDetectionThreshold;
            lowPassValue = Input.acceleration;
        }

        private void Update()
        {
            Vector3 acceleration = Input.acceleration;
            lowPassValue = Vector3.Lerp(lowPassValue, acceleration, lowPassFilterFactor);
            Vector3 deltaAcceleration = acceleration - lowPassValue;
            ShuffleTimeRemaining -= Time.deltaTime;
            if (deltaAcceleration.sqrMagnitude >= shakeDetectionThreshold && ShuffleTimeRemaining <= 0)
            {
                ShufflePlayerLetters();
            }
        }

        private void ShufflePlayerLetters()
        {
            ShuffleTimeRemaining = 1;
            List<LetterBlock> letters = PlayerLetterPositions.Values.ToList().OrderBy(a => UnityEngine.Random.Range(0, 100)).ToList(); // random order
            int i = 0;
            foreach (var key in PlayerLetterPositions.Keys.ToList())
            {                  
                LetterBlock b = letters[i];
                PlayerLetterPositions[key] = b;
                if (b != null) b.transform.position = key;
                i++;
            }
        }

        private void TradeLetterBtnTouch()
        {
            foreach (var key in PlayerLetterPositions.Keys.ToList())
            {
                LetterBlock block = PlayerLetterPositions[key];
                if (block == null) continue;
                Destroy(block.gameObject);
                PlayerLetterPositions[key] = null;
            }
            foreach (var key in PlacedLetterPositions.Keys.ToList())
            {
                LetterBlock block = PlacedLetterPositions[key];
                if (block == null || block.IsFirstLetter || block.IsSecondLetter) continue;
                Destroy(block.gameObject);
                PlacedLetterPositions[key] = null;
            }
            foreach (var t in GetLetters(15))
            {
                Vector3 pos = PlayerLetterPositions.FirstOrDefault(x => x.Value == null).Key;
                LetterBlock letterBlock = InstantiateLetterButton(t, pos);
                PlayerLetterPositions[pos] = letterBlock;
            }
        }
        
        private void InstantiateTradeLetterBtn()
        {
            Spawn(TradeLettersBtnClass, this, tradebtn =>
            {
                tradebtn.LetterManager = this;
                tradebtn.OnTradeTouched += TradeLetterBtnTouch;
            });
        }

        public LetterBlock InstantiateLetterButton(char letter, Vector3 pos, bool isFirstLetter = false, bool isSecondLetter = false)
        {
            return Spawn(LetterBlockObject, this, lttrblock =>
            {
                if (isFirstLetter || isSecondLetter) lttrblock.GetComponent<MeshRenderer>().material = WalkingLetrMaterial;
                else lttrblock.GetComponent<MeshRenderer>().material = NormalLetrMaterial;
                lttrblock.IsFirstLetter = isFirstLetter;
                lttrblock.IsSecondLetter = isSecondLetter;
                lttrblock.OnLetterTouched += LetterTouched;
                lttrblock.GetComponentsInChildren<TextMesh>()[0].text =  letter.ToString().ToUpper();
                lttrblock.GetComponentsInChildren<TextMesh>()[1].text =  CharactersValues.First(x => x.Key == letter.ToString().ToLower()).Value.ToString();
                lttrblock.transform.position = pos;
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
            foreach (LetterBlock block in PlacedLetterPositions.Values.ToList())
            {
                if (block == null) continue;
                Vector3 oldPos = PlacedLetterPositions.FirstOrDefault(x => x.Value == block).Key;
                Vector3 pos;
                if (block.IsFirstLetter) pos = firstLetterPosition;
                else if (block.IsSecondLetter) pos = secondLetterPosition;
                else
                {  
                    pos = PlayerLetterPositions.FirstOrDefault(x => x.Value == null).Key;
                    PlayerLetterPositions[pos] = block;
                }
                block.transform.localScale= new Vector3(0.5f, 0.5f, 1);
                block.transform.position = pos;
                PlacedLetterPositions[oldPos] = null;
            }
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
                if (PlacedLetterPositions.Any(x => x.Value != null))
                {
                    foreach (LetterBlock block in PlacedLetterPositions.Values.ToList())
                    {
                        if (block == null) continue;
                        madeWord += block.GetComponentInChildren<TextMesh>().text;
                    }
                    if (CheckWord(madeWord.ToLower(), out long points))
                    {
                        Player.EarnedPoints += points;
                        // Timer aanzetten zodat er 10 seconden niet gedrukt kan worden
                        PlaceWordInGameBoard();
                        RemoveAllLettersFromPlayerBoard();

                        // Nieuwe letters genereren op lege plekken?
                        AddLetters(madeWord.Length - 2);
                        ChangeFixedLetters(madeWord);
                        //Player.MustThrowLetterAway = true;
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
                Vector3 pos = PlayerLetterPositions.FirstOrDefault(x => x.Value == null).Key;
                LetterBlock block = InstantiateLetterButton(letters[i], pos);
                PlayerLetterPositions[pos] = block;
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
            foreach (var letterBlock in PlacedLetterPositions.Values.ToList())
            {
                if (letterBlock == null) continue;
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

        public void LetterTouched(LetterBlock block)
        {
            Vector3 pos;
            if (PlacedLetterPositions.Values.Contains(block))
            {
                Vector3 oldPos = PlacedLetterPositions.FirstOrDefault(x => x.Value == block).Key;
                if (block.IsFirstLetter) pos = firstLetterPosition;
                else if (block.IsSecondLetter) pos = secondLetterPosition;
                else
                {
                    pos = PlayerLetterPositions.FirstOrDefault(x => x.Value == null).Key;
                    PlayerLetterPositions[pos] = block;
                }
                PlacedLetterPositions[oldPos] = null;
                block.transform.localScale = new Vector3(0.5f, 0.5f, 1);
                block.transform.position = pos;
            }
            else
            {
                if (PlacedLetterPositions.Values.Count(x => x != null) >= 12) return;
                if (!block.IsSecondLetter && !block.IsFirstLetter)
                {
                    Vector3 oldPos = PlayerLetterPositions.FirstOrDefault(x => x.Value == block).Key;
                    PlayerLetterPositions[oldPos] = null;
                }
                block.transform.localScale = new Vector3(0.4f, 0.4f, 1);
                pos = PlacedLetterPositions.FirstOrDefault(x => x.Value == null).Key;
                PlacedLetterPositions[pos] = block;
                block.transform.position = pos;
            }
        }

        private void ChangeFixedLetters(string madeWord)
        {
            StartingLetters.secondLetter = StartingLetters.firstLetter;
            var lastIndex = madeWord.Length;
            StartingLetters.firstLetter = madeWord[lastIndex - 1];
        
            Vector3 startingLetterPos = new Vector3(-2.5f, -2.5f);

            InstantiateLetterButton(StartingLetters.firstLetter, startingLetterPos, true);
            startingLetterPos.x += 0.8f;

            InstantiateLetterButton(StartingLetters.secondLetter, startingLetterPos, false, true);
        }    

        public bool Exists(string word)
        {
            return AllWords.Contains(word);
        }

        private void RemoveAllLettersFromPlayerBoard()
        {
            foreach(LetterBlock block in PlacedLetterPositions.Values.ToList())
            {
                if (block == null) continue;
                Vector3 oldPos = PlacedLetterPositions.FirstOrDefault(x => x.Value == block).Key;
                PlacedLetterPositions[oldPos] = null;
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
            foreach(LetterBlock block in PlacedLetterPositions.Values.ToList())
            {
                if (block == null) continue;
                firstLetterPositionWordList.x += 0.45f;
                InstantiateLetterButton(block.GetComponentInChildren<TextMesh>().text[0], firstLetterPositionWordList, block.IsFirstLetter, block.IsSecondLetter);
            }
        }
    }
}