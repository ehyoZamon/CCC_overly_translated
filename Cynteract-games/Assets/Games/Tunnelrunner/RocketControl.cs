using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;
using System;
using Cynteract.CGlove;
using Cynteract.CCC;

namespace Cynteract.Tunnelrunner
{
    public class RocketControl : MonoBehaviour
    {
       new  Rigidbody2D rigidbody;
        public float upwardsVelocity;
        public float rightVelocity;
        public float acceleration = 9.81f;
        public Booster boosters;
        public ParticleSystem explosion;
        public Collider2D boosterCollider;
        bool justDown, justUp;
        public TempSettings tempSettings = new TempSettings();
        public LayerMask environment;
        public AudioSource boosterAudioSource, fireAudioSource;
        public AudioSource hitAudioSource;
        public float UpwardsVel {
            get; private set;
        }

        public class TempSettings
        {
            public bool control, inverted, invincible;
            public TempSettings()
            {
                control = true;
                inverted = false;
                invincible = false;
            }

        }

        public float GetCeillingDistance()
        {
            return GetDistaceToWall(new Vector2(1,1));

        }
        public float GetFloorDistance()
        {

            return GetDistaceToWall(new Vector2(1,-1));
        }
        private float GetDistaceToWall(Vector2 direction)
        {
            
            List<float> distances = new List<float>();

            var cols = Physics2D.OverlapAreaAll(transform.position,  new Vector2(direction.x*1000, direction.y * 1000), environment);
            if (cols.Length == 0)
            {
                return float.MaxValue;
            }
            foreach (var item in cols)
            {
                Vector2 end = item.ClosestPoint(transform.position);
                Debug.DrawLine(transform.position, end, Color.red);
                distances.Add(Vector2.Distance(transform.position, end));
            }
            return Mathf.Min(distances.ToArray());
        }

        public void Invert()
        {
            tempSettings.inverted = !tempSettings.inverted;
        }
        private void Awake()
        {
            rigidbody = GetComponent<Rigidbody2D>();
            Tunnelrunner.rocketControl = this;
        }

        private void FixedUpdate()
        {
            if (tempSettings.control)
            {
                if (Boosters())
                {

                    if (Cynteract.Database.DatabaseManager.instance?.GetSettings().useVibration??true)
                    {

                        CGlove.Glove.Any.VibrateAll(.5f);
                    }

                    if (justDown)
                    {

                        CameraShaker.Instance.Shake(CameraShakePresets.SmallExplosion);
                        justDown = false;

                    }
                    CameraShaker.Instance.Shake(CameraShakePresets.LowVibration);

                    justUp = true;
                    UpwardsVel = Mathf.Min(upwardsVelocity, rigidbody.velocity.y + Time.fixedDeltaTime * acceleration);
                    Boost();
                    if (!GameSettings.GetBool(CSettings.simpleGraphics))
                    {
                      //  rigidbody.AddTorque(-3f * Time.fixedDeltaTime);
                    }

                }
                else
                {
                    CGlove.Glove.Any.VibrateStop();

                    if (justUp)
                    {
                        justUp = false;
                        CameraShaker.Instance.Shake(CameraShakePresets.Falloff);
                    }
                    justDown = true;
                    UpwardsVel = Mathf.Max(-upwardsVelocity, rigidbody.velocity.y - Time.fixedDeltaTime * acceleration);
                    if (!GameSettings.GetBool(CSettings.simpleGraphics))
                    {
                       // rigidbody.AddTorque(3f * Time.fixedDeltaTime);

                    }
                    StopBoosting();
                }
                if (!GameSettings.GetBool(CSettings.simpleGraphics))
                {
               // rigidbody.AddTorque(-1 * (rigidbody.rotation + 45) * Time.fixedDeltaTime);

                }
                //rigidbody.AddTorque(rigidbody.rotation - 45);
                rigidbody.velocity = new Vector3(rightVelocity, UpwardsVel, 0);
            }
            else
            {
                if (GameSettings.GetBool(CSettings.simpleGraphics))
                {

                }
                else
                {
                    rigidbody.angularVelocity = 1000f;
                    rigidbody.freezeRotation = false;
                }
            }

        }

        private void StopBoosting()
        {
            boosters.StopBoosting();
            StopAudioSource(boosterAudioSource);
            StopAudioSource(fireAudioSource);
        }

        private void StopAudioSource(AudioSource audioSource)
        {
            if (audioSource.isPlaying)
            {
                if (audioSource.volume > 0)
                {
                    audioSource.volume -= Time.deltaTime * 5;
                }
                else
                {
                    audioSource.Stop();

                }


            }
        }

        private void Boost()
        {
            boosters.Boost();
            StartAudioSource(boosterAudioSource);
            StartAudioSource(fireAudioSource);
        }

        private void StartAudioSource (AudioSource boosterAudioSource)
        {
            boosterAudioSource.volume = 1;
            if (!boosterAudioSource.isPlaying)
            {
                boosterAudioSource.Play();
            }
        }

        private  bool Boosters()
        {
            return TunnelrunnerInput.GetAction(TunnelrunnerInput.boost)^tempSettings.inverted;
        }

        IEnumerator ExplodeAfterSeconds(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            CameraShaker.Instance.Shake(CameraShakePresets.Explosion);
            var expl=Instantiate(explosion, transform.position, Quaternion.identity);
            yield return new WaitForSeconds(.2f);
            var em = expl.emission;
            em.enabled = false;

            Tunnelrunner.instance.Respawn(expl.gameObject);
            gameObject.SetActive(false);
        }
        internal void Destruction()
        {
            hitAudioSource.Play();
            if (tempSettings.invincible)
            {
                return;
            }
            if (tempSettings.control)
            {
                tempSettings.control = false;
                Boost();
                StartCoroutine(ExplodeAfterSeconds(1));
            }

        }
        public void Respawn()
        {
            gameObject.SetActive(true);
            tempSettings = new TempSettings();
            rigidbody.freezeRotation = true;

        }
    }
}
