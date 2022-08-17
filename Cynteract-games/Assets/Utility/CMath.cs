using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CMath  {
    public static float ToRad(float degrees)
    {
        return degrees / 360*(2*Mathf.PI);
    }
    public static float ToDeg(float radians)
    {
        return radians /  (2 * Mathf.PI)*360;
    }

    internal static Quaternion Conjugate(Quaternion rotation)
    {
        var rot = new Quaternion(-rotation.x, -rotation.y, -rotation.z, rotation.w);
        return rot.normalized;
    }

    public static int[] Partialize(int num, int div)
    {
        int[] parts = new int[div];
        parts[0] = RoundUpToInt((double)num / div);
        for (int i = 1; i < num/ parts[0]; i++)
        {
            parts[i] = parts[0];
        }
        if (num % parts[0]!=0)
        {
            parts[parts.Length - 1] = num % parts[0];

        }
        return parts;
    }
    public int RoundToInt(double num)
    {
        int rounded = (int)num;
        if (num-rounded>=0.5)
        {
            return rounded + 1;
        }
        return rounded;
    }
    public static int RoundUpToInt(double num)
    {
        int rounded = (int)num;
        if (num - rounded >0)
        {
            return rounded + 1;
        }
        return rounded;
    }
    public static Quaternion Remap(Quaternion input, Vector3 refAxis, Vector3 axis)
   {
        float angle;
        Vector3 rotAxis;
        input.ToAngleAxis(out angle, out rotAxis);
        if (Vector3.Dot(rotAxis, refAxis) <0)
        {
            axis *= -1;
        }
        var between =Quaternion.FromToRotation(rotAxis, axis);
        Quaternion output = between * input * Quaternion.Inverse(between);
        return output;
   }

    public static Quaternion Multiply(Quaternion input, Vector3 refAxis, float calibValue)
    {
        /*
        float angle;
        Vector3 rotAxis;
        input.ToAngleAxis(out angle, out rotAxis);
        if (Vector3.Dot(rotAxis, refAxis) < 0)
        {
            rotAxis *= -1;
        }
        angle *= calibValue;

        return Quaternion.AngleAxis(angle, rotAxis);
        */
        return new Quaternion(input.x, input.y, input.z, calibValue * input.w);
    }
    Quaternion EulerAndBack(Quaternion rot)
    {
        Vector3 meinEuler = rot.eulerAngles;
        return Quaternion.Euler(meinEuler);
    }
}
