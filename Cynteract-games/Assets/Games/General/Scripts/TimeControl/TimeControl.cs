using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using UnityEngine;
namespace Cynteract.CTime
{
    public partial class TimeControl : SerializedMonoBehaviour
    {
        public enum Priority
        {
            Menu = int.MinValue,
            High=-1,
            Standard = 0
        }
        public class TimeModifier
        {

        }
        public class QueueTimeModifier : TimeModifier
        {
            public Queue<CoroutineTimeScaler> queue;

            public QueueTimeModifier()
            {
                queue = new Queue<CoroutineTimeScaler>();
            }
        }


        public class TimeScaler : TimeModifier
        {
            public float scale;

            public TimeScaler(float scale)
            {
                this.scale = scale;
            }
        }


        public class CoroutineTimeScaler : TimeScaler
        {
            public IEnumerator coroutine;
            public enum RunningMode
            {
                Idle, Running, Finished
            }
            public RunningMode Mode
            {
                get; private set;
            }
            public CoroutineTimeScaler(float scale, float duration) : base(scale)
            {
                coroutine = WaitTillFinished(scale, duration);
                Mode = RunningMode.Idle;
            }
            IEnumerator WaitTillFinished(float scale, float duration)
            {
                Mode = RunningMode.Running;
                yield return new WaitForSeconds(scale * duration);
                Mode = RunningMode.Finished;
            }

        }
        public class AbsoluteTimeScaler : TimeScaler
        {


            public AbsoluteTimeScaler(float scale) : base(scale)
            {
            }
        }
        public class PriorityTimeGroup : IComparable<PriorityTimeGroup>
        {
            public Priority priority;
            public TimeModifier modifier;
            public float fixedTime;

            public PriorityTimeGroup(Priority priority, TimeModifier timeModifier)
            {
                this.priority = priority;
                modifier = timeModifier;
                fixedTime = 0;
            }
            public PriorityTimeGroup(Priority priority) : this(priority, null)
            {
            }
            public int CompareTo(PriorityTimeGroup other)
            {
                if (other != null)
                {
                    if ((int)priority < (int)other.priority)
                    {
                        return -1;
                    }
                    else if ((int)priority == (int)other.priority)
                    {
                        return 0;
                    }
                    else
                    {
                        return 1;
                    }
                }
                else
                {
                    throw new ArgumentNullException("Object is not a PriorityTimeGroup or null");
                }
            }
        }



        PriorityQueue<PriorityTimeGroup> priorityQueue = new PriorityQueue<PriorityTimeGroup>();
        [ShowInInspector]
        [ReadOnly]
        public float BaseTime
        {
            get; private set;
        }
        [ShowInInspector]
        [ReadOnly]
        public float StandardFixedDeltaTime
        {
            get; private set;
        }
        [ShowInInspector]
        [ReadOnly]
        public float CurrentTimeScale
        {
            get; private set;
        }
        [ShowInInspector]
        [ReadOnly]
        public float UnbiasedScale
        {
            get; private set;
        }

        public static TimeControl instance;
        public IBaseTime baseTimeGetter;
        public TimeControl()
        {
            BaseTime = 1;
            CurrentTimeScale = 1;
            UnbiasedScale = 1;
            instance = this;
        }
        private void Awake()
        {
            priorityQueue.Enqueue(new PriorityTimeGroup(Priority.Menu));
            priorityQueue.Enqueue(new PriorityTimeGroup(Priority.High));
            priorityQueue.Enqueue(new PriorityTimeGroup(Priority.Standard));
            StandardFixedDeltaTime = Time.fixedDeltaTime;
        }
        private void Update()
        {
            var tempBaseTime = BaseTime;
            BaseTime = baseTimeGetter.GetBaseTime();
            if (BaseTime != tempBaseTime)
            {
                SetTimeScale(UnbiasedScale);
            }
            UpdateScale();
        }

        public void ResetAll()
        {
            foreach (var item in Enum.GetValues(typeof( Priority)))
            {
                ResetTime((Priority)item);
            }
        }

        public void MenuPause()
        {
            ScaleTime(Priority.Menu, 0);
        }
        public void MenuUnpause()
        {
            ResetTime(Priority.Menu);
        }

