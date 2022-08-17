#if UNITY_EDITOR
using Cynteract.CGlove.LinearCalibration;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterpolationTester : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        print(InterpolationManager.left.GetAngle(Cynteract.CGlove.Fingerpart.thumbBase, Cynteract.CGlove.GloveData.SplitValues.FloatValue.X));
    }
}
#endif