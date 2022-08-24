using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
namespace Cynteract.Platformer
{
    public class Train_engine: MonoBehaviour
    {
        public MovingPlatform platform;
        public Transform start, end;
        public float train_stop_duration=3;
        public GameObject[] wheels;
        public TMP_Text[] time_terminals;
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
            for(int k=0;k<time_terminals.Length;k++){
                time_terminals[k].text=System.DateTime.Now+"";
            }

    
            float t = Time.fixedTime - startTime;
            float velocityLength = Mathf.Cos(t * speed) * 0.5f * speed;
            platform.velocity = (end.position - start.position) * velocityLength;
            
            if(trainStoped){
                train_stop_timer-=0.01f;
                if(train_stop_timer<0){
                    if(Math.Abs(platform.transform.position.x-Vector3.Lerp(start.position, end.position, (Mathf.Sin(t * speed) + 1) * 0.5f).x)<0.2){
                        trainStoped=false;
                    }
                }
            }else{
                platform.transform.position = Vector3.Lerp(start.position, end.position, (Mathf.Sin(t * speed) + 1) * 0.5f);
                if(backwards){
                    for(int i=0;i<wheels.Length;i++){
                        wheels[i].GetComponent<Transform>().Rotate(0,0,-velocityLength*100,Space.Self);
                    }
                    if(platform.transform.position.x>last_position){
                        //trainStoped=true;
                        train_stop_timer=train_stop_duration;
                        backwards=false;
                        forwards=true;
                    }
                }else if(forwards){
                    for(int i=0;i<wheels.Length;i++){
                        wheels[i].GetComponent<Transform>().Rotate(0,0,velocityLength*100,Space.Self);
                    }
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