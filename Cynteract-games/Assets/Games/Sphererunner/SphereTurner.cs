using Cynteract.Sphererunner;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereTurner : MonoBehaviour
{
    public enum Direction
    {
        left=-1,
        right=1
    }
    public Direction direction=Direction.left;
    private void OnTriggerEnter2D(Collider2D other)
    {
        SphereMovement sphereMovement = other.GetComponent<SphereMovement>();
        if (sphereMovement)
        {
            sphereMovement.autoDirection = (int)direction*Mathf.Abs(sphereMovement.autoDirection);
        }
    }
}
