using System;
using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class DynamicUI : MyMonoBehaviour
{
    public GameObject GameBoardContent;
    public GameObject WritingBoard;
    public GameObject FirstRow;
    public GameObject SecondRow;
    public GameObject ThirdRow;
    public GameObject BoosterBoard;
    public RemoveWordBtn DeleteBtn;
    public PlaceWordBtn PlaceBtn;
    public BoosterBtn BoosterBtn;
    public GenericButton TradeFixedLettersBtn;
    public GenericButton DoubleWordValueBtn;
    public TradeLettersBtn TradeLettersBtnClass;
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
    public Credit CreditClass;

    void Awake()
    {
        TheLetterManagerClass = Spawn(TheLetterManagerClass, this);
    }

    // Start is called before the first frame update
    void Start()
    {
        Assert.IsNotNull(PlayerClass, "Player misses in GameState");
        Assert.IsNotNull(HUDClass, "HUD misses in GameState");
        Assert.IsNotNull(LetterManagerClass, "LetterManagerClass misses in GameState");
        Assert.IsNotNull(PlayerManagerClass, "PlayerManagerClass misses in GameState");
        Assert.IsNotNull(TheLetterManagerClass, "TheLetterManagerClass misses in GameState");

        AchievementManager achievementManager = Instantiate(AchievementManagerClass);
        MyPlayer player = Spawn(PlayerClass, this, p => { p.AchievementManager = achievementManager; p.Name = "Ik"; p.Credit = CreditClass;});
         
        HUD HUD = Instantiate(HUDClass);
        HUD.Player = player;
        
        LetterManagerClass = Spawn(LetterManagerClass, this, letterManager =>
        {
            letterManager.DynamicUi = this;
            letterManager.TheLetterManager = TheLetterManagerClass;
            letterManager.Player = player;
            letterManager.TradeFixedLetterSBtn = TradeFixedLettersBtn;
            letterManager.PlaceHolderObject = PlaceHolderObject;
            letterManager.FirstRow = FirstRow;
            letterManager.SecondRow = SecondRow;
            letterManager.ThirdRow = ThirdRow;
            letterManager.WritingBoard = WritingBoard;
            letterManager.DeleteBtn = DeleteBtn;
            letterManager.PlaceBtn = PlaceBtn;
            letterManager.TradeBtn = TradeLettersBtnClass;
            letterManager.BoosterBtn = BoosterBtn;
            letterManager.EmptyLetterBlockObject = EmptyLetterBlockObject;
            letterManager.FixedLettersBlockObject = FixedLetterBlockObject;
            letterManager.PlayerLetterBlockObject = PlayerLetterBlockObject;
            letterManager.GameBoardWordHolder = GameBoardWordHolder;
            letterManager.GameBoardWordContainer = GameBoardWordContainer;
            letterManager.PointsGainedPanel = PointsGainedPanel;
            letterManager.PointsGainedText = PointsGainedText;
            letterManager.BoosterBoard = BoosterBoard;
            letterManager.DoubleWordValueBtn = DoubleWordValueBtn;
        });

        LetterManager letterManagerBot = LetterManagerClass;
        
        PlayerManagerClass = Spawn(PlayerManagerClass, this);

        BotClass = Spawn(BotClass, this, bot =>
        {
            bot.LetterManager = LetterManagerClass;
            bot.TheLetterManager = TheLetterManagerClass;
            bot.Name = "Medium bot";
        });
        
        //PlayerManagerClass.Players = new List<Player> { player }; // Todo add bots or other players
        PlayerManagerClass.Players = new List<Player> { player, BotClass };//todo add bots or other players

        HUD.PlayersList = PlayerManagerClass.Players;

        BotClass.playerManager = PlayerManagerClass;

    }
}
