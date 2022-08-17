using Cynteract.CCC;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cynteract.Database;

namespace Cynteract.CCC
{
    public class FinishWindow : CCCWindow
    {
        public Button closeButton, abortButton;
        public StarsSelector starsSelector, painSelector;
        public TMP_InputField commentField;
        public Feedback feedback;

        protected override void OnClose(bool wasDestroyed)
        {

        }

        protected override void OnInit()
        {
            closeButton.onClick.AddListener(() => Home());
            abortButton.onClick.AddListener(() => Abort());
            starsSelector.onSelection = OnSetStars;
            painSelector.onSelection = OnSetPain;
            starsSelector.Init();
            painSelector.Init();
        }



        private void OnSetPain(int num)
        {
            feedback.pain = num+1;
        }

        private void OnSetStars(int num)
        {
            feedback.stars = num+1;
        }

        private async void Home()
        {
            feedback.comments = commentField.text;
            feedback.timestamp = DateTime.Now.Ticks;
            DatabaseManager.instance.InsertFeedback(feedback);
            await DatabaseManager.instance.SyncFeedback();
            CynteractControlCenter.Home();
        }
        private void Abort()
        {

            CynteractControlCenter.Home();

        }

        protected override void OnOpen()
        {
            feedback = new Feedback();
            
            starsSelector.DeactivateAll();
            painSelector.DeactivateAll();
        }
    }
}