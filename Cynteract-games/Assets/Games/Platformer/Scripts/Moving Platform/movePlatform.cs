using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movePlatform : MonoBehaviour
{   
   public float speed=10f;

   void FixedUpdate(){
        transform.Translate(Vector2.left*speed*Time.fixedDeltaTime,Space.Self);
   }

   void OnCollisionEnter2D(Collision2D col){
        speed=-speed;
   }
}
