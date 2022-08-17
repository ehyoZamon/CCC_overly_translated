using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Lean.Localization;
namespace Cynteract.CCC
{
    public class AchievementsWindow : CCCWindow
    {


        public Achievement achievementPrefab;
        public Transform leftPage, rightPage;
        public TextMeshProUGUI title;
        public Dictionary<string, Achievement> achievements=new Dictionary<string, Achievement>();
        public Button backButton;

        protected override void OnClose(bool wasDestroyed)
        {
            if (!wasDestroyed)
            {
                AchievementManager.Save();
            }
        }

        protected override void OnInit()
        {
            backButton.onClick.AddListener(() => CynteractControlCenter.Home());
            foreach (var item in achievements)
            {
                if (item.Value)
                {
                    Destroy(item.Value.gameObject);
                }
            }
            achievements = new Dictionary<string, Achievement>();
            int i = 0;

            foreach (var item in AchievementManager.achievementsDictionary)
            {
                Achievement achievement;
                if (i < 4)
                {
                    achievement = Instantiate(achievementPrefab, leftPage);
                }
                else
                {
                    achievement = Instantiate(achievementPrefab, rightPage);
                }
                achievement.Init(item.Key, item.Value.description);
                achievements.Add(item.Key, achievement);
                i++;
            }
        }

        protected override void OnOpen()
        {
            foreach (var item in achievements.Values)
            {
                item.Open();
            }
            title.text = LeanLocalization.GetTranslationText("My Achievements") ?? "Meine Erfolge";
            AchievementManager.Load();
            foreach (var item in achievements)
            {
                var data = AchievementManager.achievementsDictionary[item.Key].CalculateCurrentData();
                item.Value.Set(data);
            }
        }

    }
}