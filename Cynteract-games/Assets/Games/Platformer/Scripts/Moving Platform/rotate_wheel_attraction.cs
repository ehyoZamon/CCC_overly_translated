using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotate_wheel_attraction : MonoBehaviour
{
    public GameObject pivotObject;
    public float rotationSpeed=1;
    void FixedUpdate(){ 
        transform.RotateAround(pivotObject.transform.position,new Vector3(0,0,1),Time.fixedDeltaTime*rotationSpeed);
    }
}
