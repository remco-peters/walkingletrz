using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using Random = UnityEngine.Random;

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

        #region UI
        public GameObject WritingBoard { get; set; }
        public GameObject FirstRow { get; set; }
        public GameObject SecondRow { get; set; }
        public GameObject ThirdRow { get; set; }
        public RemoveWordBtn DeleteBtn { get; set; }
        public PlaceWordBtn PlaceBtn { get; set; }
        public GameObject EmptyLetterBlockObject { get; set; }
        public LetterBlock FixedLettersBlockObject { get; set; }
        public LetterBlock PlayerLetterBlockObject { get; set; }
        public GameObject PlaceHolderObject { get; set; }
        #endregion UI
        
        #region unity properties
        public PlayerLetters PlayerLettersClass;
        public LetterBlock LetterBlockObject;
        public TheLetterManager TheLetterManager;
        public TradeLettersBtn TradeLettersBtnClass;
        public StartingLetters StartingLettersClass;
        public Material PlaceButtonInactiveMaterial;
        public Material PlaceButtonActiveMaterial;
        #endregion unity properties

        #region 
        private List<LetterPosition> PlacedLetters { get; } = new List<LetterPosition>();
        public List<LetterPosition> PlayerLetters { get; } = new List<LetterPosition>();
        private StartingLetters StartLetters { get; set; }
        public MyPlayer Player { get; set; }
        public DynamicUI DynamicUi { get; set; }
        #endregion

        #region positions
        private readonly Vector3 _firstLetterPosition = new Vector3(-2.5f, -2.5f);
        private readonly Vector3 _secondLetterPosition = new Vector3(-1.7f, -2.5f);
        #endregion positions

        private void Start()
        {
            InstantiatePlayerLetters();
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

        
        private void InstantiatePlayerLetters()
        {
            StartLetters = Spawn(StartingLettersClass, this, sl =>
            {
                sl.LetterManager = this;
            });
            
            PlayerLetters playerLetters = Spawn(PlayerLettersClass, this, pl =>
            {
                pl.LetterManager = this;
            });

            PlaceBtn.OnPlaceBtnTouched += PlaceWord;
            DeleteBtn.OnRemoveTouched += RemoveAllLetters;
        }
        
        private void InitPlacedLetterPositions()
        {
            for (int i = 0; i < 12; i++)
            {
                GameObject elbi = Instantiate(EmptyLetterBlockObject);
                elbi.transform.SetParent(WritingBoard.transform);
                PlacedLetters.Add(new LetterPosition(0, i, null));
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

        private void ShufflePlayerLetters()
        {
            Vector3 acceleration = Input.acceleration;
            _lowPassValue = Vector3.Lerp(_lowPassValue, acceleration, _lowPassFilterFactor);
            Vector3 deltaAcceleration = acceleration - _lowPassValue;
            _shuffleTimeRemaining -= Time.deltaTime;
            if (deltaAcceleration.sqrMagnitude >= _shakeDetectionThreshold && _shuffleTimeRemaining <= 0)
            {
                _shuffleTimeRemaining = 1;
                List<LetterBlock> letters = PlayerLetters.Select(x => x.LetterBlock).OrderBy(a => Random.Range(0, 100)).ToList(); // random order
                for (int i = 0; i < PlayerLetters.Count; i++)
                {
                    // Todo
                    //PlayerLetters[i].AddLetter(letters[i]);
                }
            }
        }

        private void TradeLetterBtnTouch()
        {
            void ClearDictionary(IEnumerable<LetterPosition> letterPositions)
            {
                foreach (var letterPosition in letterPositions)
                {
                    //Todo
                    //if (!letterPosition.ContainsLetter() || letterPosition.LetterBlock.IsWalkingLetter()) continue;
                    Destroy(letterPosition.LetterBlock.gameObject);
                    letterPosition.RemoveLetter();
                }
            }
            ClearDictionary(PlayerLetters);
            ClearDictionary(PlacedLetters);
            foreach (var t in GetLetters(15))
            {
                LetterPosition pos = PlayerLetters.FirstOrDefault(x => x.LetterBlock == null);
                //Todo
                //LetterBlock letterBlock = InstantiateLetterButton(t, pos.Position);
                //pos.AddLetter(letterBlock);
            }
        }

        public LetterBlock InstantiateLetterButton(char letter, Vector3 pos, bool isFirstLetter = false, bool isSecondLetter = false, int row = 1)
        {
            LetterBlock block;
            if(isFirstLetter || isSecondLetter)
            {
                block = FixedLettersBlockObject;
            } else
            {
                block = PlayerLetterBlockObject;
            }

            return Spawn(block, this, lttrBlock =>
            {
                lttrBlock.IsFirstLetter = isFirstLetter;
                lttrBlock.IsSecondLetter = isSecondLetter;
                lttrBlock.OnLetterTouched += LetterTouched;
                //Todo
                //lttrBlock.OnLetterDragged += LetterDragged;
                lttrBlock.GetComponentInChildren<Text>().text = letter.ToString().ToUpper();
                // Todo
                //lttrBlock.GetComponentsInChildren<TextMesh>()[1].text = TheLetterManager.CharactersValues.First(x => x.Key == char.ToLower(letter)).Value.ToString();
                GameObject parentRow = GetRightRow(row);
                lttrBlock.transform.SetParent(parentRow.transform);
                PlayerLetters.Add(new LetterPosition(row, lttrBlock.transform.GetSiblingIndex(), lttrBlock));
            });
        }
        
        private void RemoveAllLetters()
        {
            foreach(LetterPosition lttrPos in PlacedLetters)
            {
                if (lttrPos.LetterBlock != null)
                {
                    RemoveLetterFromWritingBoardToPlayerBoard(lttrPos.LetterBlock);
                }
            }
            //Todo
            //PlaceBtn.GetComponent<MeshRenderer>().material = PlaceButtonInactiveMaterial;
        }
        
        private void PlaceWord()
        {
            // Alleen wanneer mag versturen
            if (Player.CanMove)
            {
                if (PlacedLetters.Any(x => x.LetterBlock != null))
                {
                    string madeWord = "";
                    foreach (LetterBlock block in PlacedLetters.Select(x => x.LetterBlock).ToList())
                    {
                        if (block == null) continue;
                        //Todo
                        //madeWord += block.GetLetter();
                    }
                    if (!TheLetterManager.CheckWord(madeWord, out long points, PlacedLetters)) return;
                    Player.EarnedPoints += points;
                    PlaceWordInGameBoard();
                    RemoveAllLettersFromPlayerBoard();
                    AddLetters(madeWord.Length - 2);
                    ChangeFixedLetters(madeWord);
                    PlaceBtn.GetComponent<MeshRenderer>().material = PlaceButtonInactiveMaterial;
                    DynamicUi.PlayerManagerClass.NextTurn(Player);
                }
                else
                {
                    Player.InfoText = "No letters placed yet";
                    Debug.Log("No letters placed yet");
                }
            }
            else
            {
                if (Player.TimeRemaining <= 0) Debug.Log("Time's over. Play again!");
            }
        }

        private void AddLetters(int amount)
        {
            char[] letters = GetLetters(amount);
            for (int i = 0; i < amount; i++)
            {
                LetterPosition pos = PlayerLetters.FirstOrDefault(x => x.LetterBlock == null);
                //Todo remove the fking positions
                //LetterBlock block = InstantiateLetterButton(letters[i], pos.Position);
                // Todo
                //pos.AddLetter(block);
            }
        }

        public long CalculatePoints(string word)
        {
            long value = 0;
            foreach (var letter in word)
            {
                value += TheLetterManager.CharactersValues.FirstOrDefault(x => x.Key == letter).Value;
            }
            if (word.Length >= 5 && word.Length <= 7) value = (long)(value * 1.5);
            else if (word.Length >= 8 && word.Length <= 10) value *= 2;
            else if (word.Length == 11) value = (long)(value * 2.5);
            else if (word.Length == 12) value *= 3;
            return value;
        }
        
        public bool Exists(string word)
        {
            return TheLetterManager.AllWords.Contains(word);
        }

        public char[] GetLetters(int amount)
        {
            char[] startingLetters = new char[amount];
            List<char> availableLetters = new List<char>
            { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm',
              'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };

            List<char> lettersToChoseFrom = new List<char>();

            foreach (char c in availableLetters)
            {
                // Todo: More than 2 of the same? Then the next!
                bool isVowel = "aeiou".IndexOf(c) >= 0;
                long val = TheLetterManager.CharactersValues[c];
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
            // Wanneer hij in de lijst placedLetters staat, moet deze terug naar beneden gezet worden, anders naar boven
            if (PlacedLetters.Select(x => x.LetterBlock).Contains(block))
            {
                RemoveLetterFromWritingBoardToPlayerBoard(block);
            }
            else
            {
                // Wanneer er meer dan 12 zijn, niks doen
                if (PlacedLetters.Count(x => x.LetterBlock != null) >= 12) return;
                
                // Find the block in de playerletters
                LetterPosition letterBlock = PlayerLetters.FirstOrDefault(x => x.LetterBlock == block);
                // Find the first empty block in PlacedLetters
                LetterPosition EmptyLetterBlock = PlacedLetters.FirstOrDefault(x => x.LetterBlock == null);
                
                // De geklikte letter toevoegen aan EmptyLetterBlock en values overnemen
                EmptyLetterBlock.AddLetter(block, letterBlock.GetCurrentIndex(), letterBlock.GetRow());
                
                // Het lege object vinden om te kunnen destroyen
                Transform t = WritingBoard.transform.GetChild(EmptyLetterBlock.GetCurrentIndex());
                Destroy(t.gameObject);

                // Het geklikte blokje verplaatsen naar de plaats van het lege object
                block.transform.SetParent(WritingBoard.transform);
                block.transform.SetSiblingIndex(EmptyLetterBlock.GetCurrentIndex());
                
                // Een lege placeholder plaatsen waar de letter vandaan is gehaald
                GameObject ph = Instantiate(PlaceHolderObject);
                GameObject parentRow = GetRightRow(letterBlock.GetRow());
                ph.transform.SetParent(parentRow.transform);
                ph.transform.SetSiblingIndex(letterBlock.GetCurrentIndex());
            }
            //CheckWordAndSetSubmitButtonState();
            
        }

        private void ChangeFixedLetters(string madeWord)
        {
            StartLetters.secondLetter = StartLetters.firstLetter;
            var lastIndex = madeWord.Length;
            StartLetters.firstLetter = madeWord[lastIndex - 1];
            // Todo: Remove the fking position
            Vector3 startingLetterPos = new Vector3(-2.5f, -2.5f);

            InstantiateLetterButton(StartLetters.firstLetter, startingLetterPos, true);
            startingLetterPos.x += 0.8f;

            InstantiateLetterButton(StartLetters.secondLetter, startingLetterPos, false, true);
        }    

        private void RemoveAllLettersFromPlayerBoard()
        {
            foreach (LetterPosition position in PlacedLetters)
            {
                if (position.LetterBlock == null) continue;
                Destroy(position.LetterBlock.gameObject);
                position.RemoveLetter();
            }
        }

        private void PlaceWordInGameBoard()
        {
            Vector3 firstLetterPositionWordList = new Vector3(-2.75f, 4.3f);
            firstLetterPositionWordList.y -= 0.45f * TheLetterManager.PlacedWords.Count;
            foreach (LetterBlock block in PlacedLetters.Select(x => x.LetterBlock).ToList())
            {
                if (block == null) continue;
                firstLetterPositionWordList.x += 0.45f;
                //Todo
                //InstantiateLetterButton(block.GetLetter(), firstLetterPositionWordList, block.IsFirstLetter, block.IsSecondLetter);
            }
        }

        private void LetterDragged(LetterBlock draggedLetter)
        {
            //Todo
            //draggedLetter.LetterDragged(PlacedLetters, PlayerLetters);
            CheckWordAndSetSubmitButtonState();
        }

        private void CheckWordAndSetSubmitButtonState()
        {
            string madeWord = "";
            if (PlacedLetters.All(x => x.LetterBlock == null)) return;
            bool containsFirstLetter = false;
            bool containsSecondLetter = false;
            foreach (LetterBlock block in PlacedLetters.Select(x => x.LetterBlock))
            {
                if (block == null) continue;
                //Todo
                //madeWord += block.GetLetter();
                if (block.IsFirstLetter) containsFirstLetter = true;
                if (block.IsSecondLetter) containsSecondLetter = true;
            }
            if (Exists(madeWord.ToLower()) && containsFirstLetter && containsSecondLetter)
            {
                PlaceBtn.GetComponent<MeshRenderer>().material = PlaceButtonActiveMaterial;
                //TODO: Enable interaction when these are buttons (button.interactable = true)
            }
            else
            {
                PlaceBtn.GetComponent<MeshRenderer>().material = PlaceButtonInactiveMaterial;
                //TODO: Disable interaction when these are buttons (button.interactable = false)
            }

        }

        private void RemoveLetterFromWritingBoardToPlayerBoard(LetterBlock block)
        {
            // Het weghalen van de letter
            LetterPosition letterClicked = PlacedLetters.FirstOrDefault(x => x.LetterBlock == block);
            int row = letterClicked.GetRow();
            int oldIndex = letterClicked.GetOldIndex();
            int currentIndex = letterClicked.GetCurrentIndex();
            letterClicked.RemoveLetter();
            
            // Placeholder verwijderen
            GameObject parentRow = GetRightRow(row);
            Transform placeHolder = parentRow.transform.GetChild(oldIndex);
            DestroyImmediate(placeHolder.gameObject);

            // Nieuw, leeg block toevoegen in writingBoard
            GameObject emptyBlock = Instantiate(EmptyLetterBlockObject);
            emptyBlock.transform.SetParent(WritingBoard.transform);
            emptyBlock.transform.SetSiblingIndex(currentIndex);

            block.transform.SetParent(parentRow.transform);
            block.transform.SetSiblingIndex(oldIndex);
        }

        private GameObject GetRightRow(int row)
        {
            switch(row)
            {
                case 1:
                    return FirstRow;
                case 2:
                    return SecondRow;
                default:
                    return ThirdRow;
            }
        }
    }
}