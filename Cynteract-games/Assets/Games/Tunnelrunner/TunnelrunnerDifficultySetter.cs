using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Cynteract.Tunnelrunner
{
    [Serializable]
    public class DifficultySettings
    {
        public float timescale;
        public float upwardsVelocity, rightVelocity, acceleration;
        public DifficultySettings()
        {
        }

        public DifficultySettings(float timescale)
        {
            this.timescale = timescale;
        }

        public override string ToString()
        {
            return $"Timescale: {timescale}";
        }
    }

    public class TunnelrunnerDifficultySetter
    {
        public static int lastDifficultyIndex =3;
        public static Tunnelrunner.LevelStats lastLevelStats=new Tunnelrunner.LevelStats();
        public static DifficultySettings[] difficultySettings =
        {
            new DifficultySettings
            {
                timescale=1,
                upwardsVelocity=4,
                rightVelocity=4,
                acceleration=30,
            },
            new DifficultySettings
            {
                timescale=1,
                upwardsVelocity=5,
                rightVelocity=5,
                acceleration=25,
            },
            new DifficultySettings
            {
                timescale=1,
                upwardsVelocity=6,
                rightVelocity=6,
                acceleration=20,
            },
            new DifficultySettings
            {
                timescale=1,
                upwardsVelocity=7,
                rightVelocity=7,
                acceleration=15,
            },
            new DifficultySettings
            {
                timescale=1,
                upwardsVelocity=10,
                rightVelocity=10,
                acceleration=15,
            },
            new DifficultySettings
            {
                timescale=1,
                upwardsVelocity=15,
                rightVelocity=15,
                acceleration=25,
            }
        };
        public static DifficultySettings currentDifficulty = difficultySettings[lastDifficultyIndex];
        private static int[] requiredPoints= { 
        0,500,1000,2000,
        3000,
        4000
        };
        private static int[] requiredStreak = {
        0,0,0,4,
        8,
        12
        };
        public static void CalculateNewDifficulty(Tunnelrunner.LevelStats levelStats)
        {
            int currentDifficultyIndex = lastDifficultyIndex;
            if (levelStats.score<lastLevelStats.score||levelStats.timeToDeath<TimeSpan.FromSeconds(10)|| levelStats.score < 500)
            {
                if (lastDifficultyIndex > 0)
                {
                    currentDifficultyIndex=lastDifficultyIndex - 1;
                }
            }
            else if (levelStats.score > lastLevelStats.score)
            {
                if (lastDifficultyIndex < difficultySettings.Length - 1)
                {
                    if (levelStats.score>= requiredPoints[lastDifficultyIndex + 1]&&levelStats.scoreStreak>=requiredStreak[lastDifficultyIndex + 1])
                    {
                        currentDifficultyIndex=lastDifficultyIndex + 1;

                    }
                }
            }

            currentDifficulty = difficultySettings[ currentDifficultyIndex];
            
            lastLevelStats = levelStats;
            lastDifficultyIndex = currentDifficultyIndex;
        }
        public static void TryMakeItHarder(Tunnelrunner.LevelStats levelStats)
        {
            int currentDifficultyIndex = lastDifficultyIndex;
            if (lastDifficultyIndex < difficultySettings.Length - 1)
            {
                if (levelStats.score >= requiredPoints[lastDifficultyIndex + 1])
                {
                    currentDifficultyIndex = lastDifficultyIndex + 1;

                }
            }
            currentDifficulty = difficultySettings[currentDifficultyIndex];
            lastDifficultyIndex = currentDifficultyIndex;
        }
        public static DifficultySettings GetDifficultySettings()
        {
            Debug.Log(currentDifficulty);
            return currentDifficulty;
        }
    }
}