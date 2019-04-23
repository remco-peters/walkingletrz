using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using I2.Loc;
using UnityEngine;

public class AchievementManager : MyMonoBehaviour
{
    private int _wordCount;
    private long _points;
    public MyPlayer Player { get; set; }

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
    
    public void SubmitWordCountToAchievements(int count)
    {
        _wordCount = count;
    }

    internal void SubmitPointsToAchievements(long points)
    {
        _points = points;
    }

    internal string CheckIfAchievementIsGet()
    {
        string achievement = LocalizationManager.GetTranslation("achievement_unlocked");
        string points = LocalizationManager.GetTranslation("achievement_points");
        string wordsPre = LocalizationManager.GetTranslation("achievement_words_prefix");
        string wordsSuf = LocalizationManager.GetTranslation("achievement_words");
        string returnString = "";
        if (PlayerPrefs.GetInt("25PointAchievement") == 0 && _points >= 25)
        {
            PlayerPrefs.SetInt("25PointAchievement", 1);
            Player.Credit.AddCredits(10);
            returnString += $"25 {points}\n";
        }

        if (PlayerPrefs.GetInt("50PointAchievement") == 0 && _points >= 50)
        {
            PlayerPrefs.SetInt("50PointAchievement", 1);
            Player.Credit.AddCredits(25);
            returnString += $"50 {points}\n";
        }

        if (PlayerPrefs.GetInt("100PointAchievement") == 0 && _points >= 100)
        {
            PlayerPrefs.SetInt("100PointAchievement", 1);
            Player.Credit.AddCredits(100);
            returnString += $"100 {points}\n";
        }

        if (PlayerPrefs.GetInt("250PointAchievement") == 0 && _points >= 250)
        {
            PlayerPrefs.SetInt("250PointAchievement", 1);
            Player.Credit.AddCredits(250);
            returnString += $"250 {points}\n";
        }

        if (PlayerPrefs.GetInt("5WordAchievement") == 0 && _wordCount >= 5)
        {
            PlayerPrefs.SetInt("5WordAchievement", 1);
            Player.Credit.AddCredits(10);
            returnString += $"{wordsPre} 5 {wordsSuf}\n";
        }

        if (PlayerPrefs.GetInt("10WordAchievement") == 0 && _wordCount >= 10)
        {
            PlayerPrefs.SetInt("10WordAchievement", 1);
            Player.Credit.AddCredits(25);
            returnString += $"{wordsPre} 10 {wordsSuf}\n";
        }

        if (PlayerPrefs.GetInt("25WordAchievement") == 0 && _wordCount >= 25)
        {
            PlayerPrefs.SetInt("25WordAchievement", 1);
            Player.Credit.AddCredits(50);
            returnString += $"{wordsPre} 25 {wordsSuf}\n";
        }
        return returnString;
    }
}
