using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Cynteract.Sequence
{
    public interface  SequenceElement
    {
        IEnumerator StartSequence(Action onElementFinish);
    }
}
