using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AesTest : MonoBehaviour
{
    [Button]
    public byte[] Encrypt(string s)
    {
        return AesEncryption.Encrypt(s);
    }
    [Button]
    public string DecryptToString(byte [] bytes)
    {
        return AesEncryption.DecryptToString(bytes);
    }
    [Button]
    public byte[] Encrypt(byte[] s)
    {
        return AesEncryption.Encrypt(s);
    }
    [Button]
    public byte[] DecryptToBytes(byte[] bytes)
    {
        return AesEncryption.DecryptToBytes(bytes);
    }
}
