using I2.Loc;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSettingScript : MonoBehaviour
{
    public Text TimeText;
    public Slider ButtonPushSlider;
    public Slider SoundSlider;
    public Slider VibrateSlider;
    public Slider GameTimeSlider;
    public Text NLText;
    public Text ENText;

    void Awake()
    {
        SetSoundSlider();
        SetButtonPushSlider();
        SetVibrateSlider();
        SetGameTimeSlider();
        SetLanguageText();
    }

    private void SetSoundSlider()
    {
        // Todo, implement sounds
    }

    private void SetButtonPushSlider()
    {
        ButtonPushSlider.value = GameInstance.PlopSound.volume;
        GameInstance.PlopSound.Play();
    }

    private void SetVibrateSlider()
    {
        VibrateSlider.value = GameInstance.GetVibrationMode();
    }

    private void SetGameTimeSlider()
    {
        GameTimeSlider.value = GameInstance.GetGameTimeForSlider();
    }

    public void SetGameTime(float value)
    {
        switch (value)
        {
            case 0:
                // 2 mins
                TimeText.text = "2 min.";
                break;
            case 1:
                // 2.5 mins
                TimeText.text = "2,5 min.";
                break;
            case 2:
                // 3 mins
                TimeText.text = "3 min.";
                break;
            case 3:
                // 3.5 mins
                TimeText.text = "3,5 min.";
                break;
            case 4:
                // 4 mins
                TimeText.text = "4 min.";
                break;
            default:
                // 2 mins
                TimeText.text = "2 min.";
                break;
        }
        GameInstance.SetGameTime((int)value);
    }

    public void SetButtonVolume(float value)
    {
        GameInstance.SetButtonSoundVolume(value);
    }

    public void SetGameVibration(float value)
    {
        GameInstance.SetVibrationMode((int)value);
    }

    public void SetLanguage()
    {
        if (GameInstance.GetLanguage() == "NL")
        {
            GameInstance.SetLanguage("EN");
            LocalizationManager.CurrentLanguage = "English";
        }
        else
        {
            GameInstance.SetLanguage("NL");
            LocalizationManager.CurrentLanguage = "Netherlands";
        }
        SetLanguageText();
    }

    public void SetLanguageText()
    {
        if (GameInstance.GetLanguage() == "NL")
        {
            NLText.gameObject.SetActive(true);
            ENText.gameObject.SetActive(false);
        }
        else
        {
            ENText.gameObject.SetActive(true);
            NLText.gameObject.SetActive(false);
        }
    }
}
