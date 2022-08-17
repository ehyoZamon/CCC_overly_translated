using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class ByteConversion 
{
    public static byte[] ConvertToByteArray<T>(T objectToConvert)
    {
        var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
        byte[] plainBytes;
        using (var memstream = new MemoryStream())
        {
            binaryFormatter.Serialize(memstream, objectToConvert);
            plainBytes = memstream.ToArray();
        }
        return plainBytes;
    }
    public static T ConvertToObject<T>(byte[] bytes)
    {
        T objectToReturn;
        var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
        using (var memstream = new MemoryStream(bytes))
        {
            objectToReturn =(T)binaryFormatter.Deserialize(memstream);
        }
        return objectToReturn;
    }
}
