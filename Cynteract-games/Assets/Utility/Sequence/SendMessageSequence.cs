using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
namespace Cynteract.Sequence
{
    public class SendMessageSequence :SequenceElement
    {
        public UnityEvent unityEvent=new UnityEvent();
        private Action onElementFinish;

        public IEnumerator StartSequence(Action onElementFinish)
        {
            this.onElementFinish = onElementFinish;
            return ActivateEvent();
        }
        IEnumerator ActivateEvent()
        {
            unityEvent.Invoke();
            onElementFinish();
            yield return null;
        }
    }
}