        public void ScaleStandardTime(float scale, int duration)
        {
            ScaleTime(Priority.Standard, scale, duration);
        }
        public void ResetStandardTime()
        {
            ResetTime(Priority.Standard);
        }
        public void ScaleTime(Priority priority, float scale)
        {
            var priorityTimeGroup = GetPriorityTimeGroup(priority);
            priorityTimeGroup.modifier = new AbsoluteTimeScaler(scale);
        }
        public void ResetTime(Priority priority)
        {
            var priorityTimeGroup = GetPriorityTimeGroup(priority);
            if (priorityTimeGroup!=null)
            {
                priorityTimeGroup.modifier = null;
            }
        }
        public void ScaleTime(Priority priority, float scale, float duration)
        {
            var priorityTimeGroup = GetPriorityTimeGroup(priority);
            if (priorityTimeGroup.modifier == null)
            {
                priorityTimeGroup.modifier = new QueueTimeModifier();
            }
            switch (priorityTimeGroup.modifier)
            {
                case QueueTimeModifier queueTimeModifier:
                    queueTimeModifier.queue.Enqueue(new CoroutineTimeScaler(scale, duration));
                    break;
                default:
                    priorityTimeGroup.modifier = new QueueTimeModifier();
                    var queue = priorityTimeGroup.modifier as QueueTimeModifier;
                    queue.queue.Enqueue(new CoroutineTimeScaler(scale, duration));
                    break;
            }
        }
        public IEnumerator WaitForSeconds(float seconds, Priority priority)
        {
            yield return new WaitForSeconds(seconds);
        }
        private void UpdateScale()
        {
            PriorityQueue<PriorityTimeGroup> nextQueue = new PriorityQueue<PriorityTimeGroup>();
            bool isEmpty = true;
            float currentTimeModifier=BaseTime;
            while (priorityQueue.Count() > 0)
            {
                var priorityTimeGroup = priorityQueue.Dequeue();
                if (isEmpty)
                {
                    switch (priorityTimeGroup.modifier)
                    {
                        case AbsoluteTimeScaler absoluteTimeScaler:
                            isEmpty = false;
                            SetTimeScale(absoluteTimeScaler.scale);
                            currentTimeModifier = absoluteTimeScaler.scale * BaseTime;
                            break;
                        case QueueTimeModifier queueTimeModifier:
                            if (queueTimeModifier.queue.Count > 0)
                            {
                                isEmpty = false;
                                var scaler= UpdateScale(queueTimeModifier);
                                currentTimeModifier = scaler.scale * BaseTime;

                            }

                            break;
                        default:
                            break;
                    }
                }
                priorityTimeGroup.fixedTime += currentTimeModifier * Time.unscaledDeltaTime;
                nextQueue.Enqueue(priorityTimeGroup);
            }
            if (isEmpty)
            {
                SetTimeScale(1);
            }
            priorityQueue = nextQueue;
        }
        private TimeScaler UpdateScale(QueueTimeModifier priorityTimeGroup)
        {
            while (priorityTimeGroup.queue.Count > 0)
            {
                var item = priorityTimeGroup.queue.Peek();
                switch (item)
                {
                    case CoroutineTimeScaler coroutineTimeScaler:
                        switch (coroutineTimeScaler.Mode)
                        {
                            case CoroutineTimeScaler.RunningMode.Idle:
                                SetTimeScale(coroutineTimeScaler.scale);
                                StartCoroutine(coroutineTimeScaler.coroutine);
                                break;
                            case CoroutineTimeScaler.RunningMode.Running:
                                SetTimeScale(coroutineTimeScaler.scale);
                                break;
                            case CoroutineTimeScaler.RunningMode.Finished:
                                priorityTimeGroup.queue.Dequeue();
                                break;
                            default:
                                break;
                        }
                        return coroutineTimeScaler;
                    default:

                        break;
                }
            }
            return null;
        }
        private void SetTimeScale(float scale)
        {
            UnbiasedScale = scale;
            CurrentTimeScale = scale * BaseTime;
            Time.timeScale = scale * BaseTime;
            Time.fixedDeltaTime = StandardFixedDeltaTime * scale * BaseTime;
        }

        public PriorityTimeGroup GetPriorityTimeGroup(Priority priority)
        {
            PriorityQueue<PriorityTimeGroup> nextQueue = new PriorityQueue<PriorityTimeGroup>();
            PriorityTimeGroup queue = null;
            while (priorityQueue.Count() > 0)
            {
                var item = priorityQueue.Dequeue();
                if (item.priority == priority)
                {
                    queue = item;
                }
                nextQueue.Enqueue(item);
            }
            priorityQueue = nextQueue;
            return queue;
        }
    }
}