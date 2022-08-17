using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Heat2D : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Paperjet paperjet = collision.GetComponent<Paperjet>();
        if (paperjet)
        {
            paperjet.InHeat();
        }
        
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        Paperjet paperjet = collision.GetComponent<Paperjet>();
        if (paperjet)
        {
            paperjet.NotInHeat();
        }
    }
}