using Cynteract.OnlineDatabase;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
namespace Cynteract.CCC
{
    public class ChangeEmailPopupWindow :YesNoPopupWindow
{
        public PasswordField oldPasswordField ;
        public TMP_InputField newEmailField;
        public Message message;

        protected override void OnInit()
        {

            base.OnInit();
            message.Hide();
        }
        protected override void ButtonClicked(bool accepted)
        {
            if (accepted)
            {
                TryToChangeEmail();
            }
            else
            {
                base.ButtonClicked(accepted);
            }
        }


        private async void TryToChangeEmail()
        {
            if (await Database.DatabaseManager.instance.CheckPassword(oldPasswordField.inputField.text))
            {
                var response = await Database.DatabaseManager.instance.SetEmail(newEmailField.text);
                switch (response)
                {
                    case NotConnectedResponse _:
                        base.ButtonClicked(false);
                        return;
                    case StatusResponse status:
                        switch (status.statusCode)
                        {
                            case System.Net.HttpStatusCode.OK:
                                base.ButtonClicked(true);
                                return;
                            case System.Net.HttpStatusCode.BadRequest:
                                message.DisplayTranslated("The email you provided is not a valid email.");
                                return;
                        }
                        break;
                    default:
                        break;
                }

            }
            else
            {
                message.DisplayTranslated("The current password is wrong");
            }
        }
    }
}