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

            item.SetAchievementTitle($"{LocalizationManager.GetTranslation(achievement.Key)} ({LocalizationManager.GetTranslation("achievement_level")} {at.Level} {LocalizationManager.GetTranslation("achievement_of")} {lastAchievement.Level})");
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
