using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Cynteract.Platformer
{
    public class Train_engine: MonoBehaviour
    {
        public MovingPlatform platform;
        public Transform start, end;
        public float train_stop_duration=3;
        private float train_stop_timer=0;
        float startTime;
        public float speed = 1;

        bool trainStoped=false;
        bool backwards=true;
        bool forwards=false;
        float last_position=0;
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(start.position, end.position);
        }

        void Start()
        {
            startTime = Time.fixedTime;
        }

        private void FixedUpdate()
        {
            if(trainStoped){
                train_stop_timer-=0.01f;
                if(train_stop_timer<0){
                    trainStoped=false;
                }
            }else{
                float t = Time.fixedTime - startTime;
                float velocityLength = Mathf.Cos(t * speed) * 0.5f * speed;
                platform.velocity = (end.position - start.position) * velocityLength;
                platform.transform.position = Vector3.Lerp(start.position, end.position, (Mathf.Sin(t * speed) + 1) * 0.5f);

                if(backwards){
                    if(platform.transform.position.x>last_position){
                        //trainStoped=true;
                        train_stop_timer=train_stop_duration;
                        backwards=false;
                        forwards=true;
                    }
                }else if(forwards){
                    if(platform.transform.position.x<last_position){
                        //trainStoped=true;
                        train_stop_timer=train_stop_duration;
                        backwards=true;
                        forwards=false; 
                    }
                }
                last_position=platform.transform.position.x;   
            }
        }
    }
}