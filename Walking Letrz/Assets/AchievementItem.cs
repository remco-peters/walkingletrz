using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AchievementItem : MonoBehaviour
{
    public Text startPoint;
    public Text endPoint;
    public Text achievementTitle;
    public Text percentage;
    public Text credits;
    public Image achievementBar;
    private decimal percentageAmount;

    public void SetStartPointTxt(string value)
    {
        startPoint.text = value;
    }

    public void SetEndPointTxt(string value)
    {
        endPoint.text = value;
    }

    public void SetAchievementTitle(string value)
    {
        achievementTitle.text = value;
    }

    public void SetPercentageTxt(decimal value)
    {
        percentage.text = $"{System.Math.Round(value*100, 0)}%";
        percentageAmount = value;
        SetAchievementBar(value);
    }

    public void SetCreditsTxt(int amount)
    {
        if(amount == 0)
        {
            credits.transform.parent.gameObject.SetActive(false);
        } else
        {
            credits.text = $"+{amount} credits";
        }
    }

    private void SetAchievementBar(decimal value)
    {
        achievementBar.fillAmount = (float)value;
    }
}
