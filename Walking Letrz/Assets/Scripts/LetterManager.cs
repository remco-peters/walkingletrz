using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;
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
        public Button Booster1{get;set;}
        public Button Booster2{get;set;}
        public Button Booster3{get;set;}
        public Button Booster4{get;set;}
        public Button Booster5{get;set;}
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
        public Material PlaceButtonInactiveMaterial;
        public Material PlaceButtonActiveMaterial;
        public int RotateBy = 12;
        public GameObject PointsGainedPanel { get; set; }
        public Text PointsGainedText { get; set; }
        public GameObject BoosterPanel{get;set;}
        private Text BoosterText;
        private int BoostersUsed = 0;
        private long credits;

        #endregion unity properties

        #region other properties
        public List<LetterPosition> PlacedLetters { get; } = new List<LetterPosition>();
        public List<LetterPosition> PlayerLetters { get; } = new List<LetterPosition>();
        public MyPlayer Player { get; set; }
        public DynamicUI DynamicUi { get; set; }
        private bool DoubleWordValue = false;
        private bool TripleWordValue = false;
        public LetterBlock FirstLetterBlock { get;set; }
        public LetterBlock SecondLetterBlock { get; set; }
        private Image PointsGainedPanelImage;
        private GameBoard _gameBoard;
        public UnityAction<long, List<LetterPosition>, string> OnWordPlaced;

        #endregion
        
        

        private void Start()
        {          
            credits = GameInstance.instance.credits;
            InitBoosterButtons();
            if(GameInstance.instance.IsMultiplayer)
            {
                _gameBoard = GameBoardWordContainer.GetComponent<GameBoard>();
                _gameBoard.GameBoardWordHolder = GameBoardWordHolder;
                _gameBoard.LetterManager = this;
                _gameBoard.PlaceHolderObject = PlaceHolderObject;
                _gameBoard.TheLM = TheLetterManager;
                _gameBoard.FixedLettersBlockObject = FixedLettersBlockObject;
                _gameBoard.PlayerLettersBlockObject = PlayerLetterBlockObject;

                if (PhotonNetwork.IsMasterClient)
                {
                    InitStartingLetters();
                    InitFirstLetters();
                }
            }
            else
            {
                InitStartingLetters();
                InitFirstLetters();
            }
            

            InitPlacedLetterPositions();
            
            _shuffleTimeRemaining = 1;
            _lowPassFilterFactor = AccelerometerUpdateInterval / LowPassKernelWidthInSeconds;
            _shakeDetectionThreshold *= _shakeDetectionThreshold;
            _lowPassValue = Input.acceleration;

            SetPanelsReady();
            
            if (GameInstance.instance.difficulty == Difficulty.Easy)
            {
                ThirdRow.GetComponent<HorizontalLayoutGroup>().padding.left = 20;
                ThirdRow.GetComponent<HorizontalLayoutGroup>().padding.right = 20;
            }
        }

        private void SetPanelsReady()
        {
            PointsGainedPanelImage = PointsGainedPanel.GetComponent<Image>();
            PointsGainedPanelImage.color = new Color(1f, 1f, 1f, 0f);
            PointsGainedText = PointsGainedPanel.GetComponentInChildren<Text>();
            PointsGainedText.color = new Color(1f, 1f, 1f, 0f);
            BoosterText = BoosterPanel.GetComponentInChildren<Text>();
        }

        private void Update()
        {
            ShufflePlayerLetters();
        }
        
        private void InitStartingLetters()
        {
            FirstLetterBlock = InstantiateLetterButton(TheLetterManager.FirstLetter, true, false, 1, 0);
            SecondLetterBlock = InstantiateLetterButton(TheLetterManager.SecondLetter, false, true, 1, 1);

            if(GameInstance.instance.IsMultiplayer)
            {
                _gameBoard.CallRPCPlaceLtrz(TheLetterManager.FirstLetter.ToString(), true, false, 1, 0);
                _gameBoard.CallRPCPlaceLtrz(TheLetterManager.SecondLetter.ToString(), false, true, 1, 1);

            }
        }

        private void InitBoosterButtons()
        {
            Booster2.onClick.AddListener(DoubleWordOnTouched); 
            Booster3.onClick.AddListener(TripleWordOnTouched);
            Booster4.onClick.AddListener(OnTradeFixedTouched);
            Booster5.onClick.AddListener(ExtraTimeTouched);
        }

        public List<LetterPosition> GetPlayerLetters()
        {
            List<LetterPosition> playerLetters = new List<LetterPosition>();
            for (int row = 1; row <= 3; row++)
            {
                GameObject r = GetRightRow(row);
                LetterBlock[] b = r.GetComponentsInChildren<LetterBlock>();
                foreach (LetterBlock block in b)
                {
                    LetterPosition pos = PlayerLetters.FirstOrDefault(p => p.LetterBlock == block);
                    playerLetters.Add(new LetterPosition(pos.GetRow(), pos.GetOldIndex(), block));
                }
            }
            return playerLetters;           
        }

        public void InitFirstLetters()
        {
            char[] startingLetters = TheLetterManager.FirstPlayerLetters;
            for (int i = 0; i < startingLetters.Length; i++)
            {
                if(i < 5)
                {
                    InstantiateLetterButton(startingLetters[i], false, false, 1, i + 2);
                    if (GameInstance.instance.IsMultiplayer)
                    {
                        _gameBoard.CallRPCPlaceLtrz(startingLetters[i].ToString(), false, false, 1, i + 2);
                    }
                }
                else if(i < 12)
                {
                    InstantiateLetterButton(startingLetters[i], false, false, 2, i - 5);
                    if (GameInstance.instance.IsMultiplayer)
                    {
                        _gameBoard.CallRPCPlaceLtrz(startingLetters[i].ToString(), false, false, 2, i - 5);
                    }
                }
                else
                {
                    InstantiateLetterButton(startingLetters[i], false, false, 3, i - 12);
                    if (GameInstance.instance.IsMultiplayer)
                    {
                        _gameBoard.CallRPCPlaceLtrz(startingLetters[i].ToString(), false, false, 3, i - 12);
                    }
                }                                           
            }
            InitPlayerLetters();
            if (GameInstance.instance.IsMultiplayer)
            {
                _gameBoard.CallRPCInitPlayerLetters();
            }
        }
        
        public void InitPlayerLetters()
        {
            PlaceBtn.OnPlaceBtnTouched += PlaceWord;
            PlaceBtn.OnPlaceBtnTouchedWhileInteractive += ShowPlayerWhyInactive;
            DeleteBtn.OnRemoveTouched += RemoveAllLetters;
            TradeBtn.LetterManager = this;
            TradeBtn.OnTradeTouched += TradeLetterBtnTouch;
        }

        public void DoubleWordOnTouched()
        {
            bool canMove = GameInstance.instance.IsMultiplayer ?  (bool)PhotonNetwork.LocalPlayer.CustomProperties["CanMove"] : Player.CanMove;
            if (BoostersUsed >= 3|| credits < 40 || !canMove) return;
            Player.Credit.RemoveCredits(40);
            credits -= 40;
            DoubleWordValue = true;
            BoosterText.text = TripleWordValue ? "6x" : "2x";
            Booster2.interactable = false;
            BoostersUsed++;
            if (BoostersUsed >= 3) SetBoostersInactive();
        }

        public void TripleWordOnTouched()
        {
            bool canMove = GameInstance.instance.IsMultiplayer ?  (bool)PhotonNetwork.LocalPlayer.CustomProperties["CanMove"] : Player.CanMove;
            if (BoostersUsed >= 3 || credits < 60 || !canMove) return;
            Player.Credit.RemoveCredits(60);
            credits -= 60;
            TripleWordValue = true;
            BoosterText.text = DoubleWordValue ? "6x" : "3x";
            Booster3.interactable = false;
            BoostersUsed++;
            if (BoostersUsed >= 3) SetBoostersInactive();
        }

        public void ExtraTimeTouched()
        {
            bool canMove = GameInstance.instance.IsMultiplayer ?  (bool)PhotonNetwork.LocalPlayer.CustomProperties["CanMove"] : Player.CanMove;
            if (BoostersUsed >= 3|| credits < 50 || !canMove) return;
            Player.Credit.RemoveCredits(50);
            credits -= 50;
            Player.TimeRemaining += 30f;
            Booster5.interactable = false;
            BoostersUsed++;
            if (BoostersUsed >= 3) SetBoostersInactive();
        }

        private void OnTradeFixedTouched()
        {
            bool canMove = GameInstance.instance.IsMultiplayer ?  (bool)PhotonNetwork.LocalPlayer.CustomProperties["CanMove"] : Player.CanMove;
            if (BoostersUsed >= 3|| credits < 20 || !canMove) return;
            Player.Credit.RemoveCredits(20);
            credits -= 20;
            TheLetterManager.FirstLetter = TheLetterManager.GetVowelOrConsonant(GameInstance.instance.difficulty == Difficulty.Medium);
            TheLetterManager.SecondLetter = TheLetterManager.GetVowelOrConsonant(GameInstance.instance.difficulty != Difficulty.Hard);
            FirstLetterBlock.GetComponentInChildren<Text>().text = TheLetterManager.FirstLetter.ToString().ToUpper();
            SecondLetterBlock.GetComponentInChildren<Text>().text = TheLetterManager.SecondLetter.ToString().ToUpper();
            FirstLetterBlock.GetComponentsInChildren<Text>()[1].text = TheLetterManager.CharactersValues.FirstOrDefault(x => x.Key == TheLetterManager.FirstLetter).Value.ToString().ToUpper();
            SecondLetterBlock.GetComponentsInChildren<Text>()[1].text = TheLetterManager.CharactersValues.FirstOrDefault(x => x.Key == TheLetterManager.SecondLetter).Value.ToString().ToUpper();
            Booster4.interactable = false;
            BoostersUsed++;
            if (BoostersUsed >= 3) SetBoostersInactive();
        }

        private void SetBoostersInactive()
        {
                Booster1.interactable = false;
                Booster2.interactable = false;
                Booster3.interactable = false;
                Booster4.interactable = false;
                Booster5.interactable = false;
        }

        private void InitPlacedLetterPositions()
        {
            for (int i = 0; i < 12; i++)
            {
                GameObject elbi = Instantiate(EmptyLetterBlockObject);
                elbi.transform.SetParent(WritingBoard.transform, false);
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
                List<LetterBlock> letters = PlayerLetters.Select(x => x.LetterBlock).OrderBy(a => Random.Range(0, 100)).ToList();
                for (int i = 0; i < PlayerLetters.Count; i++)
                {
                    PlayerLetters[i].AddLetter(letters[i], PlayerLetters[i].GetOldIndex(), PlayerLetters[i].GetRow());
                }
            }
        }

        private void TradeLetterBtnTouch()
        {
            bool canMove;
            if (GameInstance.instance.IsMultiplayer)
            {
                canMove = (bool)PhotonNetwork.LocalPlayer.CustomProperties["CanMove"];
            } else
            {
                canMove = Player.CanMove;
            }

            int timesTraded;
            if (GameInstance.instance.IsMultiplayer)
                timesTraded = (int) PhotonNetwork.LocalPlayer.CustomProperties["TimesTraded"];
            else
                timesTraded = Player.TimesTraded;

            switch (timesTraded)
            {
                case 0:
                    TradeLetters(timesTraded);
                    TradeBtn.ButtonText.text = I2.Loc.LocalizationManager.GetTranslation("10_points");
                    break;
                case 1:
                    if (Player.EarnedPoints < 10)
                        Player.InfoText = I2.Loc.LocalizationManager.GetTranslation("trade_10_points");
                    else
                    {
                        TradeLetters(timesTraded);
                        TradeBtn.ButtonText.text = I2.Loc.LocalizationManager.GetTranslation("20_points");
                        Player.EarnedPoints -= 10;
                    }
                    break;
                case 2:
                    if (Player.EarnedPoints < 20)
                        Player.InfoText = I2.Loc.LocalizationManager.GetTranslation("trade_20_points");
                    else
                    {
                        var subtractColor = new Color(0, 0, 0, 0.5f);
                        TradeLetters(timesTraded);
                        Button trade = TradeBtn.GetComponent<Button>();
                        trade.image.color -= subtractColor;
                        TradeBtn.ButtonText.color -= subtractColor;
                        TradeBtn.TradeRotateImage.color -= subtractColor;
                        Player.EarnedPoints -= 20;
                    }
                    break;
                default:
                    Player.InfoText = I2.Loc.LocalizationManager.GetTranslation("trade_used_all");
                    break;
            }
            
        }

        private void TradeLetters(int timesTraded)
        {
            if (GameInstance.instance.IsMultiplayer)
            {
                Hashtable hash = new Hashtable { { "TimesTraded", ++timesTraded} };
                PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
            } 
            else
                Player.TimesTraded++;
            RemoveAllLetters();
            var buttonImage = TradeBtn.GetComponentsInChildren<RectTransform>().Where(img => img.name == "TradeBtnImg").ToList()[0];
            StartCoroutine(RotateTradeButton(buttonImage, 1));
            List<LetterPosition> letterPositions = GetPlayerLetters();
            char[] newLetters = TheLetterManager.GetLetters(letterPositions.Count());
            for (int i = 0; i < letterPositions.Count; i++)
            {
                LetterPosition letterPos = letterPositions[i];
                if (!letterPos.LetterBlock.IsFirstLetter && !letterPos.LetterBlock.IsSecondLetter)
                {
                    letterPos.LetterBlock.GetComponentInChildren<Text>().text = newLetters[i].ToString().ToUpper();  
                }
            }
        }
        
        public LetterBlock InstantiateLetterButton(char letter, bool isFirstLetter = false, bool isSecondLetter = false, int row = 1, int? index = null)
        {
            LetterBlock block;
            if (isFirstLetter || isSecondLetter)
            {
                block = FixedLettersBlockObject;
                block = Instantiate(block);
                block.IsFirstLetter = isFirstLetter;
                block.IsSecondLetter = isSecondLetter; 
                block.OnLetterTouched += LetterTouched;
                //Todo
                //lttrBlock.OnLetterDragged += LetterDragged;
                block.GetComponentsInChildren<Text>()[0].text = letter.ToString().ToUpper();
                if (letter != '?')
                block.GetComponentsInChildren<Text>()[1].text = TheLetterManager.CharactersValues
                    .First(x => x.Key == char.ToLower(letter)).Value.ToString();
                else block.GetComponentsInChildren<Text>()[1].text = "";
                GameObject parentRow = GetRightRow(row);
                block.transform.SetParent(parentRow.transform, false);
                if (index != null)
                {
                    block.transform.SetSiblingIndex((int) index);
                }

                PlayerLetters.Add(new LetterPosition(row, block.transform.GetSiblingIndex(), block));
                //if (isFirstLetter) FirstLetterBlock = block;
                //if (isSecondLetter) SecondLetterBlock = block;
                return block;
            }

            block = PlayerLetterBlockObject;
            return Spawn(block, this, lttrBlock =>
            {
                lttrBlock.IsFirstLetter = isFirstLetter;
                lttrBlock.IsSecondLetter = isSecondLetter;
                lttrBlock.OnLetterTouched += LetterTouched;
                //Todo
                //lttrBlock.OnLetterDragged += LetterDragged;
                lttrBlock.GetComponentsInChildren<Text>()[0].text = letter.ToString().ToUpper();
                lttrBlock.GetComponentsInChildren<Text>()[1].text = TheLetterManager.CharactersValues.First(x => x.Key == char.ToLower(letter)).Value.ToString();
                GameObject parentRow = GetRightRow(row);
                lttrBlock.transform.SetParent(parentRow.transform, false);
                if(index != null)
                {
                    lttrBlock.transform.SetSiblingIndex((int)index);
                }
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
            SetPlaceBtnActivity(false);
        }
        
        private void PlaceWord()
        {
            // Alleen wanneer mag versturen
            bool canMove;
            if (GameInstance.instance.IsMultiplayer)
            {
                canMove = (bool)PhotonNetwork.LocalPlayer.CustomProperties["CanMove"];
            } else
            {
                canMove = Player.CanMove;
            }

            if (canMove)
            {
                if (PlacedLetters.Any(x => x.LetterBlock != null))
                {
                    string madeWord = "";
                    foreach (LetterBlock block in PlacedLetters.Select(x => x.LetterBlock).ToList())
                    {
                        if (block == null) continue;
                        madeWord += block.GetLetter();
                    }
                    if (!TheLetterManager.CheckWord(madeWord, out long points, PlacedLetters, Player)) return;
                    int bestWordIndex = Player.BestWordsThisGame.Count(word => word.points > points);
                    Player.BestWordsThisGame.Insert(bestWordIndex, new Word(madeWord, points));
                    if (DoubleWordValue) points *= 2;
                    if (TripleWordValue) points *= 3;
                    DoubleWordValue = false;
                    TripleWordValue = false;
                    Player.EarnedPoints += points;
                    
                    if(madeWord.Count() == 12)
                    {
                        Player.WordsWithTwelveLetters++;
                    }

                    //TheLetterManager.PlaceWordInGameBoard(PlacedLetters.Select(x => x.LetterBlock).ToList()); Verplaatsen naar TheLetterManager
                    DynamicUi.PlayerManagerClass.NextTurn();
                    PlaceWordInGameBoard(points);
                    RemoveAllLettersFromPlayerBoard();
                    ChangeFixedLetters(madeWord);
                    GameBoardWordContainer.transform.parent.transform.parent.GetComponent<GameboardScroll>().ScrollDownBar();
                    Player.IncreaseWordCount();
                    SetPlaceBtnActivity(false);
                    BoosterText.text = "";
                }
            }
        }

        private void ShowPlayerWhyInactive()
        {
            bool canMove;
            if (GameInstance.instance.IsMultiplayer)
            {
                canMove = (bool)PhotonNetwork.LocalPlayer.CustomProperties["CanMove"];
            } else
            {
                canMove = Player.CanMove;
            }

            if(!canMove)
            {
                Player.InfoText = I2.Loc.LocalizationManager.GetTranslation("info_not_your_turn");
            } else
            {
                CheckWordAndSetSubmitButtonState(true);
            }

        }

        public LetterBlock AddLetter(int row, int index)
        {
            List<char> myLetters = GetPlayerLetters().Select(p => char.ToLower(p?.LetterBlock?.GetLetter() ?? default)).ToList();
            char[] letters = TheLetterManager.GetLetters(1, myLetters);
            LetterBlock block = InstantiateLetterButton(letters[0], false, false, row, index);
            PlayerLetters.Add(new LetterPosition(row, index, block));
            return block; 
        }

        // TheLetterManager
        private void PlaceWordInGameBoard(long points = 0)
        {
            if (GameInstance.instance.IsMultiplayer)
            {
                //bestWords = Player.BestWordsThisGame.Select(w => w.word).ToList();
                List<string> bestWords = Player.BestWordsThisGame.Select(w => w.word).ToList();
                string w1 = "";
                string w2 = "";
                string w3 = "";

                if (bestWords.Count > 0)
                {
                    w1 = bestWords[0];
                    if (bestWords.Count > 1)
                    {
                        w2 = bestWords[1];
                        if (bestWords.Count > 2)
                        {
                            w3 = bestWords[2];
                        }
                    }
                }
                
                long playerPoints = (long)PhotonNetwork.LocalPlayer.CustomProperties["Points"];
                Hashtable hash = new Hashtable { { "Points", playerPoints + points }, { "BestWords1", w1 }, { "BestWords2", w2 }, { "BestWords3", w3 } };
                PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
                _gameBoard.CallRPC(points, PlacedLetters, PhotonNetwork.LocalPlayer.UserId);
            } else
            {
                //Instantiate wordHolder
                GameObject wordHolder = Instantiate(GameBoardWordHolder);
                int i = 0;
                // Walk through all the letters placed
                foreach (LetterPosition letterPos in PlacedLetters)
                {
                    LetterBlock block = letterPos.LetterBlock;
                    if (block != null)
                    {
                        i++;
                        block.transform.SetParent(wordHolder.transform, false);
                        block.GetComponent<Button>().interactable = false;
                        
                        Vector3 pos = block.transform.position;
                        ShowScoreGainedText(points, pos);

                        // Replace placeholder with letter on playerBoard
                        int row = letterPos.GetRow();
                        int index = letterPos.GetOldIndex();
                        int currentIndex = letterPos.GetCurrentIndex();
                        PlayerLetters.Remove(letterPos);

                        if (!block.IsFirstLetter && !block.IsSecondLetter)
                        {
                            // Placeholders verwijderen
                            GameObject parentRow = GetRightRow(row);
                            Transform placeHolder = parentRow.transform.GetChild(index);
                            DestroyImmediate(placeHolder.gameObject);
                        }

                        // Lege gameobjecten toevoegen aan writeboard
                        GameObject emptyBlock = Instantiate(EmptyLetterBlockObject);
                        emptyBlock.transform.SetParent(WritingBoard.transform, false);
                        emptyBlock.transform.SetSiblingIndex(currentIndex);

                        // Nieuwe playerletters aanmaken
                        if (!block.IsFirstLetter && !block.IsSecondLetter)
                        {
                            AddLetter(row, index);
                        }

                    }
                }
                for (;i <= 12; i++)
                {
                    GameObject emptyPlaceHolder = Instantiate(PlaceHolderObject);
                    emptyPlaceHolder.transform.SetParent(wordHolder.transform, false);
                }
                wordHolder.transform.SetParent(GameBoardWordContainer.transform, false);
            }
        }

        public void LetterTouched(LetterBlock block)
        {
            bool canMove = GameInstance.instance.IsMultiplayer ?  (bool)PhotonNetwork.LocalPlayer.CustomProperties["CanMove"] : Player.CanMove;
            if (!canMove) return;
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
                block.transform.SetParent(WritingBoard.transform, false);
                block.transform.SetSiblingIndex(EmptyLetterBlock.GetCurrentIndex());
                
                // Een lege placeholder plaatsen waar de letter vandaan is gehaald
                GameObject ph = Instantiate(PlaceHolderObject);
                GameObject parentRow = GetRightRow(letterBlock.GetRow());
                ph.transform.SetParent(parentRow.transform, false);
                ph.transform.SetSiblingIndex(letterBlock.GetCurrentIndex());
            }
            CheckWordAndSetSubmitButtonState();
        }

        public void ChangeFixedLetters(string madeWord, bool isBot = false)
        {
            TheLetterManager.SecondLetter = TheLetterManager.FirstLetter;
            var lastIndex = madeWord.Length;
            TheLetterManager.FirstLetter = madeWord[lastIndex - 1];
            
            if (isBot)
            {
                FirstLetterBlock.GetComponentInChildren<Text>().text = TheLetterManager.FirstLetter.ToString().ToUpper();
                SecondLetterBlock.GetComponentInChildren<Text>().text = TheLetterManager.SecondLetter.ToString().ToUpper();
                FirstLetterBlock.GetComponentsInChildren<Text>()[1].text = TheLetterManager.CharactersValues
                    .First(x => x.Key == char.ToLower(TheLetterManager.FirstLetter)).Value.ToString();
                SecondLetterBlock.GetComponentsInChildren<Text>()[1].text = TheLetterManager.CharactersValues
                    .First(x => x.Key == char.ToLower(TheLetterManager.SecondLetter)).Value.ToString();
                RemoveAllLetters();
            }
            else
            {
                GameObject parentRow = GetRightRow(1);
                Transform placeHolder = parentRow.transform.GetChild(0);
                DestroyImmediate(placeHolder.gameObject);
                FirstLetterBlock = InstantiateLetterButton('?', true, false, 1, 0);
                parentRow = GetRightRow(1);
                placeHolder = parentRow.transform.GetChild(1);
                DestroyImmediate(placeHolder.gameObject);
                SecondLetterBlock = InstantiateLetterButton('?', false, true, 1, 1);

                if (GameInstance.instance.IsMultiplayer)
                {
                    _gameBoard.CallRPCPlaceLtrz(TheLetterManager.FirstLetter.ToString(), true, false, 1, 0, 1);
                    _gameBoard.CallRPCPlaceLtrz(TheLetterManager.SecondLetter.ToString(), false, true, 1, 1, 1);
                }
            }
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

        private void CheckWordAndSetSubmitButtonState(bool placeBtnClick = false)
        {
            string madeWord = "";
            if (PlacedLetters.All(x => x.LetterBlock == null))
            {
                if(placeBtnClick)
                {   
                    Player.InfoText = I2.Loc.LocalizationManager.GetTranslation("no_letters_placed");
                }
                return;
            }
            bool containsFirstLetter = false;
            bool containsSecondLetter = false;
            foreach (LetterBlock block in PlacedLetters.Select(x => x.LetterBlock))
            {
                if (block == null) continue;
                madeWord += block.GetLetter();
                if (block.IsFirstLetter) containsFirstLetter = true;
                if (block.IsSecondLetter) containsSecondLetter = true;
            }
            if(!containsFirstLetter || !containsSecondLetter) {
                if (placeBtnClick)
                {
                    Player.InfoText = I2.Loc.LocalizationManager.GetTranslation("not_both_fixed_letters");
                }
            }
            else if (TheLetterManager.Exists(madeWord.ToLower()) && containsFirstLetter && containsSecondLetter)
            {
                SetPlaceBtnActivity(true);
            }
            else
            {
                if (placeBtnClick)
                {
                    Player.InfoText = I2.Loc.LocalizationManager.GetTranslation("info_word_does_not_exists");
                }
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
            emptyBlock.transform.SetParent(WritingBoard.transform, false);
            emptyBlock.transform.SetSiblingIndex(currentIndex);

            block.transform.SetParent(parentRow.transform, false);
            block.transform.SetSiblingIndex(oldIndex);
        }

        public GameObject GetRightRow(int row)
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

        public void ShowScoreGainedText(long points, Vector3 pos)
        {
            PointsGainedPanel.transform.position = pos;
            PointsGainedText.text = $"+{points.ToString()}";
            StartCoroutine(ShowInfoTextTimer(PointsGainedPanelImage, PointsGainedText, 3));
        }
        
        IEnumerator ShowInfoTextTimer(Image imageObj, Text txtObj, float time)
        {
            StartCoroutine(FadeTo(1f, 0.5f, imageObj, txtObj));
            yield return new WaitForSeconds(time);
            StartCoroutine(FadeTo(0f, 0.5f, imageObj, txtObj));
            StopCoroutine(ShowInfoTextTimer(imageObj, txtObj, time));
        }
        
        IEnumerator FadeTo(float aValue, float aTime, Image imageObj, Text txtObj)
        {
            float alpha = imageObj.color.a;
            for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
            {
                Color panelColor = new Color(1, 1, 1, Mathf.Lerp(alpha,aValue,t));
                imageObj.color = panelColor;
                Color textColor = new Color(0, 0, 0, Mathf.Lerp(alpha, aValue,t));
                txtObj.color = textColor;
                yield return null;
            }
        }
        
        IEnumerator RotateTradeButton(RectTransform tradeButtonImage, float duration)
        {
            var eulerAngles = tradeButtonImage.transform.eulerAngles;
            float startRotation = eulerAngles.z;
            float endRotation = startRotation + 360.0f;
            float t = 0.0f;
            while (t < duration)
            {
                t += Time.deltaTime;
                float zRotation = Mathf.Lerp(startRotation, endRotation, t / duration) % 360.0f;
                eulerAngles = new Vector3(eulerAngles.x, eulerAngles.y, zRotation);
                tradeButtonImage.transform.eulerAngles = eulerAngles;
                yield return null;
            }
        }
    }
}