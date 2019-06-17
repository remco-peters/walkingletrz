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
    /// <summary>
    /// To set the text of the points currently earned
    /// </summary>
    /// <param name="value"></param>
    public void SetStartPointTxt(string value)
    {
        startPoint.text = value;
    }

    /// <summary>
    /// To set the text of the points you have to get
    /// </summary>
    /// <param name="value"></param>
    public void SetEndPointTxt(string value)
    {
        endPoint.text = value;
    }

    /// <summary>
    /// Set the title of the specific achievement
    /// </summary>
    /// <param name="value"></param>
    public void SetAchievementTitle(string value)
    {
        achievementTitle.text = value;
    }

    /// <summary>
    /// To set the percentage of the achievement that's already earned
    /// </summary>
    /// <param name="value"></param>
    public void SetPercentageTxt(decimal value)
    {
        percentage.text = $"{System.Math.Round(value*100, 0)}%";
        percentageAmount = value;
        SetAchievementBar(value);
    }
    /// <summary>
    /// To set the credits that can be earned; unless the last level is already earned; than credits won't be shown
    /// </summary>
    /// <param name="amount"></param>
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
    
    /// <summary>
    /// To fill the bar with the earned percentage of this achievement
    /// </summary>
    /// <param name="value"></param>
    private void SetAchievementBar(decimal value)
    {
        achievementBar.fillAmount = (float)value;
    }
}
