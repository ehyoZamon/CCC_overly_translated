using Cynteract.CGlove;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GloveRotationCalibration : SerializedMonoBehaviour
{
    public abstract float GetAngle(Fingerpart fingerpart, GloveData.SplitValues.FloatValue value);
    
}
