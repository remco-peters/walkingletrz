using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInstance : MonoBehaviour
{
    public static AudioSource PlopSound;
    private static int gameTime;
    private static int vibrationMode;
    private static string language;
    public static GameInstance instance = null;
    public bool IsMultiplayer { get; set; }
    public List<PlayerData> PlayerData = new List<PlayerData>();
    public Difficulty difficulty;
    public long credits{get;set;}
    public List<string> selectedBoosters = new List<string>();

    void Awake()
    {
        // check if instance exists
        if (instance == null)
        {
            instance = this;
        } else if(instance != this)
        {
            // Then destroy  this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameInstance.
            Destroy(gameObject);
        }

        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);
        PlopSound = GetComponent<AudioSource>();

        InitPlayerPrefs();
    }
    /// <summary>
    /// To get the prefs from the player's device
    /// </summary>
    private void InitPlayerPrefs()
    {
        gameTime = PlayerPrefs.GetInt("GameTimePref", 0);
        vibrationMode = PlayerPrefs.GetInt("GameVibrationPref", 0);
        language = PlayerPrefs.GetString("GameLanguagePref", "NL");
        PlopSound.volume = PlayerPrefs.GetFloat("GameButtonSounds", 1);
    }

    /// <summary>
    /// Get the gameTime in seconds
    /// </summary>
    /// <returns>int</returns>
    public static int GetGameTimeInSeconds()
    {
        switch(gameTime)
        {
            case 0:
                return 120;
            case 1:
                return 150;
            case 2:
                return 180;
            case 3:
                return 210;
            case 4:
                return 240;
            default:
                return 120;
        } 
    }

    /// <summary>
    /// Get the int (1 - 4) for the slider
    /// </summary>
    /// <returns></returns>
    public static int GetGameTimeForSlider()
    {
        return gameTime;
    }

    /// <summary>
    /// Set the game time
    /// </summary>
    /// <param name="value">Gametime</param>
    public static void SetGameTime(int value)
    {
        gameTime = value;
        PlayerPrefs.SetInt("GameTimePref", value);
    }

    /// <summary>
    /// Get the vibration setting
    /// </summary>
    /// <returns>bool</returns>
    public static int GetVibrationMode()
    {
        return vibrationMode;
    }

    /// <summary>
    /// Set the vibrationmode
    /// </summary>
    /// <param name="value"></param>
    public static void SetVibrationMode(int value)
    {
        vibrationMode = value;
        PlayerPrefs.SetInt("GameVibrationPref", value);
    }

    public static string GetLanguage()
    {
        return language;
    }

    public static void SetLanguage(string value)
    {
        language = value;
        PlayerPrefs.SetString("GameLanguagePref", value);
    }

    public static bool GetSoundVolume()
    {
        return PlopSound.volume > 0;
    }

    public static void SetButtonSoundVolume(float value)
    {
        PlopSound.volume = value;
        PlayerPrefs.SetFloat("GameButtonSounds", value);
    }
}
