using Cynteract.CynteractInput;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cynteract.Sequence {
    public class CalibrationSequence : SequenceElement
    {
        Action onElementFinish;
        public IEnumerator StartSequence(Action onElementFinish)
        {
            this.onElementFinish = onElementFinish;
            return CalibrateAll();
        }
        void CalibrateNextAxis(int index)
        {
            if (CInput.instance.axes.Length>index)
            {
                GloveCalibration.instance.StartCalibration(index, CynteractInput.Type.Axis, ()=>CalibrateNextAxis(index+1));
            }
            else
            {
                CalibrateNextAction(0);
            }
            
        }
        void CalibrateNextAction(int index)
        {
            if (CInput.instance.actions.Length > index)
            {
                GloveCalibration.instance.StartCalibration(index, CynteractInput.Type.Action, () => CalibrateNextAction(index+1));
            }
            else
            {
                onElementFinish();
            }
        }
        private IEnumerator CalibrateAll()
        {
            CalibrateNextAxis(0);
            yield return null;
        }
    }
}