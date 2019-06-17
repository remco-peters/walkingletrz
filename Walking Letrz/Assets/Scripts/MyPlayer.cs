using Assets.Scripts;
using UnityEngine;

public class MyPlayer : Player
{
    public delegate void OnInfoTextChangeDelegate(string newInfo, int time);
    public event OnInfoTextChangeDelegate OnInfoTextChange;

    /// <summary>
    /// When infoText is longer than 0 chars, the delegate OnInfoTextChangeDelegate will be called
    /// </summary>
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
    public int TimesTraded { get; set; } = 0;

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
    
    /// <summary>
    /// Increases the word count and updates all achievement stats and checks if an achievement has been completed
    /// </summary>
    public void IncreaseWordCount()
    {
        AmountOfWordsPerMinute++;
        AchievementManager.SubmitWordCountPerMinuteToAchievements(AmountOfWordsPerMinute);
        AchievementManager.SubmitWordCountToAchievements(++placedWordCount);
        AchievementManager.SubmitPointsToAchievements(EarnedPoints);
        AchievementManager.SubmitTwelveCharWordToAchievements(WordsWithTwelveLetters);
        InfoText = AchievementManager.CheckIfAchievementIsGet();
    }

    /// <summary>
    /// Returns the placed word count
    /// </summary>
    /// <returns></returns> placedWordCount
    public int GetPlacedWordCount()
    {
        return placedWordCount;
    }
}
