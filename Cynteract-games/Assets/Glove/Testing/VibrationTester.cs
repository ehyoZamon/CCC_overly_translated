using Cynteract.CGlove;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VibrationTester : MonoBehaviour
{
    [Button]
    public void All()
    {
        Glove.Any.VibrateAll(1);
    }
    [Button]
    public void Thumb()
    {
        Glove.Any.Vibrate(Fingerpart.thumbTop, VibrationPattern.MediumStrongClick);
    }
    [Button]
    public void Index()
    {
        Glove.Any.Vibrate(Fingerpart.indexTop, VibrationPattern.MediumStrongClick);
    }

    [Button]
    public void Middle()
    {
        Glove.Any.Vibrate(Fingerpart.middleTop, VibrationPattern.MediumStrongClick);

    }

    [Button]
    public void Ring()
    {
        Glove.Any.Vibrate(Fingerpart.ringTop, VibrationPattern.MediumStrongClick);

    }

    [Button]
    public void Pinky()
    {
        Glove.Any.Vibrate(Fingerpart.pinkyTop, VibrationPattern.MediumStrongClick);

    }
    [Button]
    public void PalmTop()
    {
        Glove.Any.Vibrate(Fingerpart.palmTop, VibrationPattern.LongBump);

    }
    [Button]
    public void PalmBase()
    {
        Glove.Any.Vibrate(Fingerpart.palmBase, VibrationPattern.LongBump);

    }
}
