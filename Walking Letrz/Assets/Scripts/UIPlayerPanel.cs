using System;
using System.Collections;
using I2.Loc;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIPlayerPanel : UIBehaviour
{
    public Text TimeRemainingText;
    public Text PointText;
    public Text InfoText;

    public MyPlayer Player
    {
        get
        {
            HUD Hud = GetComponentInParent<HUD>();
            Assert.IsNotNull(Hud, "Hud niet ingesteld in UIPayerPanel");
            Assert.IsNotNull(Hud.Player, "Geen player in hud");

            return Hud.Player;
        }
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        StartCoroutine(Timer());
    }

    IEnumerator Timer()
    {
        
        while (Player.TimeRemaining >= 0)
        {
            TimeRemainingText.text = TimeText(Player.TimeRemaining);

            yield return new WaitForEndOfFrame();
        }

        string timeUp = LocalizationManager.GetTranslation("time_up");
        string playAgain = LocalizationManager.GetTranslation("play_again");
        string pointsPre = LocalizationManager.GetTranslation("points_earned");
        string pointsSuf = LocalizationManager.GetTranslation("points_earned_suffix");
        TimeRemainingText.text = timeUp;
        InfoText.text = $"{timeUp} {playAgain} {pointsPre} {Player.EarnedPoints} {pointsSuf}";
        
        InfoText.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        string pointString = I2.Loc.LocalizationManager.GetTranslation("points");
        PointText.text = $"{pointString} {Player.EarnedPoints}";

        if(Player.CoolDownTime >= 0 && Player.CoolDownTime < 10)
        {
            InfoText.enabled = true;
            InfoText.text = "Can't move yet: " + TimeText(Player.CoolDownTime) + " seconds remaining";
        } 
        else if(Player.InfoText.Length > 0)
        {
            InfoText.enabled = true;
            InfoText.text = Player.InfoText;
        }else
        {
            InfoText.enabled = false;
        }
    }

    private string TimeText(float seconds)
    {
        TimeSpan t = TimeSpan.FromSeconds(seconds);
        return t.ToString(@"mm\:ss");
    }
}
