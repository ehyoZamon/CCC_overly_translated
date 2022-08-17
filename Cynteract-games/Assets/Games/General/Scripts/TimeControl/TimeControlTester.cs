using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Cynteract.CTime
{
    public class TimeControlTester : MonoBehaviour
    {
        [Button]
        public void Pause()
        {
            TimeControl.instance.ScaleTime(TimeControl.Priority.Menu, 0);
        }
        [Button]
        public void UnPause()
        {
            TimeControl.instance.ResetTime(TimeControl.Priority.Menu);
        }

        [Button]
        public void ScaleTime(float scale, float duration)
        {
            TimeControl.instance.ScaleTime(TimeControl.Priority.Standard, scale, duration);
        }
        [Button]
        public void ScaleTime(float scale)
        {
            TimeControl.instance.ScaleTime(TimeControl.Priority.Standard, scale);
        }
        [Button]
        public void WaitTest()
        {
            StartCoroutine(WaitForSeconds(TimeControl.Priority.Menu));
            StartCoroutine(WaitForSeconds(TimeControl.Priority.Standard));
        }
        IEnumerator WaitForSeconds(TimeControl.Priority priority)
        {
            yield return new WaitForSecondsPrioritized(1, priority);
            print("Priority " + priority + " finished");
        }
    }
}