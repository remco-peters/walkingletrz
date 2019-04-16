using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameboardScroll : MonoBehaviour
{
    public void ScrollDownBar()
    {
        Canvas.ForceUpdateCanvases();
        GetComponent<ScrollRect>().verticalNormalizedPosition = 0f;
        Canvas.ForceUpdateCanvases();
    }
}
