using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
    public class LetterManager : MyMonoBehaviour
    {
        #region shuffle
        private const float AccelerometerUpdateInterval = 1.0f / 60.0f;
        private const float LowPassKernelWidthInSeconds = 1.0f;
        private float _shakeDetectionThreshold = 2.0f;
        private float _lowPassFilterFactor;
        private Vector3 _lowPassValue;      
        private float _shuffleTimeRemaining;
        #endregion shuffle

        #region unity properties
        public StartingLetters StartingLettersClass;
        public PlayerLetters PlayerLettersClass;
        public LetterBlock LetterBlockObject;
        public RemoveWordBtn RemoveWordBtnClass;
        public PlaceWordBtn PlaceWordBtnClass;
        public TradeLettersBtn TradeLettersBtnClass;
        public TextAsset Woordenlijst;
        public TextAsset JsonAsset;
        public Material WalkingLetrMaterial;
        public Material NormalLetrMaterial;
        public Material PlaceButtonInactiveMaterial;
        public Material PlaceButtonActiveMaterial;
        #endregion 

        private PlaceWordBtn _placeWordBtn;
        private List<LetterPosition> PlacedLetters { get; } = new List<LetterPosition>();
        public List<LetterPosition> PlayerLetters { get; } = new List<LetterPosition>();
        public Dictionary<string, object> CharactersValues { get; set; }
        private List<string> PlacedWords { get; } = new List<string>();
        private StartingLetters StartingLetters { get; set; }
        private HashSet<string> AllWords { get; set; }
        public MyPlayer Player { get; set; }

        #region positions
        private readonly Vector3 _firstLetterPosition = new Vector3(-2.5f, -2.5f);
        private readonly Vector3 _secondLetterPosition = new Vector3(-1.7f, -2.5f);
        #endregion positions

        private void Start()
        {
            InitCharactersValues();
            InstantiateStartingLetters();
            InstantiatePlayerLetters();
            InitAllWords();
            InitPlacedLetterPositions();
            InstantiateTradeLetterBtn();

            _shuffleTimeRemaining = 1;
            _lowPassFilterFactor = AccelerometerUpdateInterval / LowPassKernelWidthInSeconds;
            _shakeDetectionThreshold *= _shakeDetectionThreshold;
            _lowPassValue = Input.acceleration;
        }
    
        private void Update()
        {
            ShufflePlayerLetters();
        }

        private void ShufflePlayerLetters()
        {
            Vector3 acceleration = Input.acceleration;
            _lowPassValue = Vector3.Lerp(_lowPassValue, acceleration, _lowPassFilterFactor);
            Vector3 deltaAcceleration = acceleration - _lowPassValue;
            _shuffleTimeRemaining -= Time.deltaTime;
            if (deltaAcceleration.sqrMagnitude >= _shakeDetectionThreshold && _shuffleTimeRemaining <= 0)
            {
                _shuffleTimeRemaining = 1;
                List<LetterBlock> letters = PlayerLetters.Select(x => x.LetterBlock).OrderBy(a => UnityEngine.Random.Range(0, 100)).ToList(); // random order
                for (int i = 0; i < PlayerLetters.Count; i++)
                {                  
                    PlayerLetters[i].AddLetter(letters[i]);
                }
            }
        }

        private void TradeLetterBtnTouch()
        {
            void ClearDictionary(IEnumerable<LetterPosition> letterPositions)
            {
                foreach (var letterPosition in letterPositions)
                {
                    if (!letterPosition.ContainsLetter() || letterPosition.LetterBlock.IsWalkingLetter()) continue;
                    Destroy(letterPosition.LetterBlock.gameObject);
                    letterPosition.RemoveLetter();
                }
            }
            ClearDictionary(PlayerLetters);
            ClearDictionary(PlacedLetters);
            foreach (var t in GetLetters(15))
            {
                LetterPosition pos = PlayerLetters.FirstOrDefault(x => x.LetterBlock == null);
                LetterBlock letterBlock = InstantiateLetterButton(t, pos.Position);
                pos.AddLetter(letterBlock);
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
                lttrblock.OnLetterDragged += LetterDragged;
                lttrblock.GetComponentsInChildren<TextMesh>()[0].text = letter.ToString().ToUpper();
                lttrblock.GetComponentsInChildren<TextMesh>()[1].text = CharactersValues.First(x => x.Key == letter.ToString().ToLower()).Value.ToString();
                lttrblock.transform.position = pos;
            });
        }

        private void InstantiatePlayerLetters()
        {
            PlayerLetters playerLetters = Spawn(PlayerLettersClass, this, pl =>
            {
                pl.letterManager = this;
                pl.lastLetterPosition = StartingLetters.GetLastLetterPosition();;
            });
            
            RemoveWordBtn removeWordBtn = Instantiate(RemoveWordBtnClass);
            removeWordBtn.OnRemoveTouched += RemoveAllLetters;

            _placeWordBtn = Instantiate(PlaceWordBtnClass);
            _placeWordBtn.OnPlaceBtnTouched += PlaceWord;
        }

        private void RemoveAllLetters()
        {
            foreach (LetterPosition position in PlacedLetters)
            {
                if (!position.ContainsLetter()) continue;
                if (position.LetterBlock.IsFirstLetter)
                {
                    position.LetterBlock.transform.position = _firstLetterPosition;
                }
                else if (position.LetterBlock.IsSecondLetter)
                {
                    position.LetterBlock.transform.position = _secondLetterPosition;
                }
                else
                {
                    PlayerLetters.FirstOrDefault(x => x.LetterBlock == null)?.AddLetter(position.LetterBlock);
                }
                position.LetterBlock.transform.localScale= new Vector3(0.5f, 0.5f, 1);
                position.RemoveLetter();
            }

            _placeWordBtn.GetComponent<MeshRenderer>().material = PlaceButtonInactiveMaterial;
        }

        private void InitPlacedLetterPositions()
        {
            for (int i = 0; i < 12; i++)
            {
                PlacedLetters.Add(new LetterPosition(new Vector3(-2.5f + 0.45f * i, -1.7f), null));
            }
        }

        private void PlaceWord()
        {
            string madeWord = "";
            // Alleen wanneer mag versturen
            if (Player.CanMove)
            {
                if (PlacedLetters.Any(x => x.LetterBlock != null))
                {
                    foreach (LetterBlock block in PlacedLetters.Select(x => x.LetterBlock).ToList())
                    {
                        if (block == null) continue;
                        madeWord += block.GetLetter();
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
                        _placeWordBtn.GetComponent<MeshRenderer>().material = PlaceButtonInactiveMaterial;
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
                LetterPosition pos = PlayerLetters.FirstOrDefault(x => x.LetterBlock == null);
                LetterBlock block = InstantiateLetterButton(letters[i], pos.Position);
                pos.AddLetter(block);
            }
        }

        private void InstantiateStartingLetters()
        {
            StartingLetters =  Spawn(StartingLettersClass, this, sl =>
            {
                sl.LetterManager = this;
            });
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
            if (word.Length >= 5 && word.Length <= 7) value = (long)(value * 1.5);
            else if (word.Length >= 8 && word.Length <= 10) value *= 2;
            else if (word.Length == 11) value = (long)(value * 2.5);
            else if (word.Length == 12) value *= 3;
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
            foreach (var letterBlock in PlacedLetters.Select(x => x.LetterBlock).ToList())
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
            List<char> availableLetters =new List<char>
            { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 
              'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };

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
            if (PlacedLetters.Select(x => x.LetterBlock).Contains(block))
            {
                LetterPosition letterPosition = PlacedLetters.FirstOrDefault(x => x.LetterBlock == block);
                if (block.IsFirstLetter)
                {
                    block.transform.position = _firstLetterPosition;
                }
                else if (block.IsSecondLetter)
                {
                    block.transform.position = _secondLetterPosition;
                }
                else
                {
                    PlayerLetters.FirstOrDefault(x => x.LetterBlock == null)?.AddLetter(block);
                }
                letterPosition?.RemoveLetter();
                block.transform.localScale = new Vector3(0.5f, 0.5f, 1);
            }
            else
            {
                if (PlacedLetters.Count(x => x.LetterBlock != null) >= 12) return;
                if (!block.IsSecondLetter && !block.IsFirstLetter)
                {
                    PlayerLetters.FirstOrDefault(x => x.LetterBlock == block)?.RemoveLetter();
                }
                block.transform.localScale = new Vector3(0.4f, 0.4f, 1);
                LetterPosition letterPosition = PlacedLetters.FirstOrDefault(x => x.LetterBlock == null);
                letterPosition?.AddLetter(block);
            }
            CheckWordAndSetSubmitButtonState();
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
            foreach(LetterPosition position in PlacedLetters)
            {
                if (position.LetterBlock == null) continue;
                Destroy(position.LetterBlock.gameObject);
                position.RemoveLetter();
            }
        }

        private void PlaceWordInGameBoard()
        {
            Vector3 firstLetterPositionWordList = new Vector3(-2.75f, 4.3f);
            firstLetterPositionWordList.y -= 0.45f * PlacedWords.Count;
            foreach(LetterBlock block in PlacedLetters.Select(x => x.LetterBlock).ToList())
            {
                if (block == null) continue;
                firstLetterPositionWordList.x += 0.45f;
                InstantiateLetterButton(block.GetLetter(), firstLetterPositionWordList, block.IsFirstLetter, block.IsSecondLetter);
            }
        }
        
        private void LetterDragged(LetterBlock draggedLetter)
        {
            draggedLetter.LetterDragged(PlacedLetters, PlayerLetters);
            CheckWordAndSetSubmitButtonState();
        }

        private void CheckWordAndSetSubmitButtonState()
        {
            string madeWord = "";
            if (PlacedLetters.Any(x => x.LetterBlock != null))
            {
                bool containsFirstLetter = false;
                bool containsSecondLetter = false;
                foreach (LetterBlock block in PlacedLetters.Select(x => x.LetterBlock))
                {
                    if (block == null) continue;
                    madeWord += block.GetLetter();
                    if (block.IsFirstLetter)
                    {
                        containsFirstLetter = true;
                    }
                    if (block.IsSecondLetter)
                    {
                        containsSecondLetter = true;
                    }
                }

                if (Exists(madeWord.ToLower()) && containsFirstLetter && containsSecondLetter)
                {
                    _placeWordBtn.GetComponent<MeshRenderer>().material = PlaceButtonActiveMaterial;
                    //TODO: Enable interaction when these are buttons (button.interactable = true)
                }
                else
                {
                    _placeWordBtn.GetComponent<MeshRenderer>().material = PlaceButtonInactiveMaterial;
                    //TODO: Disable interaction when these are buttons (button.interactable = false)
                }
            }
        }
    }
}