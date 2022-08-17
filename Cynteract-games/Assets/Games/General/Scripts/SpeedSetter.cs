using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Cynteract.Sphererunner
{
    public class SpeedSetter : MonoBehaviour
    {
        [Range(0, 1)]
        public float speed = 1;
        private void OnTriggerEnter2D(Collider2D collision)
        {
            SphereMovement sphereMovement = collision.GetComponent<SphereMovement>();
            if (sphereMovement)
            {
                if (GameSettings.GetBool(CSettings.Sphererunner.autoMove))
                {
                    sphereMovement.autoDirection = speed * Mathf.Sign(sphereMovement.autoDirection);
                }
            }
        }
    }
}
