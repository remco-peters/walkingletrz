using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Assets.Scripts
{
    class GameSceneCounter : MyMonoBehaviour
    {
        public Text CounterText;
        public GameObject FixedLetterOverlay;
        public void Start()
        {
            StartCoroutine(AnimateFixedLettersScreen(Time.fixedTime));
        }

        private IEnumerator AnimateFixedLettersScreen(float timeStart)
        {
            while (Time.fixedTime - timeStart < 3f) //wait 3 seconds
            {
                CounterText.text = (3 - (int)(Time.fixedTime - timeStart)).ToString();
                yield return new WaitForSeconds(1f);
            }
            FixedLetterOverlay.SetActive(false);
        }
    }
}
