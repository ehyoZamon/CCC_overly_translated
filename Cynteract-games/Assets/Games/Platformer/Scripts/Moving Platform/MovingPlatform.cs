using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Cynteract.Platformer
{
    public class MovingPlatform : MonoBehaviour
    {
        public Vector2 velocity;

        public void SetStart()
        {
            transform.parent.Find("start").transform.position = this.transform.position;
        }

        public void SetEnd()
        {
            transform.parent.Find("end").transform.position = this.transform.position;
        }
    }
}