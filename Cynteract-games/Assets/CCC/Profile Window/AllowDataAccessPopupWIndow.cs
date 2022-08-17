using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Cynteract.Database;
using System.Threading.Tasks;
using Cynteract.OnlineDatabase;

namespace Cynteract.CCC
{
    public class AllowDataAccessPopupWIndow : YesNoPopupWindow
    {
        public PasswordField oldPasswordField;
        public TMP_InputField therapistIDField;
        public Message message;
        private bool connected;

        protected override void OnInit()
        {
            InitTherapistField();
            base.OnInit();
            message.Hide();
        }

        private  async void InitTherapistField()
        {
            var therapistName = await DatabaseManager.instance.GetTherapistName();
            if (therapistName != null)
            {
                connected = true;
                therapistIDField.gameObject.SetActive(false);
            }
        }

        protected override void ButtonClicked(bool accepted)
        {
            if (accepted)
            {
                TryGivingAccess();
            }
            else
            {
                base.ButtonClicked(accepted);
            }
        }

        private async void TryGivingAccess()
        {
            int therapistID;
            if (connected)
            {
                therapistID = -1;
            }
            else
            {
                therapistID = int.Parse(therapistIDField.text);
            }
            var allowAccessResponse= await DatabaseManager.instance.AllowTherapistAccess(therapistID);
            switch (allowAccessResponse)
            {
                case NotConnectedResponse _:
                    base.ButtonClicked(false);
                    break;
                case StatusResponse status:
                    switch (status.statusCode)
                    {
                        case System.Net.HttpStatusCode.OK:
                            base.ButtonClicked(true);
                            break;
                    }
                    break;
                default:
                    break;
            }
        }
    }
}