using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Cynteract.Platformer
{
    public class PlayerEmote : MonoBehaviour
    {
        public GameObject happyEmote, surpisedEmote;
        public GameObject leftPupil, rightPupil, leftEye, rightEye, closedLeftEye, closedRightEye, sunglasses;
        public enum EmoteType
        {
            Happy,
            Surprised
        }
        public void SetEmote(EmoteType emoteType)
        {
            switch (emoteType)
            {
                case EmoteType.Happy:
                    happyEmote.SetActive(true);
                    surpisedEmote.SetActive(false);
                    break;
                case EmoteType.Surprised:
                    surpisedEmote.SetActive(true);
                    happyEmote.SetActive(false);
                    break;
            }
        }

        public void Look(Vector2 vector2)
        {
            var vector = Vector2.ClampMagnitude(vector2, .2f);
            leftPupil.transform.localPosition = vector;
            rightPupil.transform.localPosition = vector;
        }
        public void SetEyesOpen(bool left, bool right)
        {
            leftEye.SetActive(left);
            rightEye.SetActive(right);
            closedLeftEye.SetActive(!left);
            closedRightEye.SetActive(!right);
        }

        public void ActivateSunglasses()
        {
            sunglasses.SetActive(true);
        }
        public void DeactivateSunglasses()
        {
            sunglasses.SetActive(false);
        }
    }
}