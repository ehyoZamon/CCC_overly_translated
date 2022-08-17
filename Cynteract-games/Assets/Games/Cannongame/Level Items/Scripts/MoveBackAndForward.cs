using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityStandardAssets.Utility
{
    public class MoveBackAndForward : MonoBehaviour
    {
        public float SecsToTurn;
        public Vector3andSpace moveUnitsPerSecond;
        public Vector3andSpace rotateDegreesPerSecond;
        public bool ignoreTimescale;
        private float m_LastRealTime;
        // Use this for initialization
        void Start()
        {
            StartCoroutine(Move(SecsToTurn, moveUnitsPerSecond));
        }

        // Update is called once per frame
        void Update()
        {
            float deltaTime = Time.deltaTime;
            if (ignoreTimescale)
            {
                deltaTime = (Time.realtimeSinceStartup - m_LastRealTime);
                m_LastRealTime = Time.realtimeSinceStartup;
            }
            transform.Translate(moveUnitsPerSecond.value * deltaTime, moveUnitsPerSecond.space);
            transform.Rotate(rotateDegreesPerSecond.value * deltaTime, moveUnitsPerSecond.space);
        }
        IEnumerator Move(float secs, Vector3andSpace moveUnitsPerSecond)
        {
            moveUnitsPerSecond.value = moveUnitsPerSecond.value * (-1);
            yield return new WaitForSeconds(secs);
            StartCoroutine(Move(SecsToTurn, moveUnitsPerSecond));
        }

        [Serializable]
        public class Vector3andSpace
        {
            public Vector3 value;
            public Space space = Space.Self;
        }
    }
}
