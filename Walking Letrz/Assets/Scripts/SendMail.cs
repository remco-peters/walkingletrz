using PlayFab;
using PlayFab.ClientModels;
using PlayFab.ServerModels;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    class SendMail : MyMonoBehaviour
    {
        public Text emailaddress;
        private const string templateId = "520D981132030A3";
        string PlayFabId {get;set; }

        public void Succes(SendEmailFromTemplateResult r)
        {
            Debug.Log("Succesfully send mail");
            DeleteDummyAccount(PlayFabId);
        }

        public void SendEmail()
        {
            if (!IsValidEmail(emailaddress.text)){
                Debug.Log("error");
                return;
            }
            var loginReq = new LoginWithCustomIDRequest
            {
                CustomId = RandomString(8), // replace with your own Custom ID
                CreateAccount = true // otherwise this will create an account with that ID
            };
            var emailAddress = emailaddress.text; // Set this to your own email
            PlayFabClientAPI.LoginWithCustomID(loginReq, loginRes =>
            {
                Debug.Log("Successfully logged in player with PlayFabId: " + loginRes.PlayFabId);
                AddOrUpdateContactEmail(loginRes.PlayFabId, emailAddress);
            }, FailureCallback, null, new Dictionary<string,string>{["X-SecretKey"] = "GX7ZFEH4AXZSHQGNSOKSJHCIKH73ONA5NAJG1QJO9GHYSIEIJ7" });
        }

        private static System.Random random = new System.Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        void AddOrUpdateContactEmail(string playFabId, string emailAddress)
        {
            var request = new AddOrUpdateContactEmailRequest
            {           
                EmailAddress = emailAddress,         
            };
            PlayFabClientAPI.AddOrUpdateContactEmail(request, result =>
            {
                Debug.Log("The player's account has been updated with a contact email");
            }, FailureCallback, null, new Dictionary<string,string>{["X-SecretKey"] = "GX7ZFEH4AXZSHQGNSOKSJHCIKH73ONA5NAJG1QJO9GHYSIEIJ7" });
            SendTemplateMail(playFabId);
        }

        private void DeleteDummyAccount(string playFabId)
        {
            DeletePlayerRequest r = new DeletePlayerRequest
            {
                PlayFabId = playFabId
            };
            PlayFabServerAPI.DeletePlayer(r, SuccesDelete, FailureCallback, null, new Dictionary<string,string>{["X-SecretKey"] = "GX7ZFEH4AXZSHQGNSOKSJHCIKH73ONA5NAJG1QJO9GHYSIEIJ7" });
        }

        public void SuccesDelete(DeletePlayerResult d)
        {
            Debug.Log("Succesfull deleted dummy account");
        }

        private void SendTemplateMail(string playFabId)
        {
            SendEmailFromTemplateRequest r = new SendEmailFromTemplateRequest{
                EmailTemplateId = templateId,
                PlayFabId = playFabId
            };
            r.EmailTemplateId = templateId;
            PlayFabId = playFabId;
            PlayFabServerAPI.SendEmailFromTemplate(r, Succes, FailureCallback, null, new Dictionary<string,string>{["X-SecretKey"] = "GX7ZFEH4AXZSHQGNSOKSJHCIKH73ONA5NAJG1QJO9GHYSIEIJ7" });
        }

        void FailureCallback(PlayFabError error)
        {
            Debug.LogWarning("Something went wrong with your API call. Here's some debug information:");
            Debug.LogError(error.GenerateErrorReport());
        }
        bool IsValidEmail(string email)
        {
            try {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch {
                return false;
            }
        }
    }
}
