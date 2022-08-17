using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cynteract.Tunnelrunner
{
    public abstract class Collectible : MonoBehaviour
    {
        public int points;
        public GameObject particlePrefab;
        public AudioSource audioSource;
        public bool playerCollision = true;
        public bool boosterCollision = false;
        private bool activated;
        public abstract void Effect();
        private void Awake()
        {

        }
        public virtual void ScorePoints()
        {
            Tunnelrunner.instance.ScorePoints(points, transform);
        }
        public virtual void OnTriggerEnter2D(Collider2D other)
        {
            TriggerHit(other);

        }

        protected void TriggerHit(Collider2D other)
        {

            if ((other.gameObject.tag == "Player"&&playerCollision)|| (other.gameObject.tag=="Booster"&&boosterCollision))
            {
                if (activated)
                {
                    return;
                }
                activated = true;
                ScorePoints();
                if (audioSource)
                {
                    PlaySound();
                }
                if (particlePrefab)
                {
                    SpawnParticles();
                }

                Effect();
                Vibration();
                Destroy(gameObject);
            }
        }

        private void SpawnParticles()
        {
            if (GameSettings.SimpleGraphics)
            {
                return;
            }
            Instantiate(particlePrefab, transform.position, Quaternion.identity);
        }

        protected virtual void Vibration()
        {
        }

        private void PlaySound()
        {
            var source=Instantiate(audioSource, transform.position, Quaternion.identity);
            int scoreStreak = Tunnelrunner.instance.GetScoreStreak();
            print(scoreStreak);
            source.pitch = Mathf.Clamp01((float)scoreStreak / 10f)/2+1;
        }
    }
}