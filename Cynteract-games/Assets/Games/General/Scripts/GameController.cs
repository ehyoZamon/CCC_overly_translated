using Cynteract.CynteractInput;
using Cynteract.Sequence;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Cynteract
{
    public abstract class GameController : MonoBehaviour
    {
        public static GameController gameControllerInstance;
        public CInput cInput;
        public Sequence.SequenceMono sequence;
        public bool shouldStartTutorial;
        public bool switchGame;
        [ReadOnly, ShowInInspector]
        protected int highscore;

        public TMPro.TextMeshProUGUI highscoreTextMesh;
        public void SetUpInput(CGameAxisInfo[] axes, CGameActionInfo[] actions)
        {
            for (int i = 0; i < cInput.axes.Length; i++)
            {
                cInput.axes[i].axis = axes[i].axis;
                cInput.axes[i].inverted = axes[i].inverted;
            }
            for (int i = 0; i < cInput.actions.Length; i++)
            {
                cInput.actions[i].action = actions[i].action;
                cInput.actions[i].inverted = actions[i].inverted;
            }

        }
        public void SetUpInput(InputSet inputset)
        {
            SetUpInput(inputset.axes, inputset.actions);
        }
        public virtual void StartGame()
        {
            sequence.StartSequence();
        }
        public virtual void StartGameWithoutGlove()
        {
            sequence.StartSequence(typeof(WaitForGloveSequence), typeof(GloveResetSequence));
        }
        protected virtual void Awake()
        {
            gameControllerInstance = this;
            OnAwake();
        }

        protected virtual void OnAwake()
        {
        }

        public abstract int GetScore();

        public void UpdateHighscore(int score)
        {
            if (score>highscore)
            {
                SetHighScore(score);
            }
        }

        public void SetHighScore(int v)
        {
            highscore = v;
            highscoreTextMesh.text = $"{Lean.Localization.LeanLocalization.GetTranslationText("Your Highscore")}: {highscore}";
        }
    }
}