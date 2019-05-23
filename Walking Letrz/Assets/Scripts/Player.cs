﻿using System.Collections;
using System.Collections.Generic;
 using Photon.Pun;
 using UnityEngine;
 using Hashtable = ExitGames.Client.Photon.Hashtable;

 namespace Assets.Scripts
{
    public class Player : MyMonoBehaviour
    {
        public float TimeRemaining { get; set; }
        public long EarnedPoints { get; set; }
        public bool CanMove { get; set; }
        public LetterManager LetterManager { get; set; }
        public string Name { get; set; }
        public TheLetterManager TheLetterManager { get; set; }
        public List<Word> BestWordsThisGame{get; set;} = new List<Word>();
        public int WordsWithTwelveLetters { get; set; }
        private int _amountOfWordsPerMinute;
        private float StartTime;
        public int AmountOfWordsPerMinute { 
            get {
                return _amountOfWordsPerMinute;
            }
            set
            {
                if (value == 1)
                {
                    Seconds = 60;
                    StartCoroutine(TimerForSeconds());
                }
                _amountOfWordsPerMinute = value;
            }
        }
        public int AmountOfWordsPerMinuteFinal { get; set; }
        public int PlayerIndex { get; set; }
        private float Seconds = 0;

        public static bool joinedRoom = false;

        public void Awake()
        {
            StartTime = Time.realtimeSinceStartup;
            TimeRemaining = GameInstance.GetGameTimeInSeconds();
            EarnedPoints = 0;
        }

        public void Start()
        {
            StartCoroutine(TimerForSeconds());
        }

        public void Update()
        {
            CanMove = (bool) PhotonNetwork.LocalPlayer.CustomProperties["CanMove"];
            if (!joinedRoom) return;
            if (!CanMove) return;
//            TimeRemaining -= Time.deltaTime;
            TimeRemaining = Time.realtimeSinceStartup - StartTime;
            Hashtable hash = new Hashtable{{"TimeRemaining", TimeRemaining}};
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
            if (TimeRemaining <= 0)
            {
                //CanMove = false;
            }
        }

        IEnumerator TimerForSeconds()
        {
            while(Seconds >= 0)
            {
                CanMove = (bool) PhotonNetwork.LocalPlayer.CustomProperties["CanMove"];
                if (CanMove)
                {
                    Seconds--;
                    yield return new WaitForSeconds(1.0f);
                }
                yield return new WaitForEndOfFrame();
            }
            Seconds = 0;
            AmountOfWordsPerMinute = 0;
        }
    }
}
