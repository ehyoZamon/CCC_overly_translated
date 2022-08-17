using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Cynteract.SpaceInvaders
{
    public class ShieldBonus : MonoBehaviour
    {

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.tag == "Player")
            {
                Player.instance.ActivateShield();
                Destroy(gameObject);
            }
        }
    }
}