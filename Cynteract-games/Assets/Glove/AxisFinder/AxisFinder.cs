using Cynteract.CGlove;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class AxisFinder 
{
    public bool draw;
    public List<Vector3> axes = new List<Vector3>();
    public List<Vector3> leftSide = new List<Vector3>(), rightSide = new List<Vector3>();
    public Vector3 axis, currentAxis;
    Queue<Vector3> lastAxes = new Queue<Vector3>();
    Quaternion current, last;

    
    public Vector3 CalculateAxis(Quaternion rotation)
    {
        current = rotation;
        currentAxis = GetAxis(last, current);
        Queue<Vector3> newLastAxes = new Queue<Vector3>();
        while (lastAxes.Count > 5)
        {
            lastAxes.Dequeue();
        }
        bool goodMotion = true;
        while (lastAxes.Count > 0)
        {
            var axis = lastAxes.Dequeue();
            if (Vector3.Angle(axis, currentAxis) > 10)
            {
                goodMotion = false;
            }
            newLastAxes.Enqueue(axis);
        }
        if (goodMotion && currentAxis != Vector3.zero)
        {
            axes.Add(currentAxis);

        }
        lastAxes = newLastAxes;
        lastAxes.Enqueue(currentAxis);
        last = current;
        Plane plane;
        leftSide = new List<Vector3>();
        rightSide = new List<Vector3>();
        Vector3 rightAvg = Vector3.zero, leftAvg = Vector3.zero;
        if (axes.Count > 0)
        {
            plane = new Plane(axes[0], 0);

            foreach (var item in axes)
            {
                if (plane.GetSide(item))
                {
                    rightAvg += item;
                    rightSide.Add(item);
                }
                else
                {
                    leftAvg += item;
                    leftSide.Add(item);
                }
            }
        }
        leftAvg *= 1f / leftSide.Count;
        rightAvg *= 1f / rightSide.Count;
        axis = (rightAvg - leftAvg).normalized;
        return axis;
    }


    private Vector3 GetAxis(Quaternion prev, Quaternion current)
    {
        var fromTo = Quaternion.Inverse(prev) * current;
        return new Vector3(fromTo.x, fromTo.y, fromTo.z).normalized;
    }
}
