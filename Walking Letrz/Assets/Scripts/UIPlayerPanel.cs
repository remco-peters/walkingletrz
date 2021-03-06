﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;
using ExitGames.Client.Photon;
using I2.Loc;
using Photon.Pun;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class UIPlayerPanel : UIBehaviour
{
    public Text TimeRemainingText;
    public Text PointText;
    public Text PlayerNameTxt;
    public Image CrownImage;

    public Text OpponentNameTxt;
    public Text OpponentScoreTxt;
    public Text OpponentTimeTxt;

    public Material TopBoardMaterial;
    public Material TurnMaterial;
    public GameObject PlayerBackground;
    public GameObject OthersBackground;

    public Sprite thirdPlace;
    public Sprite secondPlace;
    public Sprite firstPlace;

    public GameObject InfoTextPanel;
    public Text InfoPanelText;
    private Image InfoPanelImage;

    public UnityAction OnTutorialFinished;

    public Text CreditsTxt;

    #region tutorialImages
    public GameObject AchievementInfoPanel;
    public GameObject BoosterBtnInfoPanel;
    public GameObject DeleteBtnInfoPanel;
    public GameObject FixedLetterInfoPanel;
    public GameObject OpponentInfoPanel;
    public GameObject PlacedWordInfoPanel;
    public GameObject PlayerBoardInfoPanel;
    public GameObject PlayerLettersInfoPanel;
    public GameObject SendBtnInfoPanel;
    public GameObject SwapBtnInfoPanel;
    public GameObject TimeInfoPanel;
    public GameObject TurnInfoPanel;
    public GameObject UserInfoPanel;
    public GameObject WritingAreaInfoPanel;

    private static List<GameObject> TutorialScreens = new List<GameObject>();

    public Button skipTutorialBtnClass;
    private Button skipTutorialBtn;

    #endregion
    
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

    public List<Player> Players
    {
        get
        {
            HUD Hud = GetComponentInParent<HUD>();
            Assert.IsNotNull(Hud, "Hud niet ingesteld in UIPayerPanel");
            Assert.IsNotNull(Hud.PlayersList, "Geen playerslist in hud");

            return Hud.PlayersList;
        }
    }


    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        
        InfoPanelImage = InfoTextPanel.GetComponent<Image>();
        InfoPanelImage.color = new Color(1f, 1f, 1f, 0f);
        InfoPanelText.color = new Color(1f, 1f, 1f, 0f);

        Player.OnInfoTextChange += ShowInfoText;

        if (AccountManager.CurrentPlayer != null && AccountManager.CurrentPlayer.DisplayName.Length > 0)
            PlayerNameTxt.text = AccountManager.CurrentPlayer.DisplayName;
        
        TimeRemainingText.text = TimeText(Player.TimeRemaining);
        InitOtherPlayers();

        CreditsTxt.text = GameInstance.instance.credits.ToString();

        if (Player.IsInTutorial)
        {
            Player.CanMove = false;
            ShowTutorial();
            skipTutorialBtn = Instantiate(skipTutorialBtnClass, transform, false);
            skipTutorialBtn.GetComponent<TutSkipBtn>().OnSkipTutorialBtnTouched += UiPlayerPanelOnSkipTutorialBtnTouched;
        }

        StartCoroutine(Timer());
        StartCoroutine(CheckIfAllPlayersHaveTimeLeft());
    }

    #region Tutorial
    private void UiPlayerPanelOnSkipTutorialBtnTouched()
    {
        Destroy(skipTutorialBtn.gameObject);
        foreach (GameObject obj in TutorialScreens)
        {
            Destroy(obj);
        }
        Player.CanMove = true;
        Player.IsInTutorial = false;
    }

    private void ShowTutorial()
    {
        TutorialScreens.Add(Instantiate(TurnInfoPanel, transform, false));
        TutorialScreens.Add(Instantiate(UserInfoPanel, transform, false));
        TutorialScreens.Add(Instantiate(TimeInfoPanel, transform, false));
        TutorialScreens.Add(Instantiate(OpponentInfoPanel, transform, false));
        TutorialScreens.Add(Instantiate(PlayerBoardInfoPanel, transform, false));
        TutorialScreens.Add(Instantiate(FixedLetterInfoPanel, transform, false));
        TutorialScreens.Add(Instantiate(PlayerLettersInfoPanel, transform, false));
        TutorialScreens.Add(Instantiate(SendBtnInfoPanel, transform, false));
        TutorialScreens.Add(Instantiate(DeleteBtnInfoPanel, transform, false));
        TutorialScreens.Add(Instantiate(BoosterBtnInfoPanel, transform, false));
        TutorialScreens.Add(Instantiate(SwapBtnInfoPanel, transform, false));
        TutorialScreens.Add(Instantiate(WritingAreaInfoPanel, transform, false));
        TutorialScreens.Add(Instantiate(PlacedWordInfoPanel, transform, false));
        TutorialScreens.Add(Instantiate(AchievementInfoPanel, transform, false));

        foreach(GameObject obj in TutorialScreens)
        {
            obj.transform.SetParent(transform, false);
            if(!TutorialScreens[0].Equals(obj))
            {
                obj.SetActive(false);
            }
        }
    }

    public void ShowNextStep(int index)
    {
        TutorialScreens[index].SetActive(false);
        TutorialScreens[index + 1].SetActive(true);
    }

    public void ShowPreviousStep(int index)
    {
        TutorialScreens[index].SetActive(false);
        TutorialScreens[index - 1].SetActive(true);
    }

    public void PlayGame()
    {
        foreach (GameObject obj in TutorialScreens)
        {
            Destroy(obj);
        }

        OnTutorialFinished();
    }
    #endregion

    // Update is called once per frame
    void Update()
    {
        string pointString = LocalizationManager.GetTranslation("points");
        PointText.text = $"{Player.EarnedPoints}";
        CreditsTxt.text = GameInstance.instance.credits.ToString();

        int index = 0;
        if (GameInstance.instance.IsMultiplayer)
        {
            foreach (Photon.Realtime.Player p in PhotonManager.PhotonInstance.GetOtherPlayersList())
            {
                long points = (long)p.CustomProperties["Points"];
                SetOpponentPoints(index, points);
                if (Player.EarnedPoints > points || Player.EarnedPoints == points)
                {
                    CrownImage.sprite = firstPlace;
                }
                else
                {
                    CrownImage.sprite = secondPlace;
                }
                index++;
            }
        }
        else
        {
            foreach (Player p in Players)
            {
                if (p != Player)
                {
                    SetOpponentPoints(index, p.EarnedPoints);
                    // change this later on
                    if(Player.EarnedPoints > p.EarnedPoints || Player.EarnedPoints == p.EarnedPoints)
                    {
                        CrownImage.sprite = firstPlace;
                    } else
                    {
                        CrownImage.sprite = secondPlace;
                    }
                    index++;
                }
            }
        }
    }

    /// <summary>
    /// Method to set the info of the other players (bot when singleplayer and others in MP)
    /// </summary>
    void InitOtherPlayers()
    {
        if (GameInstance.instance.IsMultiplayer)
        {
            OpponentNameTxt.text = "";
            OpponentScoreTxt.text = "";

            int index = 0;
            foreach(Photon.Realtime.Player p in PhotonManager.PhotonInstance.GetOtherPlayersList())
            {
                Hashtable hash = new Hashtable {{"Points", (long) 0}, {"TimeRemaining", Player.TimeRemaining}};
                p.SetCustomProperties(hash);
                OpponentNameTxt.text = p.NickName;
                OpponentScoreTxt.text = $"{p.CustomProperties["Points"]}";
                StartCoroutine(SetOpponentTime(index, null, p));
                index++;
            }
        }
        else
        {
            int index = 0;
            foreach (Player p in Players)
            {
                if (p != Player)
                {
                    SetOpponentText(index, p);
                    StartCoroutine(SetOpponentTime(index, p));
                    index++;
                }
            }
        }

        
    }

    /// <summary>
    /// IEnumerator to blink the timer when you're waiting, when less than 11 seconds, timer will be red
    /// </summary>
    /// <returns></returns>
    IEnumerator Timer()
    {
        while (Player.TimeRemaining >= 0)
        {
            // Make the text blinking when waiting for your turn
            if (Player.CanMove == false && !Player.IsInTutorial)
            {
                TimeRemainingText.text = "";
                yield return new WaitForSeconds(0.75f);
                TimeRemainingText.text = TimeText(Player.TimeRemaining);
                yield return new WaitForSeconds(0.75f);
            }

            if (Player.TimeRemaining <= 11)
            {
                TimeRemainingText.color = new Color(1f, 0f, 0f);
            }
            TimeRemainingText.text = TimeText(Player.TimeRemaining);
            
            yield return new WaitForEndOfFrame();
        } 
    }

    /// <summary>
    /// IEnumerator to check if all the players still have time left
    /// </summary>
    /// <returns></returns>
    IEnumerator CheckIfAllPlayersHaveTimeLeft()
    {
        if (GameInstance.instance.IsMultiplayer)
        {
            

            while (PhotonManager.PhotonInstance.GetAllPlayersList()
                .FirstOrDefault(player => (float)player.CustomProperties["TimeRemaining"] <= 0) == null)
            {
                yield return new WaitForFixedUpdate();
            }

            string timeUp = LocalizationManager.GetTranslation("time_up");
            string playAgain = LocalizationManager.GetTranslation("play_again");
            string pointsPre = LocalizationManager.GetTranslation("points_earned");
            string pointsSuf = LocalizationManager.GetTranslation("points_earned_suffix");

            //TimeRemainingText.text = timeUp;
            ShowInfoText($"{timeUp} {playAgain} {pointsPre} {Player.EarnedPoints} {pointsSuf}", 5);
            
            WrapUpGame();
            StopCoroutine(Timer());
            StopCoroutine(CheckIfAllPlayersHaveTimeLeft());

            PutAllDataInPlayerData();
            SceneSwitcher.SwitchSceneStatic("MatchResultScene");
        }
        else
        {
            while (Players.FirstOrDefault(player => player.TimeRemaining <= 0) == null)
            {
                yield return new WaitForFixedUpdate();
            }

            string timeUp = LocalizationManager.GetTranslation("time_up");
            string playAgain = LocalizationManager.GetTranslation("play_again");
            string pointsPre = LocalizationManager.GetTranslation("points_earned");
            string pointsSuf = LocalizationManager.GetTranslation("points_earned_suffix");
            //TimeRemainingText.text = timeUp;
            ShowInfoText($"{timeUp} {playAgain} {pointsPre} {Player.EarnedPoints} {pointsSuf}", 5);
            WrapUpGame();
            StopCoroutine(Timer());
            StopCoroutine(CheckIfAllPlayersHaveTimeLeft());
            PutAllDataInPlayerData();
            SceneSwitcher.SwitchSceneStatic("MatchResultScene");
        }
    }
    
    private string TimeText(float seconds)
    {
        TimeSpan t = TimeSpan.FromSeconds(seconds);
        return t.ToString(@"mm\:ss");
    }

    private string OpponentTimeText(float seconds)
    {
        TimeSpan t = TimeSpan.FromSeconds(seconds);
        return t.ToString(@"m\:ss");
    }

    /// <summary>
    /// IEnumerator that sets the backgroundcolor of the players
    /// </summary>
    /// <returns></returns>
    public IEnumerator SetBackgroundPlayerColor()
    {
        while (true)
        {
            if (GameInstance.instance.IsMultiplayer)
            {
                Player.CanMove = (bool)PhotonNetwork.LocalPlayer.CustomProperties["CanMove"];
            }

            if(Player.CanMove)
            {
                PlayerBackground.GetComponent<Image>().material = TurnMaterial;
                OthersBackground.GetComponent<Image>().material = TopBoardMaterial;
            } else
            {
                PlayerBackground.GetComponent<Image>().material = TopBoardMaterial;
                OthersBackground.GetComponent<Image>().material = TurnMaterial;
            }
            yield return new WaitForFixedUpdate();
        }
    }
    
    /// <summary>
    /// IEnumerator to set the time of the opponent in your screen
    /// </summary>
    /// <param name="which"></param>
    /// <param name="localPlayer"></param>
    /// <param name="photonPlayer"></param>
    /// <returns></returns>
    private IEnumerator SetOpponentTime(int which, Player localPlayer, Photon.Realtime.Player photonPlayer = null)
    {
        if (GameInstance.instance.IsMultiplayer)
        {
            while ((float)photonPlayer.CustomProperties["TimeRemaining"] > 0)
            {
                float timeRemaining = (float)photonPlayer.CustomProperties["TimeRemaining"];
                SetText(which, timeRemaining);
                yield return new WaitForEndOfFrame();
            }
        }
        else
        {
            while (localPlayer.TimeRemaining > 0)
            {
                float timeRemaining = localPlayer.TimeRemaining;
                SetText(which, timeRemaining);
                yield return new WaitForEndOfFrame();
            }
        }
    }

    private void SetText(int which, float timeRemaining)
    {
        switch (which)
        {
            case 0:
                OpponentTimeTxt.text = OpponentTimeText(timeRemaining);
                break;
            default:
                break;
        }
    }
    
    public void SetOpponentText(int which, Player p)
    {

        OpponentNameTxt.text = "";
        OpponentScoreTxt.text = "";

        switch (which)
        {
            case 0:
                OpponentNameTxt.text = p.Name;
                OpponentScoreTxt.text = $"{p.EarnedPoints}";
                break;
            default:
                break;
        }
    }

    public void SetOpponentPoints(int which, long points)
    {
        
        switch (which)
        {
            case 0:
                OpponentScoreTxt.text = $"{points}";
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Method to wrap up the game if it's finisched. After this, it will go to the MatchResultScreen
    /// </summary>
    private void WrapUpGame()
    {
        int creditsToGive = 0;
        if (GameInstance.instance.IsMultiplayer)
        {
            var playersList = PhotonManager.PhotonInstance.GetOtherPlayersList();
            foreach(var pl in playersList)
            {
                Player p = new Player
                {
                    EarnedPoints = (long)pl.CustomProperties["Points"],
                    Name = pl.NickName,
                    TimeRemaining = (float)pl.CustomProperties["TimeRemaining"],
                    playfabId = pl.UserId
                };

                string w1 = (string)pl.CustomProperties["BestWords1"];
                string w2 = (string)pl.CustomProperties["BestWords2"];
                string w3 = (string)pl.CustomProperties["BestWords3"];

                List<Word> woorden = new List<Word>
                {
                    new Word(word: w1, points: 0),
                    new Word(word: w2, points: 0),
                    new Word(word: w3, points: 0)
                };

                p.BestWordsThisGame = woorden;
                Players.Add(p);
            }

            Player.Name = Player.name;
        }

        Players.Sort((p1, p2) => (p2.EarnedPoints + (p2.TimeRemaining / (int)GameInstance.instance.difficulty))
        .CompareTo(p1.EarnedPoints + (p1.TimeRemaining /  (int)GameInstance.instance.difficulty)));
        
        var indexOfPlayer = Players.IndexOf(Player);
        if (Players[0].EarnedPoints == Players[1].EarnedPoints)
            creditsToGive = 25;
        else if (indexOfPlayer == 0)
            creditsToGive = 50;
        else
            creditsToGive = 5;
        Debug.Log($"Credits to give: {creditsToGive}");
        Player.Credit.AddCredits(creditsToGive);
    }
    
    /// <summary>
    /// Method to put all the usefull data from the game in a PlayerData Object
    /// </summary>
    private void PutAllDataInPlayerData()
    {
        GameInstance.instance.PlayerData = new List<PlayerData>();
        for (int i = 0; i < Players.Count; i++)
        {
            Player p = Players[i];
            
            PlayerData pd = new PlayerData
            {
                Name = p.Name,
                Points = p.EarnedPoints + (int)Math.Ceiling(p.TimeRemaining / (int)GameInstance.instance.difficulty),
                PointsWithoutTime = p.EarnedPoints,
                place = i + 1,
                timeLeft = p.TimeRemaining,
                BestWords = p.BestWordsThisGame.Select(w => w.word).ToList(),
                WordCountTwelveLetters = p.WordsWithTwelveLetters,
                FinalWordCountPerMinute = p.AmountOfWordsPerMinuteFinal,
                playfabId = p.playfabId
            };
            if (p == Player)
            {
                var myPlayer = (MyPlayer)p;
                // Make sure you're thing is placed first
                pd.localPlayer = true;
                pd.WordCount = myPlayer.GetPlacedWordCount();
                GameInstance.instance.PlayerData.Insert(0, pd);
            }
            else
            {
                GameInstance.instance.PlayerData.Add(pd);
            }
        }
    }


    private void ShowInfoText(string text, int time)
    {
        InfoPanelText.text = text;
        StartCoroutine(ShowInfoTextTimer(InfoPanelImage, InfoPanelText, time));
    }

    IEnumerator ShowInfoTextTimer(Image imageObj, Text txtObj, float time)
    {
        StartCoroutine(FadeTo(1f, 0.5f, imageObj, txtObj));
        yield return new WaitForSeconds(time);
        StartCoroutine(FadeTo(0f, 0.5f, imageObj, txtObj));
        StopCoroutine(ShowInfoTextTimer(imageObj, txtObj, time));
    }

    IEnumerator FadeTo(float aValue, float aTime, Image imageObj, Text txtObj)
    {
        float alpha = imageObj.color.a;
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
        {
            Color panelColor = new Color(1, 1, 1, Mathf.Lerp(alpha, aValue, t));
            Color textColor = new Color(0, 0, 0, Mathf.Lerp(alpha, aValue, t));
            imageObj.color = panelColor;
            txtObj.color = textColor;
            yield return null;
        }
        if(aValue == 0f)
        {
            imageObj.color = new Color(1, 1, 1, 0f);
            txtObj.color = new Color(0, 0, 0, 0f);
        }
    }
}
