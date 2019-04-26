using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using Assets.Scripts;

public class Credit : MonoBehaviour 
{
    public void AddCredits(int amount)
    {
        var currencyRequest = new AddUserVirtualCurrencyRequest {VirtualCurrency = "CR", Amount = amount};
        PlayFabClientAPI.AddUserVirtualCurrency(currencyRequest, OnSuccess, OnFailure);
    }

    private void OnSuccess(ModifyUserVirtualCurrencyResult result)
    {
        Debug.Log($"Added currency, new balance: {result.Balance}");
        if(AccountManager.CurrentPlayerInventory.VirtualCurrency.TryGetValue("CR", out int val))
        {
            AccountManager.CurrentPlayerInventory.VirtualCurrency["CR"] = val;
        }
    }

    private void OnFailure(PlayFabError error)
    {
        Debug.Log(error.GenerateErrorReport());
    }
}
