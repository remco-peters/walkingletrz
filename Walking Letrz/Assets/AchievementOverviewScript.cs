using I2.Loc;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementOverviewScript : MonoBehaviour
{
    public GameObject ContentHolder;
    public AchievementItem AchievementItem;
    private int amountOfWords;
    private int amountOfPoints;

    void Start()
    {
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
    }
}
