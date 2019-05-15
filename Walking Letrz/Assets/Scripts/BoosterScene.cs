using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BoosterScene : MonoBehaviour
{
    // Start is called before the first frame update
    private long credits;
    void Start()
    {
        credits = GameInstance.instance.credits;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerable<Toggle> GetActiveToggles(GameObject panel)
    {
        Toggle[] toggles = panel.GetComponentsInChildren<Toggle>() ?? new Toggle[] { };
        foreach(Toggle t in toggles)
        {
            if (t.isOn) yield return t;
        }
    }

    public void PlayBttnClick(GameObject panel)
    {
        IEnumerable<Toggle> toggles = GetActiveToggles(panel);
        foreach(Toggle t in toggles)
        {
            GameInstance.instance.selectedBoosters.Add(t.name);
        }
        SceneSwitcher.SwitchSceneStatic("GameScene");
    }

    public void BoosterBtnClick(Toggle toggle)
    {
        long.TryParse(toggle.transform.Find("Credits")?.GetComponent<Text>()?.text, out long toggleCredits);
        if (toggleCredits > credits)
        {
            toggle.isOn = false;
            Debug.Log("Not enough credits");
            return;
        }
        else if (GetActiveToggles(toggle.transform.parent.gameObject).Count() > 3)
        {
            toggle.isOn = false;
            Debug.Log("Max 3 boosters");
            return;
        }
        else
        {
            credits += toggle.isOn ? toggleCredits * -1: toggleCredits;
            ColorBlock cb = toggle.colors;
            cb.normalColor = toggle.isOn ? new Color(cb.normalColor.r, cb.normalColor.g, cb.normalColor.b, 1) : new Color(cb.normalColor.r, cb.normalColor.g, cb.normalColor.b, 0.5f);
            cb.highlightedColor = toggle.isOn ? new Color(cb.normalColor.r, cb.normalColor.g, cb.normalColor.b, 1) : new Color(cb.normalColor.r, cb.normalColor.g, cb.normalColor.b, 0.5f);
            toggle.colors = cb;
        }      
    }
}
