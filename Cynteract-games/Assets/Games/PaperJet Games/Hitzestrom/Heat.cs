using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Heat : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        Paperjet paperjet = other.GetComponent<Paperjet>();
        if (paperjet)
        {
            paperjet.InHeat();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        Paperjet paperjet = other.GetComponent<Paperjet>();
        if (paperjet)
        {
            paperjet.NotInHeat();
        }
    }
}
