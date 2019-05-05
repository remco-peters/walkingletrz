using I2.Loc;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AchievementOverviewScript : MonoBehaviour
{
    public GameObject ContentHolder;
    public AchievementItem AchievementItem;
    private int amountOfWords;
    private int amountOfPoints;

    void Start()
    {
        /*
        if(AccountManager.CurrentPlayer.Statistics.Find(model => model.Name == "WordCount") != null)
        {
            amountOfWords = AccountManager.CurrentPlayer.Statistics.Find(model => model.Name == "WordCount").Value;
        } else
        {
            amountOfWords = 0;
        }

        AchievementItem newItem = Instantiate(AchievementItem);
        newItem.SetAchievementTitle(LocalizationManager.GetTranslation("achievements_words"));
        
        if (PlayerPrefs.GetInt("5WordAchievement", 0) == 0)
        {
            newItem.SetStartPointTxt(amountOfWords.ToString());
            newItem.SetEndPointTxt("5");
            newItem.SetCreditsTxt(10);
            newItem.SetPercentageTxt(System.Math.Round((decimal)amountOfWords / 5, 2));
        } else if (PlayerPrefs.GetInt("10WordAchievement", 0) == 0)
        {
            newItem.SetStartPointTxt(amountOfWords.ToString());
            newItem.SetEndPointTxt("10");
            newItem.SetCreditsTxt(25);
            newItem.SetPercentageTxt(System.Math.Round((decimal)amountOfWords / 10, 2));
        } else if (PlayerPrefs.GetInt("25WordAchievement", 0) == 0)
        {
            newItem.SetStartPointTxt(amountOfWords.ToString());
            newItem.SetEndPointTxt("25");
            newItem.SetCreditsTxt(50);
            newItem.SetPercentageTxt(System.Math.Round((decimal)amountOfWords / 25, 2));
        } else
        {
            newItem.SetStartPointTxt(amountOfWords.ToString());
            newItem.SetEndPointTxt("25");
            newItem.SetCreditsTxt(0);
            newItem.SetPercentageTxt(100);
        }

        newItem.transform.SetParent(ContentHolder.transform, false);

        if (AccountManager.CurrentPlayer.Statistics.Find(model => model.Name == "TotalScore") != null)
        {
            amountOfPoints = AccountManager.CurrentPlayer.Statistics.Find(model => model.Name == "TotalScore").Value;
        }
        else
        {
            amountOfPoints = 0;
        }

        AchievementItem newItem2 = Instantiate(AchievementItem);
        newItem2.SetAchievementTitle(LocalizationManager.GetTranslation("achievements_points"));

        if (PlayerPrefs.GetInt("25PointAchievement", 0) == 0)
        {
            newItem2.SetStartPointTxt(amountOfPoints.ToString());
            newItem2.SetEndPointTxt("25");
            newItem2.SetCreditsTxt(25);
            newItem2.SetPercentageTxt(System.Math.Round((decimal)amountOfPoints / 25, 2));
        }
        else if (PlayerPrefs.GetInt("50PointAchievement", 0) == 0)
        {
            newItem2.SetStartPointTxt(amountOfPoints.ToString());
            newItem2.SetEndPointTxt("50");
            newItem2.SetCreditsTxt(50);
            newItem2.SetPercentageTxt(System.Math.Round((decimal)amountOfPoints / 50, 2));
        }
        else if (PlayerPrefs.GetInt("100PointAchievement", 0) == 0)
        {
            newItem2.SetStartPointTxt(amountOfPoints.ToString());
            newItem2.SetEndPointTxt("100");
            newItem2.SetCreditsTxt(100);
            newItem2.SetPercentageTxt(System.Math.Round((decimal)amountOfPoints / 100, 2));
        }
        else if (PlayerPrefs.GetInt("250PointAchievement", 0) == 0)
        {
            newItem2.SetStartPointTxt(amountOfPoints.ToString());
            newItem2.SetEndPointTxt("250");
            newItem2.SetCreditsTxt(250);
            newItem2.SetPercentageTxt(System.Math.Round((decimal)amountOfPoints / 250, 2));
        }
        else
        {
            newItem2.SetStartPointTxt(amountOfPoints.ToString());
            newItem2.SetEndPointTxt("250");
            newItem2.SetCreditsTxt(0);
            newItem2.SetPercentageTxt(100);
        }
        newItem2.transform.SetParent(ContentHolder.transform, false);
        */

        var achievements = AccountManager.instance.listOfAchievements.GroupBy(item => item.Name).OrderBy(item => item.Key);

        // Lijst van achievements per naam gesorteerd, key = Name
        foreach(var achievement in achievements)
        {
            // Nieuwe achievement aanmaken, met de naam (Key) en het level welke behaald is
            AchievementItem item = Instantiate(AchievementItem);

            // De laatste achievement ophalen voor gegevens (naam in playfab) en het eventueel maximaliseren van balk wanneer laatste lvl is behaald
            Achievement lastAchievement = achievement.Last();

            // Het aantal verdiende punten die tot nu toe behaald is ophalen
            int earnedPoints = 0;
            if (AccountManager.CurrentPlayer.Statistics.Find(model => model.Name == lastAchievement.NameInPlayfab) != null)
            {
                earnedPoints = AccountManager.CurrentPlayer.Statistics.Find(model => model.Name == lastAchievement.NameInPlayfab).Value;
            }

            // Juiste achievement ophalen
            Achievement at = achievement.FirstOrDefault(x => earnedPoints < x.Amount);
            if (at == null)
            {
                at = achievement.Last();
            }

            item.SetAchievementTitle(LocalizationManager.GetTranslation(achievement.Key) + " (level " + at.Level + " van " + lastAchievement.Level + ")");
            item.SetStartPointTxt(earnedPoints.ToString());
            item.SetEndPointTxt(at.Amount.ToString());
            decimal percentage = System.Math.Round((decimal)earnedPoints / at.Amount, 2);

            if(percentage > 1)
            {
                item.SetCreditsTxt(0);
                item.SetPercentageTxt(1);
            } else
            {
                item.SetCreditsTxt(at.Credits);
                item.SetPercentageTxt(percentage);
            }
            
            item.transform.SetParent(ContentHolder.transform, false);
        }
    }
}
