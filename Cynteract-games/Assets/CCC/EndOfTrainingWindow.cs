using Cynteract.CCC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Cynteract.CCC
{
    public class EndOfTrainingWindow : CCCWindow
    {
        public Button mainMenuButton, quitButton;
        public GameQuitting quit;

        protected override void OnClose(bool wasDestroyed)
        {
        }

        protected override void OnInit()
        {
            mainMenuButton.onClick.AddListener(() => CynteractControlCenter.Home());
            quitButton.onClick.AddListener(() => CynteractControlCenter.Quit());

        }

        protected override void OnOpen()
        {
        }
    }
}
