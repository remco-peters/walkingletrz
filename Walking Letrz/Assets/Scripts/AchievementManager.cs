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
            returnString = "25 point achievement get";
        if (_points >= 50)
            returnString = "50 point achievement get";
        if (_points >= 100)
            returnString = "100 point achievement get";
        if (_points >= 250)
            returnString = "250 point achievement get";
        if (_wordCount == 5)
            returnString = "5 word achievement get";
        if (_wordCount == 10)
            returnString = "10 word achievement get";
        if (_wordCount == 25)
            returnString = "25 word achievement get";

        return returnString;
    }
}
