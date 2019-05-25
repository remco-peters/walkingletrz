using System.Collections;
using Assets.Scripts;
using Photon.Pun;
using PlayFab;
using UnityEngine;
using UnityEngine.UI;

public class MyPlayer : Player
{
    public delegate void OnInfoTextChangeDelegate(string newInfo, int time);
    public event OnInfoTextChangeDelegate OnInfoTextChange;

    public string InfoText {
        get { return InfoText; }
        set
        {
            if(value.Length > 0)
            {
                OnInfoTextChange(value, 3);
            }
        }
    }
    public GameObject WriteBoard, LetterBoardObject;
    public AchievementManager AchievementManager { private get; set; }
    public Credit Credit { get; set; }
    public bool IsInTutorial { get; set; } = false;

    private int placedWordCount;
    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        AchievementManager.Player = this;

        CanMove = false;
        placedWordCount = 0;
        Instantiate(Credit);
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();
    }
    
    public void IncreaseWordCount()
    {
        AmountOfWordsPerMinute++;
        AchievementManager.SubmitWordCountPerMinuteToAchievements(AmountOfWordsPerMinute);
        AchievementManager.SubmitWordCountToAchievements(++placedWordCount);
        AchievementManager.SubmitPointsToAchievements(EarnedPoints);
        AchievementManager.SubmitTwelveCharWordToAchievements(WordsWithTwelveLetters);
        InfoText = AchievementManager.CheckIfAchievementIsGet();
    }

    public int GetPlacedWordCount()
    {
        return placedWordCount;
    }
}
