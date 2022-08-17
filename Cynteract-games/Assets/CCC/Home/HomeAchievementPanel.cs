using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Cynteract.CCC
{
    public class HomeAchievementPanel : MonoBehaviour
    {
        public Achievement achievementPrefab;
        public Achievement[] instantiatedAchievements;
        public Transform achievementPanel;
        public void Init()
        {

            if (instantiatedAchievements!=null)
            {
                foreach (var item in instantiatedAchievements)
                {
                    if (item)
                    {
                        Destroy(item.gameObject);
                    }
                }   
            }
            instantiatedAchievements = new Achievement[3];
            int index = 0;
            if (AchievementManager.GetLastAchievement(out KeyValuePair<string, AchievementManager.AchievementData> newestAchievement))
            {
                Achievement achievement = Instantiate(achievementPrefab, achievementPanel);
                achievement.Init(newestAchievement.Key, newestAchievement.Value.description);
                achievement.Set(newestAchievement.Value.CalculateCurrentData());
                achievement.Open();
                instantiatedAchievements[0] = achievement;
                index = 1;
            }
            var closest = AchievementManager.GetClosestAchievementsArray();
            for (int i = index; i < instantiatedAchievements.Length&&i<closest.Length+index; i++)
            {
                Achievement achievement = Instantiate(achievementPrefab, achievementPanel);
                var keyValuePair = closest[i- index];
                achievement.Init(keyValuePair.Key, keyValuePair.Value.description);
                achievement.Set(keyValuePair.Value.CalculateCurrentData());
                achievement.Open();

                instantiatedAchievements[i] = achievement;
            }
        }
    }
}