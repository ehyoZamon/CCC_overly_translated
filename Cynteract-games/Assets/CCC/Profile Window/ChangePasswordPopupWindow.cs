using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Localization;
using Cynteract.OnlineDatabase;

namespace Cynteract.CCC
{
    public class ChangePasswordPopupWindow : YesNoPopupWindow
    {
        public PasswordField oldPasswordField, newPasswordField, newPasswordRepeatField;
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
                TryToChangePassword();

            }
            else
            {
                base.ButtonClicked(accepted);
            }
        }
        async void TryToChangePassword()
        {
            if (await Database.DatabaseManager.instance.CheckPassword(oldPasswordField.inputField.text))
            {
                if (newPasswordField.inputField.text == "")
                {
                    message.DisplayTranslated("The new password can not be empty");
                }
                else if (newPasswordField.inputField.text == oldPasswordField.inputField.text)
                {
                    message.DisplayTranslated("The new password is the same as the old one");
                }
                else if (newPasswordField.inputField.text == newPasswordRepeatField.inputField.text)
                {
                    var response =await Database.DatabaseManager.instance.SetPassword(newPasswordField.inputField.text);
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
                                    message.DisplayTranslated("The password must be at least 6 characters long, contain upper and lower case letters and numbers. Spaces are not allowed");
                                    return;
                            }
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    message.DisplayTranslated("New passwords do not match");
                }
            }
            else
            {
                message.DisplayTranslated("The current password is wrong");

            }
        }

    }
}