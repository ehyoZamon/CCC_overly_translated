using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Cynteract.Platformer
{
    public class JumpAndRun : GameController
    {
        public static JumpAndRun instance;
        public Scorer levelScorer, totalScorer;
        public int levelScore, totalScore;
        
        protected override void Awake()
        {
            base.Awake();
            instance = this;
        }
        private void Start()
        {
            levelScorer.ResetScore();
            totalScorer.ResetScore();
        }
        public void AddScore(int score, Transform iten)
        {
            levelScore += score;
            levelScorer.Score(score, levelScore, iten);
        }
        public void ResetLevelScore()
        {
            levelScore = 0;
            levelScorer.ResetScore();
        }
         

        public void SpawnNextLevel()
        {
            StartCoroutine(SpawnNextLevelCoroutine());
        }

        private IEnumerator SpawnNextLevelCoroutine()
        {
            levelScore += 1000;
            levelScorer.Score(1000, levelScore);
            while (levelScore > 0)
            {
                levelScore -= 100;
                totalScore += 100;
                levelScorer.Score(-100, levelScore);
                totalScorer.Score(100, totalScore, PlayerMovement.instance.transform);
                yield return new WaitForSeconds(.1f);
            }
            base.UpdateHighscore(totalScore);
            levelScorer.ResetScore();
            yield return new WaitForSeconds(1);
            LevelController.instance.SpawnNextLevel();

        }

        public override int GetScore()
        {
            return totalScore;
        }
    }
}