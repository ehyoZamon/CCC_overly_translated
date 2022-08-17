using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cynteract.CTime;
namespace Cynteract.Sequence
{


    public class TimePauseSequence : SequenceElement
    {
        Action onElementFinish;
        public bool pause;
        public IEnumerator StartSequence(Action onElementFinish)
        {
            this.onElementFinish = onElementFinish;
            return SetTime();
        }

        private IEnumerator SetTime()
        {
            if (pause)
            {
                TimeControl.instance.MenuPause();
            }
            else
            {
                TimeControl.instance.MenuUnpause();
            }
            onElementFinish();
            yield return null;
        }
    }
}
