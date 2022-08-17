using Cynteract.CCC.Charts;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Lean.Localization;


namespace Cynteract.CCC
{
    public class HomeWindow : CCCWindow
    {
        public Button traingButton, gameSelectionButton, achievementsButton, statisticsWindowButton, diaryWindowButton;
        public TextMeshProUGUI remainingTrainingTime;
        public PointDisplayer pointDisplayer;
        public TextMeshProUGUI score;
        public TextMeshProUGUI playTime;
        public TextMeshProUGUI welcomeText;
        public HomeAchievementPanel achievementPanel;
        public HomeLoadingScreen loadingScreen;
        //private bool isFirstTime = false;

        protected override void OnClose(bool wasDestroyed)
        {
        }

        protected override void OnInit()
        {
            traingButton.onClick.AddListener(() => CynteractControlCenter.Training());
            gameSelectionButton.onClick.AddListener(() => CynteractControlCenter.SelectGameWindow());
            achievementsButton.onClick.AddListener(() => CynteractControlCenter.AchievementsWindow());
            statisticsWindowButton.onClick.AddListener(() => CynteractControlCenter.StatisticsWindow());
            diaryWindowButton.onClick.AddListener(() => CynteractControlCenter.DiaryWindow());
        }

        protected override void OnOpen()
        {
            InitAsync();
        }
        private void InitAsync()
        {
            loadingScreen.gameObject.SetActive(true);
            Debug.Log("Initializing");
            pointDisplayer.Display();
            score.text = TrainingsManager.instance.GetScoreSum().ToString();
            AchievementManager.Load();
            achievementPanel.Init();
            welcomeText.text = LeanLocalization.GetTranslationText("Welcome") +" "+ Database.DatabaseManager.instance.User.username + "!";
            var trainingTime = TrainingsManager.instance.GetRemainingTrainingTime();
            var minutesRemaining = LeanLocalization.GetTranslationText("minutes remaining") ?? "Minuten verbleibend";
            remainingTrainingTime.text = CCCUtility.FormatTime(trainingTime) + " " + minutesRemaining;
            loadingScreen.gameObject.SetActive(false);

        }
    }
}