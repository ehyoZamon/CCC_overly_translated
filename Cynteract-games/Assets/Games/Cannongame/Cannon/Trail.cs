using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trail : MonoBehaviour
{
    public Transform cannonball;
    void Update()
    {
        if (cannonball)
        {
            transform.position = cannonball.position;

        }
        else
        {
            Destroy(this);
        }
    }
}
