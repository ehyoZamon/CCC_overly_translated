using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Cynteract.CCC
{
    public abstract class PopupWindow :MonoBehaviour
    {
        public void Init()
        {
            OnInit();
        }
        public void Close()
        {
            OnClose();
            Destroy(gameObject);
        }

        protected abstract void OnClose();
        protected abstract void OnInit();
    }
    public class TutorialPopupWindow : PopupWindow
    {
        public Button yesButton, noButton;
        Action<bool> callbackAction;
        protected override void OnClose()
        {
        }

        protected override void OnInit()
        {
            yesButton.onClick.AddListener(() => ButtonClicked(true));
            noButton.onClick.AddListener(() => ButtonClicked(false));
        }

        private void ButtonClicked(bool tutorial)
        {
            callbackAction(tutorial);
            Close();
        }

        public void SubscribeOnClosed(Action<bool> callback)
        {
            this.callbackAction = callback;
        }
    }
}