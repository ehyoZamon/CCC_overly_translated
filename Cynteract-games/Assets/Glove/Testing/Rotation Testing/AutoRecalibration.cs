using Cynteract.CGlove;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRecalibration : MonoBehaviour
{
    [Range(0,1)]
    public float value;
    public float [] maxima=new float[8], minima = new float[8];
    BitArray sensorsCalibrated=new BitArray(8);
    public bool calibrate;

    private void Start()
    {
        Glove.Any.Subscribe(Callback);
    }
    
    private void Callback(Glove data)
    {
        float maximum = 0, minimum = 0, sum = 0;
        AddFingerPart(Fingerpart.indexBase, 0);
        AddFingerPart(Fingerpart.indexCenter, 1);
        AddFingerPart(Fingerpart.middleBase, 2);
        AddFingerPart(Fingerpart.middleCenter, 3);
        AddFingerPart(Fingerpart.ringBase, 4);
        AddFingerPart(Fingerpart.ringCenter, 5);
        AddFingerPart(Fingerpart.pinkyBase, 6);
        AddFingerPart(Fingerpart.pinkyCenter, 7);
        void AddFingerPart(Fingerpart fingerpart, int index)
        {
            var state = data.information.GetSensorState(fingerpart);
            if (state == ImuState.NotConnected || state == ImuState.Error)
            {
                return;
            }
            else
            {
                if (calibrate)
                {
                    maxima[index] = Mathf.Max(maxima[index], data.data.GetBendAngle(fingerpart));
                    minima[index] = Mathf.Min(minima[index], data.data.GetBendAngle(fingerpart));
                    sensorsCalibrated[index] = true;
                }
                if (sensorsCalibrated[index])
                {
                    maximum += maxima[index];
                    minimum += minima[index];
                    sum += data.data.GetBendAngle(fingerpart);
                }

            }
        }
        value = Mathf.InverseLerp(minimum, maximum, sum);
    }
}
