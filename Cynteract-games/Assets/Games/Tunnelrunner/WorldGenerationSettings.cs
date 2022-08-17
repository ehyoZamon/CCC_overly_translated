using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Cynteract.Tunnelrunner
{
    [CreateAssetMenu(menuName = "Tunnelrunner/Settings/WorldGenerationSettings")]
    public class WorldGenerationSettings : ScriptableObject
    {
        [Tooltip("The size of the Tunnel depending on the score divided by divideBy")]
        public AnimationCurve diameterOverScore;
        [Tooltip("The amount of change allowed in the Tunnel depending on the score divided by divideBy")]
        public AnimationCurve absOffsetAllowed;
        public int stepsPerPart;
        public float celling;
        public float bottom;
        public float stepDistance;
        public int interpolationSteps;
        public int itemDistance;
        public int divideBy;
        public float CurrentDiameter()
        {
            return diameterOverScore.Evaluate(ScoreDivided());
        }
        public float CurrentDiameter(float x)
        {
            return diameterOverScore.Evaluate(x);
        }
        public float CurrentOffset(float x)
        {
            return absOffsetAllowed.Evaluate(x);
        }

        public float CurrentOffset()
        {
            return absOffsetAllowed.Evaluate(ScoreDivided());
        }

        private float ScoreDivided()
        {
            return (float)Tunnelrunner.instance.levelStats.score / divideBy;
        }
    }
}