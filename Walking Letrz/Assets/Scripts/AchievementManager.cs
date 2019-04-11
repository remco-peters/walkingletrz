using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using I2.Loc;
using UnityEngine;

public class AchievementManager : MyMonoBehaviour
{
    private int _wordCount;
    private long _points;
    private void Awake()
    {
        DontDestroyOnLoad(this);
        PlayerPrefs.DeleteKey("playerWordCount");
    }
    
    public void SubmitWordCountToAchievements(int count)
    {
        PlayerPrefs.SetInt("playerWordCount", count);
        _wordCount = count;
    }

    internal void SubmitPointsToAchievements(long points)
    {
        PlayerPrefs.SetFloat("playerPointsCount", points);
        _points = points;
    }

    internal string CheckIfAchievementIsGet()
    {
        string achievement = LocalizationManager.GetTranslation("achievement_unlocked");
        string points = LocalizationManager.GetTranslation("achievement_points");
        string wordsPre = LocalizationManager.GetTranslation("achievement_words_prefix");
        string wordsSuf = LocalizationManager.GetTranslation("achievement_words");
        string returnString = "";
        if (_points >= 25)
            returnString += $"25 {points}\n";
        if (_points >= 50)
            returnString += $"50 {points}\n";
        if (_points >= 100)
            returnString += $"100 {points}\n";
        if (_points >= 250)
            returnString += $"250 {points}\n";
        if (_wordCount >= 5)
            returnString += $"{wordsPre} 5 {wordsSuf}\n";
        if (_wordCount >= 10)
            returnString += $"{wordsPre} 10 {wordsSuf}\n";
        if (_wordCount >= 25)
            returnString += $"{wordsPre} 25 {wordsSuf}\n";

        return returnString;
    }
}
