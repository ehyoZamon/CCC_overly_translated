using Cynteract.CGlove;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplayAngleTester : MonoBehaviour
{
    public float thumbBase, thumbCenter, thumbTop,
        indexBase, indexCenter,
        middleBase, middleCenter,
        ringBase, ringCenter,
        pinkyBase, pinkyCenter
        ;
    public float sum;

    // Start is called before the first frame update
    void Start()
    {
        Glove.Any.Subscribe(OnData);
    }

    private void OnData(Glove glove)
    {
        var data = glove.data;
        thumbBase=data[Fingerpart.thumbBase].splitValues.yAngle;
        thumbCenter=data[Fingerpart.thumbCenter].splitValues.yAngle;
        thumbTop=data[Fingerpart.thumbTop].splitValues.yAngle;

        indexBase = data[Fingerpart.indexBase].splitValues.yAngle;
        indexCenter = data[Fingerpart.indexCenter].splitValues.yAngle;

        middleBase = data[Fingerpart.middleBase].splitValues.yAngle;
        middleCenter = data[Fingerpart.middleCenter].splitValues.yAngle;

        ringBase = data[Fingerpart.ringBase].splitValues.yAngle;
        ringCenter = data[Fingerpart.ringCenter].splitValues.yAngle;
        
        pinkyBase=data[Fingerpart.pinkyBase].splitValues.yAngle;
        pinkyCenter = data[Fingerpart.pinkyCenter].splitValues.yAngle;


        sum = (thumbCenter - pinkyCenter);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
