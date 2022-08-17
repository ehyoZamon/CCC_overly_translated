using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Cynteract.Database;
using System.Threading.Tasks;

namespace Cynteract.CCC
{
    public class ConnectionWindow : CCCWindow
    {
        private const int MaxFailsBeforeNotice = 2;
        public TextMeshProUGUI statusText, dotsText;
        public Button quitButton;

        public Transform buttons;
        Coroutine dotsCoroutine;

        Callback onConnectedCallback, onConnectionTimeoutCallback;
        Callback<int> onConnectionFailCallback;

        protected override void OnInit()
        {
            quitButton.onClick.AddListener(() => GameQuitting.QuitGame());
        }

        protected override void OnOpen()
        {
            dotsCoroutine = StartCoroutine(LoadingSymbols.Dots(dotsText, .5f, 10));
            buttons.gameObject.SetActive(false);
            Connect();
        }

        private async void Connect()
        {
            var connected = await DatabaseManager.instance.TestConnection();
            if (connected)
            {
                OnConnected();
            }
            else
            {
                OnConnectionFailed();
            }
        }

        protected override void OnClose(bool wasDestroyed)
        {
        }
        private void OnConnected()
        {
            CynteractControlCenter.LoginWindow();
        }
        private void OnConnectionFailed()
        {
            CynteractControlCenter.LoginWindow();
        }
        private void OnConnectionFailed(int fails)
        {
            if (fails>MaxFailsBeforeNotice)
            {
                OnConnectionFailed();
            }
        }
        private void OnConnectionTimeout()
        {
            OnConnectionFailed();
        }
    }
}