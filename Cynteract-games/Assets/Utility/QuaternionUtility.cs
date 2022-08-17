using Cynteract.Utility;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class QuaternionUtility
{

    public static float[] ToFloatArray(Quaternion quaternion)
    {
        return new float[] { quaternion.x, quaternion.y, quaternion.z, quaternion.w };
    }
    public static float[] ToFloatArray(Quaternion[] quaternions)
    {
        return CArrayUtility.Join(quaternions.Select(x => ToFloatArray(x)).ToArray());
    }
    public static float[][] ToFloatArrayArray(Quaternion[] quaternions)
    {
        return quaternions.Select(x => ToFloatArray(x).ToArray()).ToArray();
    }
    public static float[] ToFloatArray(HandPackage handPackage)
    {
        return CArrayUtility.Join(handPackage.parts.Select(x => ToFloatArray(x)).ToArray());
    }
    public static float[][] ToFloatArrayArray(HandPackage[] handPackages)
    {
        return handPackages.Select(x => ToFloatArray(x).ToArray()).ToArray();
    }

    public class HandPackage
    {
        public Quaternion[] parts;

        public HandPackage(Quaternion[] parts)
        {
            this.parts = parts;
        }
        public static HandPackage Random()
        {
            Quaternion[] parts = new Quaternion[12];
            for (int i = 0; i < 12; i++)
            {
                parts[i] = new Quaternion(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value).normalized;
            }
            return new HandPackage(parts);
        }
    }
}