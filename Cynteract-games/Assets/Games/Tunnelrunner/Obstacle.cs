using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Cynteract.Tunnelrunner
{


    public class Obstacle : MonoBehaviour
    {
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.GetComponent<RocketControl>())
            {
                Tunnelrunner.instance.Destruction();
            }
        }
    }
}