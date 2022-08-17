using Cynteract.CCC;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Cynteract.Sphererunner
{
    public class SphereMovement : MonoBehaviour
    {
        private const float minScaleDifference = .1f;
        public float velocity;
        #region Components
        new Rigidbody2D rigidbody;
         CircleCollider2D circleCollider;
        #endregion
        public AudioSource hitAudioSource;
        public  float maxScale;
        public  float minScale;
        public LayerMask layerMask;
        public static SphereMovement instance;
        public float Scale01
        {
            get; private set;
        }
        [ReadOnly]
        public float autoDirection = 1;
        private void Awake()
        {
            rigidbody = GetComponent<Rigidbody2D>();
            circleCollider = GetComponent<CircleCollider2D>();
            instance = this;
        }
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void FixedUpdate()
        {
            float direction;
            if (GameSettings.GetBool(CSettings.Sphererunner.autoMove))
            {
                direction = autoDirection;
            }
            else
            {
                direction = 0;
              //  direction = SphereRunnerInput.GetAxis(SphereRunnerInput.movement)*2-1;    
            }
            rigidbody.angularVelocity = -direction * velocity * 1 / transform.lossyScale.y;
            float newRadius = Mathf.Lerp(minScale, maxScale, SphereRunnerInput.GetAxis(SphereRunnerInput.size));
            Vector3 newScale = transform.lossyScale;
            if (CanScale(newRadius))
            {
                newScale=GetScale(newRadius);
            }
            else
            {
                var upper = newRadius;
                var lower = transform.localScale.y;
                var newRad2 = (lower+upper)/2;
                int i = 0;
                do
                {
                    i++;
                    if (CanScale(newRad2))
                    {
                        lower = newRad2;
                        newRad2 = (lower + upper) / 2;
                        if (upper - lower < minScaleDifference)
                        {
                            break;
                        }
                        
                    }
                    else
                    {
                        upper = newRad2;
                        newRad2 = (lower + upper) / 2;
                    }
                } while (i<100);
                newScale= GetScale(newRad2);
            }
            Scale01 = Mathf.InverseLerp(minScale, maxScale, newScale.x);
            transform.localScale = Vector3.Lerp(transform.localScale,newScale, 10*Time.fixedDeltaTime );

            Sphererunner.instance.SetDistance(transform.position.x);
        }
        private bool CanScale(float scaleValue)
        {
            if (scaleValue<=0)
            {
                return false;
            }
            var colliders = Physics2D.OverlapCircleAll(transform.position, scaleValue * circleCollider.radius);
            List<Vector3> points = new List<Vector3>();
            foreach (var item in colliders)
            {
                points.Add(item.ClosestPoint(transform.position));
            }
            float maxAngle = 0;
            foreach (var item in points)
            {
                foreach (var item2 in points)
                {
                    var v1 = item - transform.position;
                    var v2 = item2 - transform.position;

                    var angle = Vector3.Angle(v1, v2);
                    if (angle > maxAngle)
                    {
                        maxAngle = angle;
                    }
                }
                Debug.DrawLine(transform.position, item, Color.red);
            }
            if (maxAngle <= 150 || scaleValue < transform.lossyScale.y)
            {
                return true;
            }
            return false;
        }

        public void Spawn(Vector3 vector3)
        {
            transform.position = vector3;
        }

        private Vector3 GetScale(float scaleValue)
        {

                return scaleValue * Vector3.one;
        }
        private void ScaleSelfRays(float scaleValue)
        {
            bool canGrowBiggerUp = false;
            bool canGrowBiggerRight = false;
            Ray2D[] checkRays = new Ray2D[8];
            checkRays[0] = new Ray2D(transform.position + -transform.lossyScale.x / 4 * transform.right, transform.up);
            checkRays[1] = new Ray2D(transform.position + transform.lossyScale.x / 4 * transform.right, transform.up);
            checkRays[2] = new Ray2D(transform.position + -transform.lossyScale.x / 4 * transform.right, -transform.up);
            checkRays[3] = new Ray2D(transform.position + transform.lossyScale.x / 4 * transform.right, -transform.up);


            checkRays[4] = new Ray2D(transform.position + -transform.lossyScale.x / 4 * transform.up, transform.right);
            checkRays[5] = new Ray2D(transform.position + transform.lossyScale.x / 4 * transform.up, transform.right);
            checkRays[6] = new Ray2D(transform.position + -transform.lossyScale.x / 4 * transform.up, -transform.right);
            checkRays[7] = new Ray2D(transform.position + transform.lossyScale.x / 4 * transform.up, -transform.right);


            for (int i = 0; i < checkRays.Length; i++)
            {
                Debug.DrawLine(checkRays[i].origin, checkRays[i].origin + scaleValue * checkRays[i].direction, Color.red, Time.deltaTime);
                RaycastHit2D hit = Physics2D.Raycast(checkRays[i].origin, checkRays[i].direction, scaleValue, layerMask);
                if (hit.collider == null)
                {
                    if (i < 4)
                    {
                        canGrowBiggerUp = true;
                    }
                    else
                    {
                        canGrowBiggerRight = true;
                    }

                }
            }
            if (canGrowBiggerRight && canGrowBiggerUp || scaleValue < transform.lossyScale.y)
            {
                transform.localScale = scaleValue * Vector3.one;
            }
        }
        private void OnCollisionEnter2D(Collision2D collision)
        {
            float magnitude = collision.relativeVelocity.magnitude;
            if (magnitude > 1f)
            {
                if (!hitAudioSource.isPlaying)
                {
                    hitAudioSource.pitch = Mathf.InverseLerp(maxScale, minScale, transform.lossyScale.x)*0.7f + .7f;

                    //hitAudioSource.volume = Mathf.Clamp01((magnitude + 1) / 20);
                    hitAudioSource.Play();
                }                
                if (Cynteract.Database.DatabaseManager.instance.GetSettings().useVibration)
                {
                    if (magnitude<2)
                    {
                    CGlove.Glove.Any.VibrateAll(CGlove.VibrationPattern.ShortBump);

                    }
                    else if (magnitude < 4)
                    {
                        CGlove.Glove.Any.VibrateAll(CGlove.VibrationPattern.MediumBump);

                    }
                    else
                    {
                        CGlove.Glove.Any.VibrateAll(CGlove.VibrationPattern.LongBump);

                    }
                }
            }
        }
    }
}