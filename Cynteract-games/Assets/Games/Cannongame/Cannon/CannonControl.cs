using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cynteract.CGlove;
namespace Cynteract.Cannongame
{
    public class CannonControl : MonoBehaviour
    {
        public Fire fire;
        public Transform cannon;
        public Transform cannonBallpool;
        float angle;
        public float minAngle, maxAngle;
        private bool fired;
        private bool frozen;

        private void Awake()
        {
            Cannongame.cannonControl = this;
        }
        private void Update()
        {
            if (Time.timeScale==0)
            {
                return;
            }
            if (!frozen && FireAction())
            {
                fire.Shoot();
            }
            angle = CannonInput.GetAxis(CannonInput.aim)*90;
            cannon.rotation = Quaternion.Euler(0, 0, -angle);
        }

        public void Freeze()
        {
            frozen = true;
        }
        public void UnFreeze()
        {
            frozen = false;
        }

        public void Clear()
        {
            for (int i = 0; i < cannonBallpool.childCount; i++)
            {
                Destroy(cannonBallpool.GetChild(i).gameObject);
            }
        }

        private bool FireAction()
        {

            if (!fired)
            {

                bool yes =
                CannonInput.GetAction(CannonInput.shoot);
                fired = yes;
                return yes;
            }
            else
            {
                if (!CannonInput.GetAction(CannonInput.shoot))
                {
                    fired = false;
                }
            }
            return false;
        }
    }
}