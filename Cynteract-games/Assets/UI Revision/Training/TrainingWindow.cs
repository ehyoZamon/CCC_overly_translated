using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Cynteract.CCC
{
    public class TrainingWindow : CCCWindow
    {
        public GamePanel gamePanelPrefab;
        public Transform gamePanelParent;
        public Button playButton, backButton;
        private TrainingGame[] cGames;
        [ReadOnly]
        public GamePanel[] panels;


        protected override void OnInit()
        {

        }

        protected override void OnOpen()
        {
            DestroyPanels();
            cGames = TrainingsManager.instance.GetRandomGames(3);
            panels = new GamePanel[cGames.Length];
            playButton.onClick.RemoveAllListeners();
            playButton.onClick.AddListener(StartGames);
            backButton.onClick.RemoveAllListeners();
            backButton.onClick.AddListener(() => CynteractControlCenter.Home());
            for (int i = 0; i < panels.Length; i++)
            {
                panels[i] = Instantiate(gamePanelPrefab, gamePanelParent);
                panels[i].Init(cGames[i]);
            }
        }
        protected override void OnClose(bool wasDestroyed)
        {
            DestroyPanels();

            panels = null;
        }

        private void DestroyPanels()
        {
            if (panels != null)
            {
                foreach (var item in panels)
                {
                    if (item != null)
                    {
                        Destroy(item.gameObject);
                    }
                }
            }
        }

        private async void StartGames()
        {
            var trainingTime =  TrainingsManager.instance.GetRemainingTrainingTime();
            if (trainingTime == TimeSpan.Zero)
            {
                bool startGame = await CynteractControlCenter.PlaytimeWarningPopup();
                if (!startGame)
                {
                    return;
                }
            }
            GameTrainingController.instance.StartGames(cGames);
        }
    }
}
