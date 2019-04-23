using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardEntry : MonoBehaviour
{
    public Text playerName;
    public Text score;
    public Text place;
    public Image crown;
    public Sprite goldCrown;
    public Sprite silverCrown;
    public Sprite bronzeCrown;


    public void SetName(string name)
    {
        this.playerName.text = name;
    }

    public void SetPlace(int place)
    {
        place++;
        this.place.text = place.ToString();
    }

    public void SetImage(int place)
    {
        switch(place)
        {
            case 0:
                crown.sprite = goldCrown;
                break;
            case 1:
                crown.sprite = silverCrown;
                break;
            case 2:
                crown.sprite = bronzeCrown;
                break;
            default:
                crown.sprite = null;
                var tempColor = crown.color;
                tempColor.a = 0f;
                crown.color = tempColor;
                break;
        }

    }

    public void SetScore(int score)
    {
        this.score.text = score.ToString();
    }
}
