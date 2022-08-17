

using Cynteract.CTime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
namespace Cynteract.Tunnelrunner
{


    public class Tunnelrunner : GameController
    {
        public GameOverScreen gameOverScreen;
        public static Tunnelrunner instance;
        public static RocketControl rocketControl;
        public LevelStats levelStats;
        public Scorer scorer;
        internal WorldGenerator worldGenerator;
        public new Cinemachine.CinemachineVirtualCamera camera;
        public int maxScore = 0;

        public void ScorePoints(int points, Transform transform)
        {
            levelStats.ScorePoints(points, transform);
            base.UpdateHighscore(levelStats.score);
        }

        public bool showGameOverScreen;
        public Tunnelrunner()
        {
            instance = this;
        }
        public void Init()
        {
            levelStats = new LevelStats();
            if (shouldStartTutorial)
            {
                StartTutorial();
            }
            else
            {
                StartStandardLevel();
            }
        }
        public void StartStandardLevel()
        {
            levelStats = new LevelStats();
            TunnelrunnerLevelSelector.instance.SpawnStandardWorld(TunnelrunnerDifficultySetter.GetDifficultySettings());
            camera.enabled = false;
            rocketControl.transform.position = Vector3.zero;
            rocketControl.transform.rotation = Quaternion.Euler(0, 0, -45);
            TimeControl.instance.ResetStandardTime();
            TimeControl.instance.ScaleStandardTime(0.5f, 1);
            rocketControl.Respawn();
            camera.enabled = true;
            levelStats.startTime = DateTime.Now;

        }
        public void StartTutorial()
        {
            levelStats = new LevelStats();
            TunnelrunnerLevelSelector.instance.SpawnTutorial();
            rocketControl.transform.position = Vector3.zero;
            rocketControl.transform.rotation = Quaternion.Euler(0, 0, -45);
            rocketControl.Respawn();

        }
        public int GetScoreStreak()
        {
            return levelStats.scoreStreak;
         }
        [Serializable]
        public class LevelStats
        {
            public int score, scoreStreak;
            public float timeOfLastScore;
            public List<Vector3[]> points = new List<Vector3[]>();
            internal float lastOffset;
            internal float lastDiameter;
            internal int currentLevel;
            public TimeSpan timeToDeath;
            public DateTime startTime;
            public LevelStats()
            {
                startTime = DateTime.Now;
                score = 0;
                currentLevel = 0;
                lastDiameter = 10;
                lastOffset = 0;
                if (instance.scorer)
                {
                    instance.scorer.Score(0, 0);
                }
            }
            public void ScorePoints(int points, Transform item)
            {
                score += points;
                instance.scorer.Score(points, score, item);
                UpdateScore();

            }
            public void ScorePoints(int points)
            {
                score += points;
                instance.scorer.Score(points, score);
                UpdateScore();
            }

            private void UpdateScore()
            {
                if(Time.fixedTime- timeOfLastScore<=2f)
                {
                    scoreStreak++;
                }
                else
                {
                    scoreStreak = 0;
                }
                //print($"Time:{Time.fixedTime} Time of last pickup: { timeOfLastScore} Diff {Time.fixedTime - timeOfLastScore} Streak: {scoreStreak}");
                timeOfLastScore = Time.fixedTime;
                int threash = Tunnelrunner.instance.worldGenerator.levels.CurrentThreashold();
                if (score > threash && threash > 0)
                {
                    currentLevel++;
                }
                instance.maxScore = Mathf.Max(score, instance.maxScore);
                TunnelrunnerDifficultySetter.TryMakeItHarder(this);
                Tunnelrunner.instance.RecalculateDifficulty();
            }

            public void CalculateTimeToDeath()
            {
                timeToDeath = DateTime.Now - startTime;
                Debug.Log(timeToDeath);
            }
        }

        private void RecalculateDifficulty()
        {
            var difficulty = TunnelrunnerDifficultySetter.GetDifficultySettings();
            TunnelrunnerLevelSelector.instance.SetDifficulty(difficulty);
        }

        public void Destruction()
        {
            rocketControl.Destruction();
        }
        private Task WaitTask(int timeout)
        {
            TaskCompletionSource<bool> tcs1 = new TaskCompletionSource<bool>();
            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(timeout);
                tcs1.SetResult(true);
            });
            return tcs1.Task;
        }
        private Task<bool> GameOverScreen()
        {
            var task = new TaskCompletionSource<bool>();
            var popup = Instantiate(gameOverScreen, PopupWindowCanvas.instance.transform);
            popup.Init();
            popup.SubscribeOnClosed(x => task.SetResult(x));
            popup.SetCurrentScore((uint)levelStats.score);
            return task.Task;
        }
        public async void Respawn(GameObject explosion)
        {
            await WaitTask(2000);
            if (levelStats.score>500)
            {
                if (showGameOverScreen)
                {

                    await GameOverScreen();
                }
            }
            StartCoroutine(RespawnAfterSeconds(0, explosion));
        }

        private IEnumerator RespawnAfterSeconds(int v, GameObject explosion)
        {
            yield return new WaitForSeconds(v);
            rocketControl.transform.position = Vector3.zero;
            rocketControl.transform.rotation = Quaternion.Euler(0, 0, -45);

            camera.enabled = false;
            camera.enabled = true;
            //worldGenerator.SpawnNewWorld();
            levelStats.CalculateTimeToDeath();
            TunnelrunnerDifficultySetter.CalculateNewDifficulty(levelStats);
            levelStats = new LevelStats();
            TunnelrunnerLevelSelector.instance.SpawnStandardWorld(TunnelrunnerDifficultySetter.GetDifficultySettings());
            TimeControl.instance.ResetStandardTime();
            TimeControl.instance.ScaleStandardTime(0.5f, 1);
            Destroy(explosion);
            rocketControl.Respawn();
            levelStats.startTime = DateTime.Now;
        }



        public override int GetScore()
        {
            return maxScore;
        }
    }
}