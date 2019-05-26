using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Events;

namespace Assets.Scripts
{
    public class GameSceneCounter : MyMonoBehaviour
    {
        public Text CounterText;
        public GameObject FixedLetterOverlay;
        public GameObject Counter;
        public UnityAction OnCountDownFinished;
        public void Start()
        {
            StartCoroutine(AnimateFixedLettersScreen());
        }

        private IEnumerator AnimateFixedLettersScreen()
        {
            float rotate = 0;
            int timeleft = 3;
            while (timeleft > 0) //wait 3 seconds
            {
                rotate += 45f;
                StartCoroutine(AnimateBlock(rotate));
                CounterText.text = timeleft.ToString();
                timeleft--;
                yield return new WaitForSeconds(1);
            }
            FixedLetterOverlay.SetActive(false);
            OnCountDownFinished();
        }

        private IEnumerator AnimateBlock(float rotate)
        {        
            float duration = 0.3f;
            Quaternion startRotation = Counter.transform.rotation;
            Quaternion newRotate = Quaternion.Euler(new Vector3(0, 0, rotate));
            for(float t = 0 ; t < duration ; t += Time.deltaTime )
            {
                Counter.transform.rotation = Quaternion.Lerp( startRotation, newRotate, t / duration ) ;
                yield return null;
            }
            Counter.transform.rotation = newRotate;
        }
    }
}
