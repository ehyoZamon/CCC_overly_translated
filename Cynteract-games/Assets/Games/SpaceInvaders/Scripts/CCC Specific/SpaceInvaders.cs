using Cynteract.Sequence;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using Cynteract.CTime;
using Sirenix.OdinInspector;
using UnityEngine.SocialPlatforms.Impl;

namespace Cynteract.SpaceInvaders
{
    public class SpaceInvaders : GameController
    {
        private const int scoreMultiplier = 50;
        public static SpaceInvaders instance;
        public int money;
        [ShowInInspector]
        [ReadOnly]
        private int moneyAtLastDamage;
        [ShowInInspector]
        [ReadOnly]
        private int moneyAtLastDifficultyIncrease;
        [ShowInInspector]
        [ReadOnly]
        private int moneyAtLastHeart;
        [ShowInInspector]
        [ReadOnly]
        private float timeOfLastDamage;
        private int maxMoney=0;
        [ShowInInspector]
        [ReadOnly]
        private float timeScalePosition=.5f;
        public AnimationCurve timeScaleCurve;
        public SpaceInvadersTutorial tutorial;
        public TextMeshProUGUI scoreText;
        public Scorer scorer;
        public int Money
        {
            get
            {
                return money;
            }
            set
            {
                money = value;
                if (money>maxMoney)
                {
                    maxMoney = money;
                    UpdateHighscore(maxMoney* scoreMultiplier);
                }
            }
        }
        public void AddScore(int amount)
        {
            Money += amount;
            scorer.Score(amount* scoreMultiplier, Money * scoreMultiplier);
        }
        public void AddScore(int amount, Transform item)
        {
            Money += amount;
            scorer.Score(amount * scoreMultiplier, Money * scoreMultiplier, item);
        }
        public void ResetScore()
        {
            Money = 0;
            scorer.Score(0, 0);
            moneyAtLastDifficultyIncrease = 0;
            moneyAtLastDamage = 0;
            timeOfLastDamage = Time.fixedTime;
        }
        public void PlayerDied()
        {
            if (money<20)
            {
                MakeItEasier();
            }
        }
        public void PlayerSpawned()
        {
            SetTimeScale();
        }
        public void PlayerGotDamaged()
        {

            if (money-moneyAtLastDamage<5||Time.fixedTime- timeOfLastDamage<5f||timeScalePosition>.5f)
            {
                MakeItEasier();
            }
            moneyAtLastDamage = money;
            timeOfLastDamage = Time.fixedTime;
        }


        public void PlayerGotMoney()
        {
            if (money - moneyAtLastDamage > 10&& money - moneyAtLastDifficultyIncrease>10)
            {
                MakeItHarder();
                moneyAtLastDifficultyIncrease = money;
            }
            if (Player.instance.lives < 3)
            {
                if (money - moneyAtLastHeart > Player.instance.lives * 20)
                {
                    LevelController.instance.SpawnHeart();
                    moneyAtLastHeart = money;
                }
            }
            else
            {
                moneyAtLastHeart = money;
            }
            int level = money / 50;
            while (level>PlayerShooting.instance.gunsLevel)
            {
                PlayerShooting.instance.IncreaseGunLevel();
            }
        }
        private void MakeItEasier()
        {
            timeScalePosition = Mathf.Clamp01(timeScalePosition - .1f);
            SetTimeScale();
        }



        private void MakeItHarder()
        {
            timeScalePosition = Mathf.Clamp01(timeScalePosition + .1f);
            SetTimeScale();

        }
        private void SetTimeScale()
        {
            float scale = timeScaleCurve.Evaluate(timeScalePosition);
            TimeControl.instance.ScaleTime(TimeControl.Priority.Standard, scale);
            if (Player.instance)
            {

            Player.instance.playerMoving.baseSpeed =  (1 / scale);
            }
        }
        SpaceInvaders()
        {
            instance = this;
        }
        public override int GetScore()
        {
            return maxMoney * scoreMultiplier;
        }
        public void Init()
        {
            if (shouldStartTutorial)
            {
                tutorial.StartTutorial();
                TimeControl.instance.ScaleTime(TimeControl.Priority.Standard, 1f);
            }
            else
            {
                SetTimeScale();
                LevelController.instance.CreateNewWorld();
            }
        }


    }
}
