using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
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
        string returnString = "";
        if (_points >= 25)
            returnString += "25 points achievement get\n";
        if (_points >= 50)
            returnString += "50 points achievement get\n";
        if (_points >= 100)
            returnString += "100 points achievement get\n";
        if (_points >= 250)
            returnString += "250 points achievement get\n";
        if (_wordCount >= 5)
            returnString += "5 words achievement get\n";
        if (_wordCount >= 10)
            returnString += "10 words achievement get\n";
        if (_wordCount >= 25)
            returnString += "25 words achievement get\n";

        return returnString;
    }
}
