using Cynteract.CGlove;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxesFinder : MonoBehaviour
{
    public AxisFinder right, up, forward;
    public Vector3 rightAxis, upAxis, forwardAxis;
    public bool calculateRight, calculateUp, calculateForward;
    Quaternion rawRotation,calibRotation;
    public Transform rawCube, calibCube;
    private void Start()
    {
        Glove.Any.Subscribe(OnGloveData);
    }

    private void OnGloveData(Glove data)
    {
        Quaternion rotation = data.data.wristRotation.absolute;
        rawRotation = rotation;
        if (calculateRight)
        {
            rightAxis = right.CalculateAxis(rotation);
        }
        if (calculateUp)
        {
            upAxis = up.CalculateAxis(rotation);
        }
        if (calculateForward)
        {
            forwardAxis = forward.CalculateAxis(rotation);
        }
        RotationFunctions.SwingTwist(rotation, rightAxis, out Quaternion rightTwist, out Quaternion _);
        RotationFunctions.SwingTwist(rotation, upAxis, out Quaternion upTwist, out Quaternion _);
        RotationFunctions.SwingTwist(rotation, forwardAxis, out Quaternion forwardTwist, out Quaternion _);

        rightTwist = Remap(rightTwist, Vector3.right);
        upTwist = Remap(upTwist, Vector3.up);
        forwardTwist = Remap(forwardTwist, Vector3.forward);

        this.calibRotation = upTwist;
    }

    private static Quaternion Remap(Quaternion rightTwist, Vector3 axis)
    {
        rightTwist.ToAngleAxis(out float rightAngle, out Vector3 _);
        rightTwist = Quaternion.AngleAxis(rightAngle, axis);
        return rightTwist;
    }

    private void Update()
    {
        rawCube.rotation = rawRotation;
        calibCube.rotation = calibRotation;
    }
}
