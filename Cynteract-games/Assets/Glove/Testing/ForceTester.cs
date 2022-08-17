using Cynteract.CGlove;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ForceTester : MonoBehaviour
{

    public float thumb, index, middle, ring, pinky;
    public Slider thumbSlider, indexSlider, middleSlider, ringSlider, pinkySlider;
    public bool vibrate;
    private void Update()
    {
        thumb = Glove.Any.data.GetForce(Fingerpart.thumbTop);

        index = Glove.Any.data.GetForce(Fingerpart.indexTop);
        middle = Glove.Any.data.GetForce(Fingerpart.middleTop);
        ring = Glove.Any.data.GetForce(Fingerpart.ringTop);
        pinky = Glove.Any.data.GetForce(Fingerpart.pinkyTop);
        if (vibrate)
        {
            Glove.Any.Vibrate(Fingerpart.thumbTop, thumb);
            Glove.Any.Vibrate(Fingerpart.indexTop, index);
            Glove.Any.Vibrate(Fingerpart.middleTop, middle);
            Glove.Any.Vibrate(Fingerpart.ringTop, ring);
            Glove.Any.Vibrate(Fingerpart.pinkyTop, pinky);
        }
        if (thumbSlider)
        {
            thumbSlider.value = thumb;
        }
        if (indexSlider)
        {
            indexSlider.value = index;
        }
        if (middleSlider)
        {
            middleSlider.value = middle;
        }
        if (ringSlider)
        {
            ringSlider.value = ring;
        }
        if (pinkySlider)
        {
            pinkySlider.value = pinky;
        }
    }
}
