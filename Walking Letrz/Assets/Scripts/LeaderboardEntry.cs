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
    public Sprite firstPlace;
    public Sprite secondPlace;
    public Sprite thirdPlace;


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
                crown.sprite = firstPlace;
                break;
            case 1:
                crown.sprite = secondPlace;
                break;
            case 2:
                crown.sprite = thirdPlace;
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
