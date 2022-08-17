using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Cynteract.Cannongame
{
    public class GreenZone : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {


            if (collision.gameObject.tag == "Cannonball")
            {


                //some funky shit

                Destroy(collision.gameObject);
                Cannongame.instance.ReachedGoal();
            }
        }
    }
}
