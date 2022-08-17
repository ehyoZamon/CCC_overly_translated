using Cynteract.CTime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Cynteract.Tunnelrunner {
    public class TunnelrunnerLevelSelector : MonoBehaviour
    {
        public TunnelrunnerTutorial tutorial;
        public LevelSet standardLevel, tutorialLevel;
        public WorldGenerationSettings standardWorldGenerationSettings, tutorialWorldGenerationSettings;
        public static TunnelrunnerLevelSelector instance;
        private void Awake()
        {
            instance = this;
        }
        public void SpawnTutorial()
        {
            tutorial.gameObject.SetActive(true);
            WorldGenerator worldGenerator = Tunnelrunner.instance.worldGenerator;
            worldGenerator.levels = tutorialLevel;
            worldGenerator.worldGenerationSettings = tutorialWorldGenerationSettings;
            worldGenerator.SpawnNewWorld();
            GameSettings.SetFloat(CSettings.gameSpeed,1);

            tutorial.StartTutorial();
        }
        public void SpawnStandardWorld(DifficultySettings difficultySettings)
        {
            WorldGenerator worldGenerator = Tunnelrunner.instance.worldGenerator;
            worldGenerator.levels = standardLevel;
            worldGenerator.worldGenerationSettings = standardWorldGenerationSettings;
            worldGenerator.SpawnNewWorld();
            SetDifficulty(difficultySettings);
            TimeControl.instance.ScaleStandardTime(0.5f, 1);
        }

        public void SetDifficulty(DifficultySettings difficultySettings)
        {
            GameSettings.SetFloat(CSettings.gameSpeed, difficultySettings.timescale);
            Tunnelrunner.rocketControl.acceleration = difficultySettings.acceleration;
            Tunnelrunner.rocketControl.upwardsVelocity = difficultySettings.upwardsVelocity;
            Tunnelrunner.rocketControl.rightVelocity = difficultySettings.rightVelocity;
        }
    }
}