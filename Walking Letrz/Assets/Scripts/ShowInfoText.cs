using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowInfoText : MonoBehaviour
{
    public void ShowToast(int duration)
    {
        StartCoroutine(ShowToastCOR(duration));
    }

    private IEnumerator ShowToastCOR(int duration)
    {
        Text textObj = gameObject.GetComponent<Text>();
        Color originalColor = textObj.color;

        yield return FadeInAndOut(textObj, true, 0.5f);

        float counter = 0;
        while (counter < duration)
        {
            counter += Time.deltaTime;
            yield return null;
        }

        //Fade out
        yield return FadeInAndOut(textObj, false, 0.5f);

        textObj.enabled = false;
        textObj.color = originalColor;
    }
    
    IEnumerator FadeInAndOut(Text targetText, bool fadeIn, float duration)
    {
        //Set Values depending on if fadeIn or fadeOut
        float a, b;
        if (fadeIn)
        {
            a = 0f;
            b = 1f;
        }
        else
        {
            a = 1f;
            b = 0f;
        }

        Color currentColor = Color.clear;
        float counter = 0f;

        while (counter < duration)
        {
            counter += Time.deltaTime;
            float alpha = Mathf.Lerp(a, b, counter / duration);

            targetText.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
            yield return null;
        }
    }
}
