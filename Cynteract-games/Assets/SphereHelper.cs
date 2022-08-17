using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Cynteract.Sphererunner {
    public class SphereHelper : MonoBehaviour
    {
        public enum Type
        {
            Big, Small
        }
        public float time;
        public Type type;
        private void OnTriggerEnter2D(Collider2D collision)
        {
            SphereMovement sphereMovement = collision.GetComponent<SphereMovement>();
            if (sphereMovement)
            {
                SphererunnerTutorial.instance.ShowHintAfterTime(type, time);
            }
        }
        private void OnTriggerExit2D(Collider2D collision)
        {
            SphereMovement sphereMovement = collision.GetComponent<SphereMovement>();
            if (sphereMovement)
            {
                SphererunnerTutorial.instance.AborthowHintAfterTime();
            }
        }
    }
}