using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotatePlatform : MonoBehaviour
{
    public float rotateAngle=30f;
    public float rotateSpeed=0.1f;
    private float localAngle=0;
    void FixedUpdate(){
        localAngle+=rotateSpeed;
        if(localAngle>rotateAngle){
            rotateSpeed*=(-1);
        }

        if(localAngle<-rotateAngle){
            rotateSpeed*=(-1);
        }


        transform.Rotate(0,0,rotateSpeed);
        Debug.Log(-transform.rotation.eulerAngles.z);
    }
}
