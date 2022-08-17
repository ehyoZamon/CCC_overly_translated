using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
namespace Cynteract.Sphererunner
{
    public class Sphererunner : GameController
    {
        public static int levelsPlayed=0;
        public bool quitAtLevelEnd;
        public SphererunnerTutorial sphererunnerTutorial;
        public static Sphererunner instance;
        public TextMeshProUGUI scoreTextMesh;
        private int score;

        protected override void OnAwake()
        {
            instance = this;
        }
        public override int GetScore()
        {
            return score;   
        }
        public void Init()
        {
            if (shouldStartTutorial)
            {
                WorldGenerator.instance.SpawnNewLevelWithTutorial();
                sphererunnerTutorial.StartTutorial();
            }
            else
            {
                WorldGenerator.instance.SpawnNewLevel();
            }
        }
        public void NextLevel()
        {
            if (quitAtLevelEnd)
            {
                switchGame = true;
            }
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.N))
            {
                switchGame = true;
            }
        }

        public void SetDistance(float x)
        {
            score = (int)x * 5;
            scoreTextMesh.text = $"{Lean.Localization.LeanLocalization.GetTranslationText("Distance")}: {score} m";
            UpdateHighscore(score);
        }
    }
}