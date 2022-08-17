using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
namespace Cynteract.CCC
{
    public class DeleteDataPopupWindow : YesNoPopupWindow
    {
        public PasswordField oldPasswordField;
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
                TryToDeleteData();
            }
            else
            {
                base.ButtonClicked(accepted);
            }
        }
        async void TryToDeleteData()
        {
            if (await Database.DatabaseManager.instance.CheckPassword(oldPasswordField.inputField.text))
            {
                AchievementManager.DeleteData();
                Database.DatabaseManager.instance.DeleteSynchedTrainingsData();
                base.ButtonClicked(true);
            }
            else
            {
                message.Display("Das Passwort scheint nicht korrekt zu sein");
            }
        }
    }
}