using Cynteract.CynteractInput;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cynteract.Sequence
{
    public class GloveResetSequence : SequenceElement
    {
        public bool collectData = true;
        Action onElementFinish;
        public IEnumerator StartSequence(Action onElementFinish)
        {
            this.onElementFinish = onElementFinish;
            return Reset();
        }

        private IEnumerator Reset()
        {
            GloveCalibration.instance.StartResetting(collectData, onElementFinish);
            yield return null;
        }
    }
}