using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

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
        }

        private void OnFailure(PlayFabError error)
        {
            Debug.Log(error.GenerateErrorReport());
        }
}
