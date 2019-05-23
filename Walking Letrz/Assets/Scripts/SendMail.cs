
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    class SendMail
    {

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
    }
}
