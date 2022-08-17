using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Cynteract.Platformer
{
    public class PlatformMover : MonoBehaviour
    {
        public MovingPlatform platform;
        public Transform start, end;
        float startTime;
        public float speed = 1;
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(start.position, end.position);
        }
        void Start()
        {
            startTime = Time.fixedTime;
        }
        private void FixedUpdate()
        {
            float t = Time.fixedTime - startTime;
            platform.transform.position = Vector3.Lerp(start.position, end.position, (Mathf.Sin(t * speed) + 1) * 0.5f);
            float velocityLength = Mathf.Cos(t * speed) * 0.5f * speed;
            platform.velocity = (end.position - start.position) * velocityLength;
        }
    }
}