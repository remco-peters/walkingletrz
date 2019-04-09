using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementManager : MonoBehaviour
{
    private int _wordCount;
    private void Awake()
    {
        DontDestroyOnLoad(this);
        PlayerPrefs.DeleteKey("playerWordCount");
    }
    
    public void SubmitWordCountToAchievements(int count)
    {
        int storedCount = PlayerPrefs.GetInt("playerWordCount");
        if (storedCount == 0)
        {
            PlayerPrefs.SetInt("playerWordCount", count);
            _wordCount = count;
        }
        else
        {
            count += storedCount;
            PlayerPrefs.SetInt("playedWordCount", count);
            _wordCount = count;
        }
        CheckWordCountAchievement();   
    }

    internal void CheckWordCountAchievement()
    {
        if (_wordCount == 0)
        {
            _wordCount = PlayerPrefs.GetInt("playerWordCount");
        }
        if (_wordCount >= 5)
        {
            Debug.Log("5 word achievement get");
        }
        else if (_wordCount >= 10)
        {
            Debug.Log("10 word achievement get");
        }
        else if (_wordCount >= 25)
        {
            Debug.Log("25 word achievement get");
        }
    }
}
