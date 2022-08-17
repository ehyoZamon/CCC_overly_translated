using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Cynteract.SpaceInvaders
{
    public class Backgroundcontroller : MonoBehaviour
    {
        [Serializable]
        public class MovingStars
        {
            public DirectMoving directMoving;
            private float standardSpeed;

            public void SetStandardSpeed()
            {
                standardSpeed = directMoving.speed;
            }

            public void MoveAtStandardSpeed()
            {
                directMoving.speed = standardSpeed;
            }

            public void StopMoving()
            {
                directMoving.speed = 0;
            }
        }
        public Color blueColor, grayColor;
        public static Backgroundcontroller instance;
        public GameObject stars, speedEffect;
        public MovingStars[] movingStars;
        public new Camera camera;
        private void Awake()
        {
            instance = this;
            camera = Camera.main;
            foreach (var item in movingStars)
            {
                item.SetStandardSpeed();
            }
        }
        public void StopMoving()
        {
            speedEffect.SetActive(false);
            foreach (var item in movingStars)
            {
                item.StopMoving();
            }
        }
        public void StartMoving()
        {
            speedEffect.SetActive(true);

            foreach (var item in movingStars)
            {
                item.MoveAtStandardSpeed();
            }
        }
        public void ShowStars()
        {
            stars.SetActive(true);
        }
        public void HideStars()
        {
            stars.SetActive(false);

        }
        public void BlueBackGround()
        {
            camera.backgroundColor = blueColor;
        }
        public void GrayBackGround()
        {
            camera.backgroundColor = grayColor;
        }
    }
}