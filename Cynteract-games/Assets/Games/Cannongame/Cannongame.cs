using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Cynteract.Cannongame
{
    public class Cannongame: GameController
    {
        public static CannonControl cannonControl;
        public static Cannongame instance;
        public LevelSwitcher LevelSwitcher;
        public new Sequence.SequenceMono sequence;
        public int goalsReached = 0;
        public Cannongame()
        {
            instance = this;
            LevelSwitcher = new LevelSwitcher("Cannongame/Levels");
        }

        public void ReachedGoal()
        {
            goalsReached++;
            UpdateHighscore(goalsReached * 500);
            cannonControl.Clear();
            LevelSwitcher.NextLevel();
            
        }

        protected override void OnAwake()
        {
            LevelSwitcher.InitLevels();
            
        }
        public void Init()
        {
            LevelSwitcher.LoadLevel(0);
        }



        public override int GetScore()
        {
            return goalsReached *500;
        }
    }
}
