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

        TimeRemainingText.text = "TIJD IS OP!!!!!";
    }

    // Update is called once per frame
    void Update()
    {
        PointText.text = $"Points: {Player.EarnedPoints}";
    }

    private string TimeText(float seconds)
    {
        TimeSpan t = TimeSpan.FromSeconds(seconds);
        return t.ToString(@"mm\:ss");
    }
}
