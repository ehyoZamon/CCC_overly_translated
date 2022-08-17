using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Cynteract.CCC
{
    public class HelpWindow : CCCWindow
    {
        public Button backButton;
        protected override void OnClose(bool wasDestroyed)
        {

        }

        protected override void OnInit()
        {
            backButton.onClick.AddListener(() => CCCStatusBar.instance.CloseAllOverlays());

        }

        protected override void OnOpen()
        {

        }
    }
}