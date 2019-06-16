using PlayFab;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class shopBtnScript : MonoBehaviour
{

    public Text credits;
    public GameObject shopPanel;

    public void OnButtonClicked(int which)
    {
        int creditsTotal = int.Parse(AccountManager.instance.Credits);
        switch (which)
        {
            case 1:
                creditsTotal += 500;
                AccountManager.instance.Credits = creditsTotal.ToString();
                credits.text = AccountManager.instance.Credits;
                AddCredits(500);
                break;
            case 2:
                creditsTotal += 1000;
                AccountManager.instance.Credits = creditsTotal.ToString();
                credits.text = AccountManager.instance.Credits;
                AddCredits(1000);
                break;
            case 3:
                creditsTotal += 5000;
                AccountManager.instance.Credits = creditsTotal.ToString();
                credits.text = AccountManager.instance.Credits;
                AddCredits(5000);
                break;
            case 4:
                creditsTotal += 10000;
                AccountManager.instance.Credits = creditsTotal.ToString();
                credits.text = AccountManager.instance.Credits;
                AddCredits(10000);
                break;
        }

        shopPanel.SetActive(false);

    }

    public void AddCredits(int amount)
    {
        var currencyRequest = new AddUserVirtualCurrencyRequest { VirtualCurrency = "CR", Amount = amount };
        PlayFabClientAPI.AddUserVirtualCurrency(currencyRequest, OnSuccess, OnFailure);
    }

    private void OnSuccess(ModifyUserVirtualCurrencyResult result)
    {
        Debug.Log($"Added currency, new balance: {result.Balance}");
        if (AccountManager.CurrentPlayerInventory.VirtualCurrency.TryGetValue("CR", out int val))
        {
            AccountManager.CurrentPlayerInventory.VirtualCurrency["CR"] = val;
        }
    }

    private void OnFailure(PlayFabError error)
    {
        Debug.Log(error.GenerateErrorReport());
    }
}
