using Cynteract.SpaceInvaders;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            SpaceInvaders.instance.AddScore(1, transform);
            SpaceInvaders.instance.PlayerGotMoney();
            Destruction();
        }
    }
    void Destruction()
    {
        Destroy(gameObject);
    }
}
