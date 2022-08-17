using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Cynteract.RoadCrosser
{
    public class RoadCrossing : GameController
    {
        public int score, maxScore;
        public TextMeshProUGUI scoreTextMesh;
        public static RoadCrossing instance;

        public void ScorePoints(int points)
        {
            score += points;
            if (score>maxScore)
            {
                maxScore = score;
                UpdateHighscore(score);
            }
            scoreTextMesh.text = $"{Lean.Localization.LeanLocalization.GetTranslationText("Score")}: {score}";
        }
        public void ResetScore()
        {
            score = 0;
        }
        protected override void OnAwake()
        {
            instance = this;
        }
        public override int GetScore()
        {
            return maxScore;
        }
    }
}