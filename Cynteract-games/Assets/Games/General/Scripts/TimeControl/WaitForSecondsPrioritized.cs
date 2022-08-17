using UnityEngine;
using static Cynteract.CTime.TimeControl;

namespace Cynteract.CTime
{

        public class WaitForSecondsPrioritized : CustomYieldInstruction
        {
            public float seconds;
            public Priority priority;
            private float startTime;

            public WaitForSecondsPrioritized(float seconds, Priority priority)
            {
                this.seconds = seconds;
                this.priority = priority;
                startTime = TimeControl.instance.GetPriorityTimeGroup(priority).fixedTime;
            }

            public override bool keepWaiting
            {
                get
                {
                    return TimeControl.instance.GetPriorityTimeGroup(priority).fixedTime-startTime<seconds;
                }
            }
        }
    
}