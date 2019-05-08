using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoosterScene : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayBttnClick(GameObject panel)
    {
        Toggle[] toggles = panel.GetComponentsInChildren<Toggle>() ?? new Toggle[] { };
        foreach(Toggle t in toggles)
        {
            if (!t.isOn) continue;
            Debug.Log(t.GetComponentInChildren<Text>().text);
        }
        SceneSwitcher.SwitchSceneStatic("GameScene");
    }

    public void BoosterBtnClick(Toggle toggle)
    {
        ColorBlock cb = toggle.colors;
        cb.normalColor = toggle.isOn ? new Color(cb.normalColor.r, cb.normalColor.g, cb.normalColor.b, 1) : new Color(cb.normalColor.r, cb.normalColor.g, cb.normalColor.b, 0.5f);
        cb.highlightedColor = toggle.isOn ? new Color(cb.normalColor.r, cb.normalColor.g, cb.normalColor.b, 1) : new Color(cb.normalColor.r, cb.normalColor.g, cb.normalColor.b, 0.5f);
        toggle.colors = cb;
        
    }
}
