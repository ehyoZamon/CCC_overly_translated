using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Cynteract.Hitzestrom
{
    public class PaperjetControl : PaperJetControlAbstract
    {
        new Rigidbody rigidbody;

        public Transform plane, left, right;
        private void Awake()
        {
            rigidbody = GetComponent<Rigidbody>();
        }
        private void FixedUpdate()
        {
            
            var y = rigidbody.velocity.y;
            if (inHeat)
            {
                y = 20;
            }
            else
            {
                y = -5;
            }
            rigidbody.velocity = y * Vector3.up + 10f * transform.forward;

            var planeDestination = Vector3.Lerp(left.position,right.position,HitzeStromInput.GetAxis(HitzeStromInput.leftRight));
            plane.position = planeDestination;
            
        }
    }
}