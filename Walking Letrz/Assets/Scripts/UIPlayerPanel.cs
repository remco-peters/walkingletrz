using System;
using System.Collections;
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

        TimeRemainingText.text = "Time's up";
        InfoText.text = $"Time's up. Play again! You've got a total of {Player.EarnedPoints} points!";
        InfoText.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        PointText.text = $"Points: {Player.EarnedPoints}";

        if(Player.CoolDownTime >= 0 && Player.CoolDownTime < 10)
        {
            InfoText.enabled = true;
            InfoText.text = Player.MustThrowLetterAway ? 
                "Choose a Letter to get rid of" : 
                "Can't move yet: " + TimeText(Player.CoolDownTime) + " seconds remaining";
        } 
        else if (Player.MustThrowLetterAway)
        {
            InfoText.enabled = true;
            InfoText.text = "Choose a Letter to get rid of";
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
