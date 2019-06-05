using System;
using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class DynamicUI : MyMonoBehaviour
{
    public GameObject GameBoardContent;
    public GameObject WritingBoard;
    public GameObject FirstRow;
    public GameObject SecondRow;
    public GameObject ThirdRow;
    public GameObject BoosterPanel;
    public RemoveWordBtn DeleteBtn;
    public PlaceWordBtn PlaceBtn;
    public TradeLettersBtn TradeLettersBtnClass;
    public Button Booster1;
    public Button Booster2;
    public Button Booster3;
    public Button Booster4;
    public Button Booster5;
    public GameObject EmptyLetterBlockObject;
    public LetterBlock FixedLetterBlockObject;
    public LetterBlock PlayerLetterBlockObject;
    public GameObject PlaceHolderObject;
    public MyPlayer PlayerClass;
    public MyBot BotClass;
    public HUD HUDClass;
    public LetterManager LetterManagerClass;
    public PlayerManager PlayerManagerClass;
    public TheLetterManager TheLetterManagerClass;
    public GameObject GameBoardWordHolder;
    public GameObject GameBoardWordContainer;
    public AchievementManager AchievementManagerClass;
    public GameObject PointsGainedPanel;
    public Text PointsGainedText;
    public GameObject PlayerInfoPanel;
    public Text PlayerInfoTxt;
    public Credit CreditClass;
    public GameSceneCounter FixedLetterOverlay;          
    public Material FixedLetterOtherPlayerMaterial;
    public Material PlayerLetterOtherPlayerMaterial;
    public LetterBlock FixedLettersBlockObjectGameBoard;
    public LetterBlock PlayerLetterBlockObjectGameBoard;
    public bool Tutorial { get; set; }

    void Awake()
    {
        TheLetterManagerClass = Spawn(TheLetterManagerClass, this);
    }

    // Start is called before the first frame update
    void Start()
    {
   
        Difficulty difficulty = GameInstance.instance.difficulty;
        Debug.Log(difficulty.ToString());

        Assert.IsNotNull(PlayerClass, "Player misses in GameState");
        Assert.IsNotNull(HUDClass, "HUD misses in GameState");
        Assert.IsNotNull(LetterManagerClass, "LetterManagerClass misses in GameState");
        Assert.IsNotNull(PlayerManagerClass, "PlayerManagerClass misses in GameState");
        Assert.IsNotNull(TheLetterManagerClass, "TheLetterManagerClass misses in GameState");

        AchievementManager achievementManager = Instantiate(AchievementManagerClass);
        MyPlayer localPlayer;
        if(GameInstance.instance.IsMultiplayer)
        {
            localPlayer = Spawn(PlayerClass, this, p => { p.AchievementManager = achievementManager; p.Name = AccountManager.CurrentPlayer.DisplayName; p.Credit = CreditClass; });
            localPlayer.AchievementManager = achievementManager;
            localPlayer.name = PhotonNetwork.LocalPlayer.NickName;
            localPlayer.Credit = CreditClass;
            localPlayer.IsInTutorial = Tutorial;
            HUD HUD = Spawn(HUDClass, hud => { hud.Player = localPlayer; });

            LetterManagerClass = Spawn(LetterManagerClass, this, letterManager =>
            {
                letterManager.DynamicUi = this;
                letterManager.TheLetterManager = TheLetterManagerClass;
                letterManager.Player = localPlayer;
                letterManager.PlaceHolderObject = PlaceHolderObject;
                letterManager.FirstRow = FirstRow;
                letterManager.SecondRow = SecondRow;
                letterManager.ThirdRow = ThirdRow;
                letterManager.WritingBoard = WritingBoard;
                letterManager.DeleteBtn = DeleteBtn;
                letterManager.PlaceBtn = PlaceBtn;
                letterManager.TradeBtn = TradeLettersBtnClass;
                letterManager.EmptyLetterBlockObject = EmptyLetterBlockObject;
                letterManager.FixedLettersBlockObject = FixedLetterBlockObject;
                letterManager.PlayerLetterBlockObject = PlayerLetterBlockObject;
                letterManager.GameBoardWordHolder = GameBoardWordHolder;
                letterManager.GameBoardWordContainer = GameBoardWordContainer;
                letterManager.PointsGainedPanel = PointsGainedPanel;
                letterManager.PointsGainedText = PointsGainedText;
                letterManager.BoosterPanel = BoosterPanel;
                letterManager.Booster1 = Booster1;
                letterManager.Booster2 = Booster2;
                letterManager.Booster3 = Booster3;
                letterManager.Booster4 = Booster4;
                letterManager.Booster5 = Booster5;
                letterManager.FixedLettersBlockObjectGameBoard = FixedLettersBlockObjectGameBoard;
                letterManager.PlayerLetterBlockObjectGameBoard = PlayerLetterBlockObjectGameBoard;
            });

            LetterManager letterManagerBot = LetterManagerClass;

            PlayerManagerClass = Spawn(PlayerManagerClass, this);
            PlayerManagerClass.Players = new List<Player> { localPlayer };
            //localPlayer.TimeRemaining = 120f;
            HUD.PlayersList = PlayerManagerClass.Players;
            BotClass.playerManager = PlayerManagerClass;


            FixedLetterOverlay = Instantiate(FixedLetterOverlay);
            FixedLetterOverlay.transform.SetParent(transform, false);

            if (PhotonNetwork.IsMasterClient)
            {
                FixedLetterOverlay.GetComponent<GameSceneCounter>().OnCountDownFinished = () =>
                {
                    Hashtable hash = new Hashtable {{"CanMove", true}};
                    PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
                };
            }
            else
            {
                FixedLetterOverlay.GetComponent<GameSceneCounter>().OnCountDownFinished = () =>
                {
                    Hashtable hash = new Hashtable {{"CanMove", false}};
                    PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
                };
            }
        }
        else
        {
            localPlayer = Spawn(PlayerClass, this, p => { p.AchievementManager = achievementManager; p.Name = AccountManager.CurrentPlayer.DisplayName; p.Credit = CreditClass; });
            localPlayer.AchievementManager = achievementManager;
            localPlayer.Credit = CreditClass;
            localPlayer.IsInTutorial = Tutorial;
            HUD HUD = Spawn(HUDClass, hud => { hud.Player = localPlayer; });
            
            LetterManagerClass = Spawn(LetterManagerClass, this, letterManager =>
            {
                letterManager.DynamicUi = this;
                letterManager.TheLetterManager = TheLetterManagerClass;
                letterManager.Player = localPlayer;
                letterManager.PlaceHolderObject = PlaceHolderObject;
                letterManager.FirstRow = FirstRow;
                letterManager.SecondRow = SecondRow;
                letterManager.ThirdRow = ThirdRow;
                letterManager.WritingBoard = WritingBoard;
                letterManager.DeleteBtn = DeleteBtn;
                letterManager.PlaceBtn = PlaceBtn;
                letterManager.TradeBtn = TradeLettersBtnClass;
                letterManager.EmptyLetterBlockObject = EmptyLetterBlockObject;
                letterManager.FixedLettersBlockObject = FixedLetterBlockObject;
                letterManager.PlayerLetterBlockObject = PlayerLetterBlockObject;
                letterManager.GameBoardWordHolder = GameBoardWordHolder;
                letterManager.GameBoardWordContainer = GameBoardWordContainer;
                letterManager.PointsGainedPanel = PointsGainedPanel;
                letterManager.PointsGainedText = PointsGainedText;
                letterManager.BoosterPanel = BoosterPanel;
                letterManager.Booster1 = Booster1;
                letterManager.Booster2 = Booster2;
                letterManager.Booster3 = Booster3;
                letterManager.Booster4 = Booster4;
                letterManager.Booster5 = Booster5;
                letterManager.FixedLettersBlockObjectGameBoard = FixedLettersBlockObjectGameBoard;
                letterManager.PlayerLetterBlockObjectGameBoard = PlayerLetterBlockObjectGameBoard;
            });

            LetterManager letterManagerBot = LetterManagerClass;

            BotClass = Spawn(BotClass, this, bot =>
            {
                bot.LetterManager = LetterManagerClass;
                bot.TheLetterManager = TheLetterManagerClass;
                bot.Name = $"{difficulty.ToString()} bot";
                bot.difficulty = difficulty;
                bot.PlayerLetterOtherPlayerMaterial = PlayerLetterOtherPlayerMaterial;
                bot.FixedLetterOtherPlayerMaterial = FixedLetterOtherPlayerMaterial;
            });

            PlayerManagerClass = Spawn(PlayerManagerClass, this);
            PlayerManagerClass.Players = new List<Player> { localPlayer, BotClass };
            
            HUD.PlayersList = PlayerManagerClass.Players;
            BotClass.playerManager = PlayerManagerClass;

            if(!localPlayer.IsInTutorial)
            {
                FixedLetterOverlay = Instantiate(FixedLetterOverlay);
                FixedLetterOverlay.transform.SetParent(transform, false);
            }

            //localPlayer.BestWordsThisGame = null;

            FixedLetterOverlay.GetComponent<GameSceneCounter>().OnCountDownFinished = () =>
                {
                    localPlayer.CanMove = true;
                };
        }
    }

    void Update()
    {

    }
}
