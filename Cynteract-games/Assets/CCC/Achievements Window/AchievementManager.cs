using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Cynteract.CCC
{

    public static class AchievementManager
    {
        [Serializable]
        public class AchievementData:CurrentData, ICloneable
        {
            public string description;
            public Func<int> currentValueFunction, totalValueFunction;
            public Func<bool> isActiveFunction;
            public DateTime achievementTime;
            public AchievementData(string description, Func<int> currentValueFunction, Func<int> totalValueFunction, Func<bool> isActiveFunction)
            {
                this.description = description;
                this.currentValueFunction = currentValueFunction;
                this.totalValueFunction = totalValueFunction;
                this.isActiveFunction = isActiveFunction;
            }
            public AchievementData(string description, Func<int> currentValueFunction, Func<int> totalValueFunction) : this(description, currentValueFunction, totalValueFunction, () => currentValueFunction() >= totalValueFunction())
            {

            }

            public AchievementData(string description, Func<int> currentValueFunction, Func<int> totalValueFunction, Func<bool> isActiveFunction, DateTime achievementTime) : this(description, currentValueFunction, totalValueFunction, isActiveFunction)
            {
                this.achievementTime = achievementTime;
            }

            public CurrentData CalculateCurrentData()
            {
                currentValue = currentValueFunction();
                totalValue = totalValueFunction();
                bool wasActive = active;
                active = isActiveFunction();
                if (!wasActive&&active)
                {
                    achievementTime = DateTime.Now;
                }
                return this as CurrentData;
            }

            public object Clone()
            {
                return new AchievementData(description, currentValueFunction, totalValueFunction, isActiveFunction, achievementTime);
            }
        }



        [Serializable]
        public class CurrentData
        {
            public bool active;
            public int currentValue, totalValue;
        }

        public static readonly Dictionary<string, AchievementData> achievementsDictionary = new Dictionary<string, AchievementData>()
        {
            {"Champion", new AchievementData("Train 3 days in a row", ()=>TrainingsManager.instance.GetMaxDaysInARow(), ()=>3)},
            {"Point Collector", new AchievementData("Reach 500 points in a game", ()=>TrainingsManager.instance.GetHighestScore(), ()=>5000)},
            {"Game Collector",  new AchievementData( "Play every game", ()=> TrainingsManager.instance.GetPlayedGames().Count, ()=>CynteractControlCenter.instance.cGames.Length)},
            {"Movement Record",  new AchievementData("Do your movement 100 times" , ()=> TrainingsManager.instance.MovementMaximum(), ()=>100)},
            {"Night owl",  new AchievementData("Train after 8 pm for at least 10 minutes", ()=>(int)TrainingsManager.instance.MaxTimeAfter8pm().TotalMinutes, ()=>10)},
            {"Weekend gamer",  new AchievementData("Train for at least 10 minutes on weekends", ()=>(int)TrainingsManager.instance.MaxTimeOnWeekends().TotalMinutes, ()=>10)},
            {"Game master",  new AchievementData("Play a game more than 10 times", ()=>(int)TrainingsManager.instance.GetMaxTimeGamePlayed(), ()=>10)},
            {"Explorer", new AchievementData("Collect 10 hidden items", ()=>0, ()=>10)},
        };
        public static Dictionary<string, AchievementData> achievements = new Dictionary<string, AchievementData>();
        public static void Save()
        {
            JsonFileManager.SaveEncrypted(achievements, GetSaveName());
        }
        public static void DeleteData()
        {
            achievements =CloneDictionaryCloningValues( achievementsDictionary);
            Save();
        }
        private static string GetSaveName()
        {
            return "achievements" + Database.DatabaseManager.instance.User.Username;
        }

        public static void Load()
        {
            try
            {
                var loadedAchievements = JsonFileManager.LoadEncrypted <Dictionary<string, AchievementData>>(GetSaveName());
                foreach (var item in achievements)
                {
                    if (loadedAchievements.ContainsKey(item.Key)&& achievements.ContainsKey(item.Key))
                    {
                        achievements[item.Key] = loadedAchievements[item.Key];
                    }
                }
            }
            catch (Exception)
            {

                achievements = CloneDictionaryCloningValues(achievementsDictionary);
            }
        }
        public static bool  GetLastAchievement(out KeyValuePair<string, AchievementData> newestAchievement)
        {
            bool achievedAnything=false;
            DateTime newest = DateTime.MinValue;
            foreach (var item in achievementsDictionary)
            {
                item.Value.CalculateCurrentData();
                if (item.Value.active&&item.Value.achievementTime>newest)
                {
                    achievedAnything = true;
                    newest = item.Value.achievementTime;
                    newestAchievement = item;
                }
            }
            return achievedAnything;
        }
        public static KeyValuePair<string, AchievementData>[] GetUnsolvedAchievements()
        {
            List<KeyValuePair<string, AchievementData>> unsolvedAchievements = new List<KeyValuePair<string, AchievementData>>();
            foreach (var item in achievementsDictionary)
            {
                item.Value.CalculateCurrentData();
                if (!item.Value.active)
                {
                    unsolvedAchievements.Add(item);
                }
            }
            return unsolvedAchievements.ToArray();
        }
        public static KeyValuePair<string, AchievementData>[] GetClosestAchievementsArray()
        {
            Comparison<KeyValuePair<string, AchievementData>> comparison = (left,right) =>
           {
               var x = left.Value;
               var y = right.Value;
               double xRatio = (double)x.currentValue / (double)x.totalValue;
               double yRatio = (double)y.currentValue / (double)y.totalValue;
               if (xRatio < yRatio)
               {
                   return 1;
               }
               else if (xRatio > yRatio)
               {
                   return -1;
               }
               else
               {
                   return 0;
               }
           };
            var unsolvedAchievements = GetUnsolvedAchievements();
            Array.Sort(unsolvedAchievements, comparison);
            return unsolvedAchievements;
        }
        public static Dictionary<TKey, TValue> CloneDictionaryCloningValues<TKey, TValue> (Dictionary<TKey, TValue> original) where TValue : ICloneable
        {
            Dictionary<TKey, TValue> ret = new Dictionary<TKey, TValue>(original.Count,
                                                                    original.Comparer);
            foreach (KeyValuePair<TKey, TValue> entry in original)
            {
                ret.Add(entry.Key, (TValue)entry.Value.Clone());
            }
            return ret;
        }
    }
}