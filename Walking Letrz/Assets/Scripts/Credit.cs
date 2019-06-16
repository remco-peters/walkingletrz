using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using Assets.Scripts;

public class Credit : MonoBehaviour 
{
    /// <summary>
    /// Send a request to playfab to increase the credit count for a player
    /// </summary>
    /// <param name="amount"></param>
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

    /// <summary>
    /// Set the new credit count in the game instance so it is updated in the ui
    /// </summary>
    /// <param name="result"></param>
    private void OnSuccess(ModifyUserVirtualCurrencyResult result)
    {
        GameInstance.instance.credits = result.Balance;
    }

    private void OnFailure(PlayFabError error)
    {
        Debug.Log(error.GenerateErrorReport());
    }
}
