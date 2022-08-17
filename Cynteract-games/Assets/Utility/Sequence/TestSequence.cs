using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Cynteract.Sequence
{
    public class TestSequence :SequenceElement
    {
        public int time;
        Action onElementFinish;
        IEnumerator Timer()
        {
            yield return new WaitForSeconds(time);
            Debug.Log("Stopped");
            onElementFinish();
        }

        public  IEnumerator StartSequence(Action onElementFinish)
        {
            Debug.Log("Started");
            this.onElementFinish = onElementFinish;
            return Timer();
        }
    }
}
