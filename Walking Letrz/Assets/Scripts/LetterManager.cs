using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Internal.Experimental.UIElements;
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
        public TradeLettersBtn TradeBtn {get;set;}
        public GameObject EmptyLetterBlockObject { get; set; }
        public LetterBlock FixedLettersBlockObject { get; set; }
        public LetterBlock PlayerLetterBlockObject { get; set; }
        public GameObject PlaceHolderObject { get; set; }
        public GameObject GameBoardWordHolder { get; set; }
        public GameObject GameBoardWordContainer { get; set; }
        #endregion UI
        
        #region unity properties
        //public PlayerLetters PlayerLettersClass;
        public LetterBlock LetterBlockObject;
        public TheLetterManager TheLetterManager;
        public StartingLetters StartingLettersClass;
        public Material PlaceButtonInactiveMaterial;
        public Material PlaceButtonActiveMaterial;
        public GameObject PointsGainedPanel { get; set; }
        public Text PointsGainedText { get; set; }

        #endregion unity properties

        #region 
        private List<LetterPosition> PlacedLetters { get; } = new List<LetterPosition>();
        public List<LetterPosition> PlayerLetters { get; } = new List<LetterPosition>();
        private StartingLetters StartLetters { get; set; }
        public MyPlayer Player { get; set; }
        public DynamicUI DynamicUi { get; set; }
        #endregion

        public LetterBlock FirstLetterBlock { get;set; }
        public LetterBlock SecondLetterBlock { get; set; }
        private Image PointsGainedPanelImage;

        #region positions
        private readonly Vector3 _firstLetterPosition = new Vector3(-2.5f, -2.5f);
        private readonly Vector3 _secondLetterPosition = new Vector3(-1.7f, -2.5f);
        #endregion positions

        private void Awake()
        {
        }

        private void Start()
        {
            InitStartingLetters();
            InitFirstLetters();
            InitPlacedLetterPositions();

            _shuffleTimeRemaining = 1;
            _lowPassFilterFactor = AccelerometerUpdateInterval / LowPassKernelWidthInSeconds;
            _shakeDetectionThreshold *= _shakeDetectionThreshold;
            _lowPassValue = Input.acceleration;

            PointsGainedPanelImage = PointsGainedPanel.GetComponent<Image>();
            PointsGainedPanelImage.color = new Color(1f,1f,1f,0f);
            PointsGainedText.color = new Color(1f,1f,1f,0f);
        }               
        private void Update()
        {
            ShufflePlayerLetters();
        }
        
        private void InitStartingLetters()
        {
            FirstLetterBlock = InstantiateLetterButton(TheLetterManager.FirstLetter, new Vector3(), true, false, 1, 0);
            SecondLetterBlock = InstantiateLetterButton(TheLetterManager.SecondLetter, new Vector3(), false, true, 1, 1);
        }

        public void InitFirstLetters()
        {
            int version = 2;
            int row = 0;
            int index = 0;
            Vector3 pos = new Vector3(-0.9f, -2.5f);
            char[] startingLetters = TheLetterManager.FirstPlayerLetters;
            for (int i = 0; i < startingLetters.Length; i++)
            {
                if(version == 2)
                {
                    if(i < 5)
                    {
                        row = 1;
                        index = i;
                    }
                    else if(i < 12)
                    {
                        row = 2;
                        index = i - 5;
                    }
                    else
                    {
                        row = 3;
                        index = i - 12;
                    }
                }
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
                index = index + 2;
                //Todo remove all the positions
                LetterBlock letterBlock = InstantiateLetterButton(startingLetters[i], pos, false, false, row);
                PlayerLetters.Add(new LetterPosition(row, index, letterBlock));
            }
            pos.x += 0.80f;

            InitPlayerLetters();
        }
        
        private void InitPlayerLetters()
        {
            PlaceBtn.OnPlaceBtnTouched += PlaceWord;
            DeleteBtn.OnRemoveTouched += RemoveAllLetters;
            TradeBtn.LetterManager = this;
            TradeBtn.OnTradeTouched += TradeLetterBtnTouch;
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
                    //Todo             
                    //PlayerLetters[i].AddLetter(letters[i]);
                }
            }
        }

        private void TradeLetterBtnTouch()
        {
            if (!Player.CanMove) return;
            foreach (LetterPosition letterPos in PlayerLetters.ToList())
            {
                if (!letterPos.LetterBlock.IsFirstLetter && !letterPos.LetterBlock.IsSecondLetter)
                {
                    int row = letterPos.GetRow();
                    int index = letterPos.GetCurrentIndex();

                    GameObject parentRow = GetRightRow(row);
                    Transform placeHolder = parentRow.transform.GetChild(index);
                    Destroy(letterPos.LetterBlock.gameObject);
                    letterPos.LetterBlock = AddLetter(row, index);   
                }
            }
            DynamicUi.PlayerManagerClass.NextTurn();
        }
        
        public LetterBlock InstantiateLetterButton(char letter, Vector3 pos, bool isFirstLetter = false, bool isSecondLetter = false, int row = 1, int? index = null)
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
                if(index != null)
                {
                    lttrBlock.transform.SetSiblingIndex((int)index);
                }
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
            SetPlaceBtnActivity(false);
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
                        madeWord += block.GetLetter();
                    }
                    if (!TheLetterManager.CheckWord(madeWord, out long points, PlacedLetters)) return;
                    Player.EarnedPoints += points;
                    ShowScoreGainedText(points);
                    //TheLetterManager.PlaceWordInGameBoard(PlacedLetters.Select(x => x.LetterBlock).ToList());
                    PlaceWordInGameBoard();
                    RemoveAllLettersFromPlayerBoard();
                    //AddLetter(madeWord.Length - 2);
                    ChangeFixedLetters(madeWord);
                    GameBoardWordContainer.transform.parent.transform.parent.GetComponent<GameboardScroll>().ScrollDownBar();
                    DynamicUi.PlayerManagerClass.NextTurn();
                    Player.IncreaseWordCount();
                    SetPlaceBtnActivity(false);
                    //DynamicUi.PlayerManagerClass.NextTurn(Player);
                }
                else
                {
                    string noLetters = I2.Loc.LocalizationManager.GetTranslation("no_letters_placed");
                    Player.InfoText = noLetters;
                    Debug.Log("No letters placed yet");
                }
            }
            else
            {
                if (Player.TimeRemaining <= 0) Debug.Log("Time's over. Play again!");
            }
        }

        private LetterBlock AddLetter(int row, int index)
        {
            char[] letters = TheLetterManager.GetLetters(1);
            return InstantiateLetterButton(letters[0], new Vector3(), false, false, row, index);
        }
            
       // TheLetterManager
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
        
        // TheLetterMananger
        public bool Exists(string word)
        {
            return TheLetterManager.AllWords.Contains(word);
        }

        // TheLetterManager
        private void PlaceWordInGameBoard()
        {
            // Insantiate wordHolder
            GameObject wordHolder = Instantiate(GameBoardWordHolder);

            // Walk through all the letters placed
            foreach (LetterBlock block in PlacedLetters.Select(x => x.LetterBlock).ToList())
            {
                if (block != null)
                {
                    block.transform.SetParent(wordHolder.transform);
                    block.GetComponent<Button>().interactable = false;
                    // Replace placeholder with letter on playerBoard
                    LetterPosition letterPos = PlacedLetters.FirstOrDefault(x => x.LetterBlock == block);
                    int row = letterPos.GetRow();
                    int index = letterPos.GetOldIndex();
                    int currentIndex = letterPos.GetOldIndex();
                    PlayerLetters.Remove(letterPos);

                    // Placeholders verwijderen
                    GameObject parentRow = GetRightRow(row);
                    Transform placeHolder = parentRow.transform.GetChild(index);
                    Destroy(placeHolder.gameObject);

                    // Lege gameobjecten toevoegen aan writeboard
                    GameObject emptyBlock = Instantiate(EmptyLetterBlockObject);
                    emptyBlock.transform.SetParent(WritingBoard.transform);
                    emptyBlock.transform.SetSiblingIndex(currentIndex);

                    // Nieuwe playerletters aanmaken
                    if (!block.IsFirstLetter && !block.IsSecondLetter)
                    {
                        AddLetter(row, index);
                    }
                }
                else
                {
                    GameObject emptyPlaceHolder = Instantiate(PlaceHolderObject);
                    emptyPlaceHolder.transform.SetParent(wordHolder.transform);
                }
                wordHolder.transform.SetParent(GameBoardWordContainer.transform);
            }
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
            CheckWordAndSetSubmitButtonState();
        }

        public void ChangeFixedLetters(string madeWord)
        {
            TheLetterManager.SecondLetter = TheLetterManager.FirstLetter;
            var lastIndex = madeWord.Length;
            TheLetterManager.FirstLetter = madeWord[lastIndex - 1];
            FirstLetterBlock = InstantiateLetterButton(TheLetterManager.FirstLetter, new Vector3(), true, false, 1, 0);
            SecondLetterBlock = InstantiateLetterButton(TheLetterManager.SecondLetter, new Vector3(), false, true, 1, 1);         
            }    

        private void RemoveAllLettersFromPlayerBoard()
        {
            foreach (LetterPosition position in PlacedLetters)
            {
                if (position.LetterBlock == null) continue;
                position.RemoveLetter();
            }
        }
        
        private void LetterDragged(LetterBlock draggedLetter)
        {
            //draggedLetter.LetterDragged(PlacedLetters, PlayerLetters);
            //CheckWordAndSetSubmitButtonState();
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
                SetPlaceBtnActivity(true);
            }
            else
            {
                SetPlaceBtnActivity(false);
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
            emptyBlock.transform.localScale.Set(1, 1, 1);

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

        private void SetPlaceBtnActivity(bool SetActive)
        {
            if(SetActive)
            {
                PlaceBtn.GetComponent<CanvasRenderer>().SetMaterial(PlaceButtonActiveMaterial, 0);
                PlaceBtn.GetComponent<Button>().interactable = true;
            } else
            {
                PlaceBtn.GetComponent<CanvasRenderer>().SetMaterial(PlaceButtonInactiveMaterial, 0);
                PlaceBtn.GetComponent<Button>().interactable = false;
            }
            
        }

        private void ShowScoreGainedText(long points)
        {
            PointsGainedText.text = $"+{points.ToString()}";
            StartCoroutine(PointsGainedTimer);
        }

        IEnumerator PointsGainedTimer()
        {
            StartCoroutine(FadeTo(1f, 0.5f));
            yield return new WaitForSeconds(3);
            StartCoroutine(FadeTo(0f, 0.5f));
            StopCoroutine(PointsGainedTimer());
        }
        
        IEnumerator FadeTo(float aValue, float aTime)
        {
            float alpha = PointsGainedPanelImage.color.a;
            for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
            {
                Color panelColor = new Color(1, 1, 1, Mathf.Lerp(alpha,aValue,t));
                PointsGainedPanelImage.color = panelColor;
                Color textColor = new Color(0, 0, 0, Mathf.Lerp(alpha, aValue,t));
                PointsGainedText.color = textColor;
                yield return null;
            }
        }

    }
}