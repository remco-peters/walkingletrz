using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LetterBlock : MyMonoBehaviour, IPunObservable
{
    public event UnityAction<LetterBlock> OnLetterTouched;
    public event UnityAction<LetterBlock> OnLetterDragged;
    public bool IsFirstLetter { get; set; } = false;
    public bool IsSecondLetter { get; set; } = false;

    public void ButtonClicked()
    {
        if (GameInstance.GetSoundVolume())
        {
            GameInstance.PlopSound.Play();
        }

        # if UNITY_ANDROID || UNITY_IOS
            if(GameInstance.GetVibrationMode() == 1)
            {
                Handheld.Vibrate();
        
            }
        #endif

        OnLetterTouched(this);
    }
    /// <summary>
    /// Get if it's a walking/orange letter
    /// </summary>
    /// <returns>bool</returns>
    internal bool IsWalkingLetter()
    {
        return IsFirstLetter || IsSecondLetter;
    }

    /// <summary>
    /// Gets the letter of the current block
    /// </summary>
    /// <returns>char</returns>
    internal char GetLetter()
    {
        return GetComponentInChildren<Text>().text[0];
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        
    }
}
