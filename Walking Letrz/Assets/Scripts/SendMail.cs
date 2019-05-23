<<<<<<< HEAD
﻿using PlayFab;
using PlayFab.ClientModels;
using PlayFab.ServerModels;
using System;
using System.Text.RegularExpressions;
using UnityEngine;
=======
﻿using UnityEngine;
>>>>>>> parent of 3bfd46a... Email shit
using UnityEngine.UI;

namespace Assets.Scripts
{
    class SendMail
    {
<<<<<<< HEAD
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
                CustomId = "SomeCustomID", // replace with your own Custom ID
                CreateAccount = true // otherwise this will create an account with that ID
            };

            var username = "dummyaccount"; // Set this to your username
            var password = "boeitnietechtnatuurlijk"; // Set this to your password
            var emailAddress = emailaddress.text; // Set this to your own email

            PlayFabClientAPI.LoginWithCustomID(loginReq, loginRes =>
            {
                Debug.Log("Successfully logged in player with PlayFabId: " + loginRes.PlayFabId);
                AddUserNamePassword(username, password, emailAddress); // Add a username and password
                AddOrUpdateContactEmail(loginRes.PlayFabId, emailAddress);
            }, FailureCallback);
        }

        void AddUserNamePassword(string username, string password, string emailAddress)
        {
            var request = new AddUsernamePasswordRequest
            {
                Username = username,
                Password = password,
                Email = emailAddress // Login email
            };
            PlayFabClientAPI.AddUsernamePassword(request, result =>
            {
                Debug.Log("The player's account now has username and password");
            }, FailureCallback);
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
            }, FailureCallback);
            SendTemplateMail(playFabId);
        }

        private void DeleteDummyAccount(string playFabId)
        {
            DeletePlayerRequest r = new DeletePlayerRequest
            {
                PlayFabId = playFabId
            };
            PlayFabServerAPI.DeletePlayer(r, SuccesDelete, FailureCallback);
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
            PlayFabServerAPI.SendEmailFromTemplate(r, Succes, FailureCallback);
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
=======
        Text emailaddress;
        void SendEmail ()
         {
              string email = emailaddress.text;
              string subject = MyEscapeURL("Invite Walking Letrz");
              string body = MyEscapeURL("My Body\r\nFull of non-escaped chars");
              Application.OpenURL("mailto:" + email + "?subject=" + subject + "&body=" + body);
         }
         string MyEscapeURL (string url)
         {
               return WWW.EscapeURL(url).Replace("+","%20");
         }
>>>>>>> parent of 3bfd46a... Email shit
    }
}
