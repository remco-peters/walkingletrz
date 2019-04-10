using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;
using UnityEngine.Assertions;

public class GameState : MyMonoBehaviour
{
    public GameObject GameBoardClass;
    public Camera CameraClass;
    public MyPlayer PlayerClass;
    public MyBot BotClass;
    public HUD HUDClass;
    public LetterManager LetterManagerClass;
    public PlayerManager PlayerManagerClass;
    public TheLetterManager TheLetterManager;

    // Start is called before the first frame update
    void Start()
    {
        Assert.IsNotNull(CameraClass, "Camera misses in GameState");
        Assert.IsNotNull(PlayerClass, "Player misses in GameState");
        Assert.IsNotNull(HUDClass, "HUD misses in GameState");
        Assert.IsNotNull(GameBoardClass, "GameBoard misses in GameState");
        Assert.IsNotNull(LetterManagerClass, "LetterManagerClass misses in GameState");
        Assert.IsNotNull(PlayerManagerClass, "PlayerManagerClass misses in GameState");
        Instantiate(GameBoardClass);
        Instantiate(CameraClass);
        MyPlayer player = Instantiate(PlayerClass);
        HUD HUD = Instantiate(HUDClass);
        HUD.Player = player;
        TheLetterManager = Instantiate(TheLetterManager);
        LetterManagerClass = Spawn(LetterManagerClass, this, letterManager =>
        {
            letterManager.TheLetterManager = TheLetterManager;
            letterManager.Player = player;
            letterManager.GameState = this;
        });
        PlayerManagerClass = Spawn(PlayerManagerClass, this);
        BotClass = Spawn(BotClass, this, bot => { 
            bot.LetterManager = LetterManagerClass;
            bot.TheLetterManager = TheLetterManager;
        });
        PlayerManagerClass.Players = new List<Player> {player, BotClass}; //todo add bots or other players
        BotClass.playerManager = PlayerManagerClass;
    }
}
