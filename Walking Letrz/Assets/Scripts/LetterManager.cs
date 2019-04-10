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
        public TheLetterManager TheLetterManager;
        public LetterBlock LetterBlockObject;
        public RemoveWordBtn RemoveWordBtnClass;
        public PlaceWordBtn PlaceWordBtnClass;
        public TradeLettersBtn TradeLettersBtnClass;
        public Material WalkingLetrMaterial;
        public Material NormalLetrMaterial;
        public Material PlaceButtonInactiveMaterial;
        public Material PlaceButtonActiveMaterial;
        #endregion unity properties

        #region 
        private PlaceWordBtn _placeWordBtn;
        public List<LetterPosition> PlacedLetters { get; } = new List<LetterPosition>();
        public List<LetterPosition> PlayerLetters { get; } = new List<LetterPosition>();
        public MyPlayer Player { get; set; }
        public GameState GameState { get; set; }
        public LetterBlock FirstLetterBlock { get; set; }
        public LetterBlock SecondLetterBlock{get; set; }
        #endregion

        #region positions
        public readonly Vector3 _firstLetterPosition = new Vector3(-2.5f, -2.5f);
        public readonly Vector3 _secondLetterPosition = new Vector3(-1.7f, -2.5f);
        #endregion positions

        private void Start()
        {
            InitStartingLetters();
            InitFirstLetters();
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
        
        private void InitStartingLetters()
        {
            FirstLetterBlock = InstantiateLetterButton(TheLetterManager.FirstLetter, _firstLetterPosition, true);
            SecondLetterBlock = InstantiateLetterButton(TheLetterManager.SecondLetter, _secondLetterPosition, false, true);
        }

        public void InitFirstLetters()
        {
            Vector3 pos = new Vector3(-0.9f, -2.5f);
            char[] startingLetters = TheLetterManager.GetLetters(15);
            for (int i = 0; i < startingLetters.Length; i++)
            {
                if (i > 0)
                    pos.x += 0.80f;
                if (i == 5)
                {
                    pos.x = -2.5f;
                    pos.y -= 0.75f;
                }

                if (i == 12)
                {
                    pos.x = -0.9f;
                    pos.y -= 0.75f;
                }
                LetterBlock letterBlock = InstantiateLetterButton(startingLetters[i], pos);
                PlayerLetters.Add(new LetterPosition(pos, letterBlock));
            }
            pos.x += 0.80f;

            InitPlayerLetters();
        }
        
        private void InitPlayerLetters()
        {
            RemoveWordBtn removeWordBtn = Instantiate(RemoveWordBtnClass);
            removeWordBtn.OnRemoveTouched += RemoveAllLetters;

            _placeWordBtn = Instantiate(PlaceWordBtnClass);
            _placeWordBtn.OnPlaceBtnTouched += PlaceWord;
        }

        private void InitPlacedLetterPositions()
        {
            for (int i = 0; i < 12; i++)
            {
                PlacedLetters.Add(new LetterPosition(new Vector3(-2.5f + 0.45f * i, -1.7f), null));
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
                    PlayerLetters[i].AddLetter(letters[i]);
                }
            }
        }

        private void TradeLetterBtnTouch()
        {
            if (!Player.CanMove) return;
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
            foreach (var t in TheLetterManager.GetLetters(15)) 
            {
                LetterPosition pos = PlayerLetters.FirstOrDefault(x => x.LetterBlock == null);
                LetterBlock letterBlock = InstantiateLetterButton(t, pos.Position);
                pos.AddLetter(letterBlock);
            }
            Player.TimeRemaining -= 10;
            GameState.PlayerManagerClass.NextTurn();
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
                lttrblock.GetComponentsInChildren<TextMesh>()[1].text = TheLetterManager.CharactersValues.First(x => x.Key == char.ToLower(letter)).Value.ToString();
                lttrblock.transform.position = pos;
            });
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

        private void PlaceWord()
        {
            // Alleen wanneer mag versturen
            if (Player.CanMove)
            {
                if (PlacedLetters.Any(x => x.LetterBlock != null))
                {
                    string madeWord = "";
                    List<LetterBlock> blocks = PlacedLetters.Select(x => x.LetterBlock).ToList();
                    foreach (LetterBlock block in blocks)
                    {
                        if (block == null) continue;
                        madeWord += block.GetLetter();
                    }
                    if (!TheLetterManager.CheckWord(madeWord, out long points, PlacedLetters)) return;
                    Player.EarnedPoints += points;
                    TheLetterManager.PlaceWordInGameBoard(PlacedLetters.Select(x => x.LetterBlock).ToList());
                    RemoveAllLettersFromPlayerBoard();
                    AddLetters(madeWord.Length - 2);
                    ChangeFixedLetters(madeWord);
                    _placeWordBtn.GetComponent<MeshRenderer>().material = PlaceButtonInactiveMaterial;
                    GameState.PlayerManagerClass.NextTurn(Player);
                    Player.IncreaseWordCount();
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
            char[] letters = TheLetterManager.GetLetters(amount);
            for (int i = 0; i < amount; i++)
            {
                LetterPosition pos = PlayerLetters.FirstOrDefault(x => x.LetterBlock == null);
                LetterBlock block = InstantiateLetterButton(letters[i], pos.Position);
                pos.AddLetter(block);
            }
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

        public void ChangeFixedLetters(string madeWord)
        {
            TheLetterManager.SecondLetter = TheLetterManager.FirstLetter;
            var lastIndex = madeWord.Length;
            TheLetterManager.FirstLetter = madeWord[lastIndex - 1];
            Destroy(FirstLetterBlock.gameObject);
            Destroy(SecondLetterBlock.gameObject);
            FirstLetterBlock = InstantiateLetterButton(TheLetterManager.FirstLetter, _firstLetterPosition, true);
            SecondLetterBlock = InstantiateLetterButton(TheLetterManager.SecondLetter, _secondLetterPosition, false, true);
        }    

        private void RemoveAllLettersFromPlayerBoard()
        {
            foreach(LetterPosition position in PlacedLetters)
            {
                if (position.LetterBlock == null || position.LetterBlock.IsWalkingLetter()) continue;
                Destroy(position.LetterBlock.gameObject);
                position.RemoveLetter();
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
            if (PlacedLetters.All(x => x.LetterBlock == null)) return;        
            bool containsFirstLetter = false;
            bool containsSecondLetter = false;
            foreach (LetterBlock block in PlacedLetters.Select(x => x.LetterBlock))
            {
                if (block == null) continue;
                madeWord += block.GetLetter();
                if (block.IsFirstLetter) containsFirstLetter = true;
                if (block.IsSecondLetter) containsSecondLetter = true;              
            }
            if (TheLetterManager.Exists(madeWord.ToLower()) && containsFirstLetter && containsSecondLetter)
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