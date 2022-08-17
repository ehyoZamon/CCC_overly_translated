using Cynteract.CynteractInput;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Lean.Localization;
using TMPro;
namespace Cynteract.CCC {
    public class StatisticsWindow :CCCWindow
    {
        public StatisticsGamePanel statisticsGamePanelPrefab;
        [ReadOnly]
        public StatisticsGamePanel[] instantiatedStatisticsGamePanels;
        public RectTransform gamePanelsParent;
        public Button backButton;
        public TMPro.TextMeshProUGUI pointSumTextMesh, movementsTextMesh,playtimeTextMesh;
        public Sorter titleSorter, totalScoreSorter,highscoreSorter, globalHighscoreSorter,rankSorter,movementsSorter,timeSorter;
        private Sorter[] sorters;

        protected override void OnClose(bool wasDestroyed)
        {
        }

        protected override void OnInit()
        {
            sorters =new  Sorter[]{ titleSorter, totalScoreSorter, highscoreSorter, globalHighscoreSorter, rankSorter, movementsSorter, timeSorter };

            backButton.onClick.AddListener(()=>CynteractControlCenter.Home());
            titleSorter.Init<StatisticsGamePanel>(gamePanelsParent, panel => panel.titleTextMesh.text, sorters);
            totalScoreSorter.Init<StatisticsGamePanel>(gamePanelsParent, panel => panel.TotalScore, sorters);
            highscoreSorter.Init<StatisticsGamePanel>(gamePanelsParent, panel => panel.HighScore, sorters);
            globalHighscoreSorter.Init<StatisticsGamePanel>(gamePanelsParent, panel => panel.GlobalScore, sorters);
            rankSorter.Init<StatisticsGamePanel>(gamePanelsParent, panel => panel.Rank, sorters);
            movementsSorter.Init<StatisticsGamePanel>(gamePanelsParent, panel => panel.MovementsSum, sorters);
            timeSorter.Init<StatisticsGamePanel>(gamePanelsParent, panel => panel.Time, sorters);

            titleSorter.Toggle();
        }

        protected override void OnOpen()
        {
            if (instantiatedStatisticsGamePanels!=null)
            {
                foreach (var item in instantiatedStatisticsGamePanels)
                {
                    if (item)
                    {
                        Destroy(item.gameObject);
                    }
                }
            }
            instantiatedStatisticsGamePanels = new StatisticsGamePanel[CynteractControlCenter.instance.cGames.Length];
            gamePanelsParent.sizeDelta= new Vector2(gamePanelsParent.sizeDelta.x ,instantiatedStatisticsGamePanels.Length * 150);
            for (int i = 0; i < instantiatedStatisticsGamePanels.Length; i++)
            {
                instantiatedStatisticsGamePanels[i] = Instantiate(statisticsGamePanelPrefab, gamePanelsParent);
                instantiatedStatisticsGamePanels[i].Init(CynteractControlCenter.instance.cGames[i]);
            }
            pointSumTextMesh.text =  $"{LeanLocalization.GetTranslationText("Points")}: {TrainingsManager.instance.GetScoreSum()}";
            movementsTextMesh.text = $"{LeanLocalization.GetTranslationText("Movements")}: {TrainingsManager.instance.GetTotalMovements()}";
            System.TimeSpan timeSpan = TrainingsManager.instance.GetTotalTrainingTime();
            playtimeTextMesh.text = $"{LeanLocalization.GetTranslationText("Playtime")}: {timeSpan.Hours}h {timeSpan.Minutes}m {timeSpan.Seconds}s";
        }
    }
}