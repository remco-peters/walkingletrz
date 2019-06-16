using System.Linq;
using Assets.Scripts;
using I2.Loc;
using UnityEngine;

public class AchievementManager : MyMonoBehaviour
{
    private int _wordCount;
    private long _points;
    private int _wordPerMinute;
    private int _wordOfTwelve;
    public MyPlayer Player { get; set; }
    private Achievement TotalPointsAchievement;
    private Achievement WordsPerMinteAchievement;
    private Achievement TwelveLengthWordAchievement;

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

    public void SubmitTwelveCharWordToAchievements(int amount)
    {
        _wordOfTwelve = amount;
    }

    public void SubmitWordCountPerMinuteToAchievements(int amount)
    {
        _wordPerMinute = amount;
    }

    internal string CheckIfAchievementIsGet()
    {
        string achievement = LocalizationManager.GetTranslation("achievement_unlocked");
        string points = LocalizationManager.GetTranslation("achievement_points");
        string wordsPre = LocalizationManager.GetTranslation("achievement_words_prefix");
        string wordsSuf = LocalizationManager.GetTranslation("achievement_words");
        string returnString = "";

        #region oldStuff
        /*if (PlayerPrefs.GetInt("25PointAchievement") == 0 && _points >= 25)
        {
            PlayerPrefs.SetInt("25PointAchievement", 1);
            Player.Credit.AddCredits(10);
            returnString = $"{achievement} 25 {points}";
        }

        if (PlayerPrefs.GetInt("50PointAchievement") == 0 && _points >= 50)
        {
            PlayerPrefs.SetInt("50PointAchievement", 1);
            Player.Credit.AddCredits(25);
            returnString = $"{achievement} 50 {points}";
        }

        if (PlayerPrefs.GetInt("100PointAchievement") == 0 && _points >= 100)
        {
            PlayerPrefs.SetInt("100PointAchievement", 1);
            Player.Credit.AddCredits(100);
            returnString = $"{achievement} 100 {points}";
        }

        if (PlayerPrefs.GetInt("250PointAchievement") == 0 && _points >= 250)
        {
            PlayerPrefs.SetInt("250PointAchievement", 1);
            Player.Credit.AddCredits(250);
            returnString = $"{achievement} 250 {points}";
        }

        if (PlayerPrefs.GetInt("5WordAchievement") == 0 && _wordCount >= 5)
        {
            PlayerPrefs.SetInt("5WordAchievement", 1);
            Player.Credit.AddCredits(10);
            returnString = $"{achievement} {wordsPre} 5 {wordsSuf}";
        }

        if (PlayerPrefs.GetInt("10WordAchievement") == 0 && _wordCount >= 10)
        {
            PlayerPrefs.SetInt("10WordAchievement", 1);
            Player.Credit.AddCredits(25);
            returnString = $"{achievement} {wordsPre} 10 {wordsSuf}";
        }

        if (PlayerPrefs.GetInt("25WordAchievement") == 0 && _wordCount >= 25)
        {
            PlayerPrefs.SetInt("25WordAchievement", 1);
            Player.Credit.AddCredits(50);
            returnString = $"{achievement} {wordsPre} 25 {wordsSuf}";
        }
        //return returnString;
        */
        #endregion oldStuff

        var achievements = AccountManager.instance.listOfAchievements.GroupBy(item => item.Name).OrderBy(item => item.Key);
        // Lijst van achievements per naam gesorteerd, key = Name
        foreach (var achievementGroup in achievements)
        {
            // De laatste achievement ophalen voor gegevens (naam in playfab)
            Achievement lastAchievement = achievementGroup.Last();
            bool doesStatExists = false;
            // Het aantal verdiende punten / gelegde woorden die tot nu toe behaald is ophalen
            int earnedPoints = 0;
            if (AccountManager.CurrentPlayer.Statistics.Find(model => model.Name == lastAchievement.NameInPlayfab) != null)
            {
                earnedPoints = AccountManager.CurrentPlayer.Statistics.Find(model => model.Name == lastAchievement.NameInPlayfab).Value;
                doesStatExists = true;
            }
            
            switch(achievementGroup.Key)
            {
                case "AmountOfWordsPerMin":
                    // Checken of het behaald aantal woorden per minuut hoger is dan de woorden per minuut tot nu toe
                    if (_wordPerMinute > earnedPoints)
                    {
                        // Achievement ophalen waar hij/zij op dit moment mee bezig is
                        Achievement currentAchievement = achievementGroup.FirstOrDefault(x => earnedPoints < x.Amount && x.Name == "AmountOfWordsPerMin");
                        if (currentAchievement == null)
                        {
                            currentAchievement = achievementGroup.Last();
                        }
                        Achievement checkAchievement = achievementGroup.FirstOrDefault(x => _wordPerMinute < x.Amount && x.Name == "AmountOfWordsPerMin");
                        if (checkAchievement.Level > currentAchievement.Level && WordsPerMinteAchievement != checkAchievement)
                        {
                            // Set value in statistics
                            if(doesStatExists)
                            {
                                AccountManager.CurrentPlayer.Statistics.Find(model => model.Name == lastAchievement.NameInPlayfab).Value = _wordPerMinute;
                            } else
                            {
                                Player.AmountOfWordsPerMinuteFinal = _wordPerMinute;
                            }
                            WordsPerMinteAchievement = checkAchievement;
                            Player.Credit.AddCredits(checkAchievement.Credits);
                            returnString = $"{achievement} {LocalizationManager.GetTranslation(achievementGroup.Key)} ({currentAchievement.Credits} {points})";
                        }
                    }
                    break;
                case "PointsInGame":
                    {
                        // Checken of het behaald aantal punten hoger is dan de punten tot nu toe
                        if(_points > earnedPoints)
                        {
                            // Achievement ophalen waar hij/zij op dit moment mee bezig is
                            Achievement currentAchievement = achievementGroup.FirstOrDefault(x => earnedPoints < x.Amount && x.Name == "PointsInGame");
                            if (currentAchievement == null)
                            {
                                currentAchievement = achievementGroup.Last();
                            }
                            Achievement checkAchievement = achievementGroup.FirstOrDefault(x => _points < x.Amount && x.Name == "PointsInGame");
                            if (checkAchievement.Level > currentAchievement.Level)
                            {
                                if(doesStatExists)
                                {
                                    // Set value in statistics
                                    AccountManager.CurrentPlayer.Statistics.Find(model => model.Name == lastAchievement.NameInPlayfab).Value = (int)_points;
                                }
                                Player.Credit.AddCredits(checkAchievement.Credits);
                                returnString = $"{achievement} {LocalizationManager.GetTranslation(achievementGroup.Key)} ({currentAchievement.Credits} {points})";
                            }
                        }
                        break;
                    }
                case "PointsTotal":
                    {
                        // Achievement ophalen waar hij/zij op dit moment mee bezig is
                        Achievement currentAchievement = achievementGroup.FirstOrDefault(x => earnedPoints < x.Amount && x.Name == "PointsTotal");
                        if (currentAchievement == null)
                        {
                            currentAchievement = achievementGroup.Last();
                        }
                        
                        Achievement checkAchievement = achievementGroup.FirstOrDefault(x => earnedPoints + _points < x.Amount && x.Name == "PointsTotal");

                        if (checkAchievement.Level > currentAchievement.Level && TotalPointsAchievement != checkAchievement)
                        {
                            if (doesStatExists)
                            {
                                // Set value in statistics
                                AccountManager.CurrentPlayer.Statistics.Find(model => model.Name == lastAchievement.NameInPlayfab).Value = earnedPoints + (int)_points;
                                TotalPointsAchievement = checkAchievement;
                            }
                            Player.Credit.AddCredits(checkAchievement.Credits);
                            returnString = $"{achievement} {LocalizationManager.GetTranslation(achievementGroup.Key)} ({currentAchievement.Credits} {points})";
                        }
                        break;
                    }
                case "WordLengthOfTwelve":
                    {
                        // Achievement ophalen waar hij/zij op dit moment mee bezig is
                        Achievement currentAchievement = achievementGroup.FirstOrDefault(x => earnedPoints < x.Amount && x.Name == "WordLengthOfTwelve");
                        if (currentAchievement == null)
                        {
                            currentAchievement = achievementGroup.Last();
                        }
                        Achievement checkAchievement = achievementGroup.FirstOrDefault(x => _wordOfTwelve + earnedPoints < x.Amount && x.Name == "WordLengthOfTwelve");
                        if (checkAchievement.Level > currentAchievement.Level && TwelveLengthWordAchievement != checkAchievement)
                        {
                            if(doesStatExists)
                            {
                                // Set value in statistics
                                AccountManager.CurrentPlayer.Statistics.Find(model => model.Name == lastAchievement.NameInPlayfab).Value = _wordOfTwelve;
                            }
                            Debug.Log(Player.WordsWithTwelveLetters + " Achievement");
                            TwelveLengthWordAchievement = checkAchievement;
                            Player.Credit.AddCredits(checkAchievement.Credits);
                            returnString = $"{achievement} {LocalizationManager.GetTranslation(achievementGroup.Key)} ({currentAchievement.Credits} {points})";
                        }
                    }
                    break;
                default:
                    break;
            }
        }
        return returnString;
    }
}
