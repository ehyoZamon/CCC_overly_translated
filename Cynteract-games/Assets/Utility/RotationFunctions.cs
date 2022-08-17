using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationFunctions
{

    public static void SwingTwist(Quaternion rotation, Vector3 axis, out Quaternion twist, out Quaternion swing)
    {
        Vector3 rotationAxis = new Vector3(rotation.x, rotation.y, rotation.z);
        Vector3 projectedVector = Vector3.Project(rotationAxis, axis);
        twist = new Quaternion(projectedVector.x, projectedVector.y, projectedVector.z, rotation.w);
        twist.Normalize();
        swing = rotation * CMath.Conjugate(twist);
    }

    public static float ProjectToPlane(Quaternion rotation, Vector3 axis, Vector3 planeNormal)
    {

        Plane plane = new Plane(planeNormal, 0);
        Vector3 rotatedDirection = rotation * axis;
        float angle = CMath.ToDeg(Mathf.Asin(plane.GetDistanceToPoint(rotatedDirection)));
        return angle;
    }

}
