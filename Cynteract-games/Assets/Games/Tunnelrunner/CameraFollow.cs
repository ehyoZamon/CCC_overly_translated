using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {
    Vector3 refVelocity;
    public Transform target;
    public Vector3 offset;
    [Header("Directions to Ignore")]
    public bool x;
    public bool y;
    public bool z;

    private void FixedUpdate()
    {

        Vector3 pos = target.position;
        if (x)
        {
            pos.x=0;
        }
        if (y)
        {
            pos.y = 0;

        }
        if (z)
        {

            pos.z = 0;

        }
        pos +=offset;
        transform.position = Vector3.SmoothDamp(transform.position, pos, ref refVelocity, .5f);
    }

    public void ResetPosition() {
        transform.position = offset;
    }
}
