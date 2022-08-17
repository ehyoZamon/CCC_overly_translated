using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Cynteract.CCC
{
    public class MessagePopupWindow : PopupWindow
    {
        Action<bool> callbackAction;
        public TextMeshProUGUI messageTextMesh;
        public Button okButton;
        protected override void OnClose()
        {
            callbackAction(true);
        }

        protected override void OnInit()
        {
            okButton.onClick.AddListener(Close);
        }
        public void SubscribeOnClosed(Action<bool> callback)
        {
            this.callbackAction = callback;
        }
        public void SetText(string text)
        {
            messageTextMesh.text = text;
        }
    }
}