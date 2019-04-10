using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class DynamicUI : MyMonoBehaviour
{
    public GameObject TopBoard;
    public GameObject GameBoardContent;
    public GameObject WritingBoard;
    public GameObject FirstRow;
    public GameObject SecondRow;
    public GameObject ThirdRow;
    public RemoveWordBtn DeleteBtn;
    public PlaceWordBtn PlaceBtn;
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

        MyPlayer player = Instantiate(PlayerClass);
        HUD HUD = Instantiate(HUDClass);
        HUD.Player = player;
        
        LetterManagerClass = Spawn(LetterManagerClass, this, letterManager =>
        {
            letterManager.DynamicUi = this;
            letterManager.TheLetterManager = TheLetterManagerClass;
            letterManager.Player = player;

            letterManager.PlaceHolderObject = PlaceHolderObject;
            letterManager.FirstRow = FirstRow;
            letterManager.SecondRow = SecondRow;
            letterManager.ThirdRow = ThirdRow;
            letterManager.WritingBoard = WritingBoard;
            letterManager.DeleteBtn = DeleteBtn;
            letterManager.PlaceBtn = PlaceBtn;
            letterManager.EmptyLetterBlockObject = EmptyLetterBlockObject;
            letterManager.FixedLettersBlockObject = FixedLetterBlockObject;
            letterManager.PlayerLetterBlockObject = PlayerLetterBlockObject;
            letterManager.GameBoardWordHolder = GameBoardWordHolder;
            letterManager.GameBoardWordContainer = GameBoardWordContainer;
        });
        
        PlayerManagerClass = Spawn(PlayerManagerClass, this);

        BotClass = Spawn(BotClass, this, bot =>
        {
            bot.LetterManager = LetterManagerClass;
            bot.TheLetterManager = TheLetterManagerClass;
        });

        PlayerManagerClass.players = new List<Player> { player }; // Todo add bots or other players
        BotClass.playerManager = PlayerManagerClass;
    }
}
