using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;

public class AchievementManager : MyMonoBehaviour
{
    private static int _wordCount;
    private void Awake()
    {
        DontDestroyOnLoad(this);
        PlayerPrefs.DeleteKey("playerWordCount");
    }
    
    public void SubmitWordCountToAchievements(int count)
    {
//        int storedCount = PlayerPrefs.GetInt("playerWordCount");
        PlayerPrefs.SetInt("playerWordCount", count);
        _wordCount = count;
        Debug.Log($"wordcount before check: {count}");
        CheckWordCountAchievement();
    }

    internal string CheckWordCountAchievement()
    {
        
//        Debug.Log($"wordcount: {_wordCount}");
        if (_wordCount == 5)
        {
            return "5 word achievement get";
        }
        if (_wordCount == 10)
        {
            return "10 word achievement get";
        }
        if (_wordCount == 25)
        {
            return "25 word achievement get";
        }

        return "";
    }
}
