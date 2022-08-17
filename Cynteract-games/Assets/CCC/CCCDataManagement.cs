using Cynteract.CCC;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Video;

public static class DataManagementUtility
{
    public static string GetHash(string username)
    {
        if (username == null)
        {
            username = "default";
        }
        return username;
        /*
        string hashedUsername = SecurePasswordHasher.Hash(username);
        byte[] byteArray = Encoding.UTF8.GetBytes(hashedUsername);
        var hexString = BitConverter.ToString(byteArray, 0);
        string userHash = hexString.Replace("-", "");
        return userHash;
        */
    }

}