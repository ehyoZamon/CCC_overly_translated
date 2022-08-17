using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Cynteract.SpaceInvaders
{
    public class HeartBonus : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.tag == "Player")
            {
                Player.instance.Heal(1);
                Destroy(gameObject);
            }
        }
    }
}