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

    public void RemoveCredits(int amount)
    {
        var currencyRequest = new SubtractUserVirtualCurrencyRequest {VirtualCurrency = "CR", Amount = amount};
        PlayFabClientAPI.SubtractUserVirtualCurrency(currencyRequest, OnSuccess, OnFailure);
    }

    private void OnSuccess(ModifyUserVirtualCurrencyResult result)
    {
        Debug.Log($"Added currency, new balance: {result.Balance}");
        GameInstance.instance.credits = result.Balance;
    }

    private void OnFailure(PlayFabError error)
    {
        Debug.Log(error.GenerateErrorReport());
    }
}
