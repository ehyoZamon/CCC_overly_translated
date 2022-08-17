using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Cynteract.Sequence
{
    public class SequenceMono : SerializedMonoBehaviour
    {
        private Type[] filters;
        public bool autostart;
        [Space]
        [TypeFilter("GetFilteredTypeList")]
        public SequenceElement[] sequenceElements=new SequenceElement[0];
        private void Start()
        {
            if (autostart)
            {
                StartSequence();
            }
        }
        public void StartSequence()
        {
            print("Sequence Started"); 
            StartSequence(new Type[] { });
        }
        public void StartSequence(params Type [] filters)
        {
            this.filters = filters;
            if (sequenceElements != null )
            {
                OnElementFinish(0);
            }
        }
        public void OnElementFinish(int index)
        {
            if (sequenceElements.Length > index)
            {
                if (filters.Contains<Type>( sequenceElements[index].GetType()))
                {
                    OnElementFinish(index + 1);
                }
                else
                {
                    var coroutine = sequenceElements[index].StartSequence(() => OnElementFinish(index + 1));
                    StartCoroutine(coroutine);
                }

            }
        }
        public IEnumerable<Type> GetFilteredTypeList() {
            var q = typeof(SequenceElement).Assembly.GetTypes()
                .Where(x => !x.IsAbstract)
                .Where(x => !x.IsGenericTypeDefinition)
                .Where(x => typeof(SequenceElement).IsAssignableFrom(x));
            return q;
        }
    }
}