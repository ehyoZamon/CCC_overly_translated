using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Threading.Tasks;
using Lean.Localization;
using UnityEngine.EventSystems;
using Cynteract.OnlineDatabase;

namespace Cynteract.CCC {
    public class ResetPasswordPopupWindow : YesNoPopupWindow
    {
        public TMP_InputField emailField;
        public Message message;
        protected override void OnInit()
        {

            base.OnInit();
            message.Hide();
            EventSystem.current.SetSelectedGameObject(emailField.gameObject, null);

        }
        protected override void ButtonClicked(bool accepted)
        {

            if (accepted)
            {
                ResetPassword();
            }
            else
            {
                base.ButtonClicked(accepted);
            }
        }

        private async void ResetPassword()
        {
            var respose=await Database.DatabaseManager.instance.ResetPassword(emailField.text);
            switch (respose)
            {
                case NotConnectedResponse _:
                    message.Display(LeanLocalization.GetTranslationText("Please check your internet connection"));
                    break;
                case StatusResponse statusResponse:
                    switch (statusResponse.statusCode)
                    {
                        case System.Net.HttpStatusCode.NotAcceptable:
                            message.Display(LeanLocalization.GetTranslationText("Your provided email was no found"));
                            break;
                        case System.Net.HttpStatusCode.OK:
                            base.ButtonClicked(true);
                            break;
                    }
                    break;
            }

        }
    }
}