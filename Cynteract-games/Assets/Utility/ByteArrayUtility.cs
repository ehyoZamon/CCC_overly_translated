using Cynteract.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ByteArrayUtility 
{
    public const int  QuaternionLength= 16;
    public const int  DateTimeLength= 8;
    public static byte[] ToByteArray(Quaternion quaternion)
    {
        byte[] xByteArray = BitConverter.GetBytes(quaternion.x);
        byte[] yByteArray = BitConverter.GetBytes(quaternion.y);
        byte[] zByteArray = BitConverter.GetBytes(quaternion.z);
        byte[] wByteArray = BitConverter.GetBytes(quaternion.w);
        return CArrayUtility.Join(xByteArray,
                                 yByteArray,
                                 zByteArray,
                                 wByteArray);
    }
    public static Quaternion ToQuaternion(byte[] byteArray)
    {
        float x = BitConverter.ToSingle(byteArray, 0);
        float y = BitConverter.ToSingle(byteArray, 4);
        float z = BitConverter.ToSingle(byteArray, 8);
        float w = BitConverter.ToSingle(byteArray, 12);
        return new Quaternion(x, y, z, w);
    }
    
    public static Quaternion[] ToQuaternionArray(byte[] byteArray)
    {
        var byteArrays = CArrayUtility.CutIntoEqualParts(byteArray, QuaternionLength);
        return CArrayUtility.Join(byteArrays.Select(x => ToQuaternion(x)).ToArray());
    }
    public static byte[] ToByteArray(Quaternion[] quaternions)
    {
        return CArrayUtility.Join(quaternions.Select(x => ByteArrayUtility.ToByteArray(x)).ToArray());
    }
    public static DateTime ToDateTime(byte[] data)
    {
        return DateTime.FromBinary(BitConverter.ToInt64(data, 0));
    }
    public static byte[] ToByteArray(DateTime dateTime)
    {
        return BitConverter.GetBytes(dateTime.Ticks);
    }
}
